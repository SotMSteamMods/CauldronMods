using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Gyrosaur
{
    public class ReadTheTerrainCardController : GyrosaurUtilityCardController
    {
        public ReadTheTerrainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowDamageDealt(new LinqCardCriteria(CharacterCard), thisTurn: true);
        }

        public override void AddTriggers()
        {
            //"At the start of your turn, reveal the top card of your deck and replace or discard it.",
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, RevealTopCard_PutItBackOrDiscardIt_Response, TriggerType.RevealCard);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"If {Gyrosaur} deals no damage this turn, increase damage dealt by {Gyrosaur} during your next turn to non-hero targets by 1."
            int numDamageIncrease = GetPowerNumeral(0, 1);
            var damageTrackerEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(CheckTrackedDamage), $"If {DecisionMaker.Name} deals no damage this turn, increase damage they deal during their next turn by {numDamageIncrease}.", new TriggerType[] { TriggerType.CreateStatusEffect, TriggerType.IncreaseDamage }, Card);
            damageTrackerEffect.TurnIndexCriteria.GreaterThan = Game.TurnIndex;
            damageTrackerEffect.TurnTakerCriteria.IsOneOfTheseTurnTakers = GameController.AllTurnTakers.ToList();
            damageTrackerEffect.TurnPhaseCriteria.IsEphemeral = false; 
            damageTrackerEffect.NumberOfUses = 1;
            damageTrackerEffect.CanEffectStack = true;
            damageTrackerEffect.UntilTargetLeavesPlay(CharacterCard);
            damageTrackerEffect.SetPowerNumeralsArray(new int[] { numDamageIncrease });

            IEnumerator coroutine = AddStatusEffect(damageTrackerEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public IEnumerator CheckTrackedDamage(PhaseChangeAction pc, OnPhaseChangeStatusEffect trackerEffect)
        {
            IEnumerator coroutine;
            var usingHeroCharacter = trackerEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            var usedTurnIndex = trackerEffect.TurnIndexCriteria.GreaterThan;
            var dealtDamage = GameController.Game.Journal.DealDamageEntries().Any((DealDamageJournalEntry ddje) => ddje.TurnIndex == usedTurnIndex && ddje.Amount > 0 && ddje.SourceCard == usingHeroCharacter);
            if(dealtDamage)
            {
                coroutine = GameController.SendMessageAction($"{usingHeroCharacter.Title} dealt damage, and will not recieve a damage boost from {trackerEffect.CardSource.Title}", Priority.Medium, FindCardController(trackerEffect.CardSource).GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                int numDamageIncrease = trackerEffect.PowerNumeralsToChange?.FirstOrDefault() ?? 1;
                if (Game.ActiveTurnTaker == usingHeroCharacter.Owner)
                {
                    var boostEffect = new IncreaseDamageStatusEffect(numDamageIncrease);
                    boostEffect.SourceCriteria.IsSpecificCard = usingHeroCharacter;
                    boostEffect.TargetCriteria.IsHero = false;
                    boostEffect.TargetCriteria.IsTarget = true;
                    boostEffect.UntilTargetLeavesPlay(usingHeroCharacter);
                    boostEffect.UntilThisTurnIsOver(Game);
                    boostEffect.CardSource = trackerEffect.CardSource;

                    coroutine = AddStatusEffect(boostEffect);
                }
                else
                {
                    var damageSpawnerEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(IncreaseDamageThisTurn), $"Increase damage dealt by {usingHeroCharacter.Title} during their next turn by {numDamageIncrease}.", new TriggerType[] { TriggerType.CreateStatusEffect, TriggerType.IncreaseDamage }, trackerEffect.CardSource);
                    damageSpawnerEffect.TurnTakerCriteria.IsSpecificTurnTaker = usingHeroCharacter.Owner;
                    damageSpawnerEffect.TurnPhaseCriteria.IsEphemeral = false;
                    damageSpawnerEffect.NumberOfUses = 1;
                    damageSpawnerEffect.CanEffectStack = true;
                    damageSpawnerEffect.UntilTargetLeavesPlay(usingHeroCharacter);
                    damageSpawnerEffect.CardSource = trackerEffect.CardSource;
                    damageSpawnerEffect.SetPowerNumeralsArray(new int[] { numDamageIncrease });

                    coroutine = AddStatusEffect(damageSpawnerEffect);
                }
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public IEnumerator IncreaseDamageThisTurn(PhaseChangeAction pc, OnPhaseChangeStatusEffect spawnerEffect)
        {
            int numDamageIncrease = spawnerEffect.PowerNumeralsToChange?.FirstOrDefault() ?? 1;
            var usingHeroCharacter = spawnerEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();

            var boostEffect = new IncreaseDamageStatusEffect(numDamageIncrease);
            boostEffect.SourceCriteria.IsSpecificCard = usingHeroCharacter;
            boostEffect.TargetCriteria.IsHero = false;
            boostEffect.TargetCriteria.IsTarget = true;
            boostEffect.UntilTargetLeavesPlay(usingHeroCharacter);
            boostEffect.UntilThisTurnIsOver(Game);
            boostEffect.CardSource = spawnerEffect.CardSource;

            IEnumerator coroutine = AddStatusEffect(boostEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        protected IEnumerator RevealTopCard_PutItBackOrDiscardIt_Response(PhaseChangeAction pca)
        {
            List<Card> cards = new List<Card>();
            IEnumerator coroutine = GameController.RevealCards(HeroTurnTakerController, TurnTaker.Deck, 1, cards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            Card revealedCard = GetRevealedCard(cards);
            if (revealedCard != null)
            {
                YesNoDecision yesNo = new YesNoCardDecision(GameController, DecisionMaker, SelectionType.DiscardCard, revealedCard, cardSource: GetCardSource());
                List<IDecision> decisionSources = new List<IDecision>
                {
                    yesNo
                };
                IEnumerator coroutine2 = GameController.MakeDecisionAction(yesNo);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine2);
                }
                if (DidPlayerAnswerYes(yesNo))
                {
                    IEnumerator coroutine3 = GameController.DiscardCard(DecisionMaker, revealedCard, decisionSources, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine3);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine3);
                    }
                }
                if (yesNo != null && yesNo.Completed && yesNo.Answer.HasValue)
                {
                    decisionSources.Add(yesNo);
                    if (!yesNo.Answer.Value)
                    {
                        IEnumerator coroutine4 = GameController.MoveCard(TurnTakerController, revealedCard, TurnTaker.Deck, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine4);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine4);
                        }
                    }
                }
            }
            List<Location> list = new List<Location>();
            list.Add(TurnTaker.Revealed);
            IEnumerator coroutine5 = CleanupCardsAtLocations(list, TurnTaker.Deck, cardsInList: cards);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine5);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine5);
            }
        }
    }
}

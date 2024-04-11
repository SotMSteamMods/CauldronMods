using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class UnstablePyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public UnstablePyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {

            int numCards = GetPowerNumeral(0, 1);
            //"{PyreIrradiate} 1 non-{PyreIrradiate} card in a player's hand until it leaves their hand."
            var viableHeroes = GameController.AllHeroes.Where(htt => GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource()) && htt.Hand.Cards.Any(c => !c.IsIrradiated())).Select(htt => htt as TurnTaker);
            var decision = new SelectTurnTakerDecision(GameController, DecisionMaker, viableHeroes, SelectionType.Custom, cardSource: GetCardSource());
            CurrentMode = CustomMode.PlayerToIrradiate;
            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(decision, tt => SelectAndIrradiateCardsInHand(DecisionMaker, tt, numCards, numCards));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //example yes-no decision to show it with the same CustomDecisionText
            /*
            CurrentMode = CustomMode.PlayerToIrradiate;
            var testDecision = new YesNoCardDecision(GameController, DecisionMaker, SelectionType.Custom, Card, cardSource: GetCardSource());
            coroutine = GameController.MakeDecisionAction(testDecision);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            */

            // Play an equipment card. 
            var equipmentCriteria = new LinqCardCriteria((Card c) => IsEquipment(c), "equipment");
            coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, false, cardCriteria: equipmentCriteria, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle a cascade card from your trash into your deck.
            if (TurnTaker.Trash.Cards.Any((Card c) => GameController.IsCascade(c)))
            {
                var cardToMove = TurnTaker.Trash.Cards.Where((Card c) => GameController.IsCascade(c)).FirstOrDefault();
                coroutine = GameController.SendMessageAction($"{Card.Title} shuffles {cardToMove.Title} into {TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource(), new Card[] { cardToMove });
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.MoveCard(DecisionMaker, cardToMove, TurnTaker.Deck, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card. If they do, 1 target regains 4 HP.",
                        var heroesWithIrradiated = GameController.AllHeroes.Where(htt => htt.Hand.Cards.Any(card => card.IsIrradiated()) && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
                        var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesWithIrradiated, SelectionType.DiscardCard, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DiscardForGainHP);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"Select a target. That target is immune to damage during the next environment turn.",
                        var targetDecision = new SelectCardDecision(GameController, DecisionMaker, SelectionType.PreventDamage, GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource())), cardSource: GetCardSource());
                        coroutine = GameController.SelectCardAndDoAction(targetDecision, MakeImmuneToDamageNextEnvironmentTurn);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"One player may draw a card now."
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }

        private IEnumerator DiscardForGainHP(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var storedDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCard(heroTTC, true, card => card.IsIrradiated(), storedDiscard, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(storedDiscard))
            {
                coroutine = GameController.SelectAndGainHP(DecisionMaker, 4, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        private IEnumerator MakeImmuneToDamageNextEnvironmentTurn(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null)
            {
                var chosenCard = scd.SelectedCard;
                var holderEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(ActivateThisTurnImmunity), $"On the environment's next turn, {chosenCard.Title} is immune to damage.", new TriggerType[] { TriggerType.Hidden }, this.Card);
                holderEffect.UntilTargetLeavesPlay(chosenCard);
                holderEffect.CanEffectStack = true;
                holderEffect.TurnPhaseCriteria.TurnTaker = FindEnvironment().TurnTaker;
                holderEffect.TurnTakerCriteria.IsEnvironment = true;
                holderEffect.NumberOfUses = 1;
                holderEffect.BeforeOrAfter = BeforeOrAfter.After;
                holderEffect.CardSource = CharacterCard;

                IEnumerator coroutine = AddStatusEffect(holderEffect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public IEnumerator ActivateThisTurnImmunity(PhaseChangeAction pc, StatusEffect holderEffect)
        {
            var cardToImmunize = holderEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            if(cardToImmunize == null || !cardToImmunize.IsInPlayAndHasGameText)
            {
                yield break;
            }

            var activeEffect = new ImmuneToDamageStatusEffect();
            activeEffect.UntilThisTurnIsOver(Game);
            activeEffect.TargetCriteria.IsSpecificCard = cardToImmunize;
            activeEffect.CardSource = CharacterCard;
            activeEffect.UntilTargetLeavesPlay(cardToImmunize);

            IEnumerator coroutine = AddStatusEffect(activeEffect, showMessage: true);
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
    }
}

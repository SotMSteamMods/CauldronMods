using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ParticleColliderCardController : PyreUtilityCardController
    {
        public ParticleColliderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowIrradiatedCardsInHands();
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int numPlayers = GetPowerNumeral(0, 1);
            int numTargets = GetPowerNumeral(1, 1);
            int numDamage = GetPowerNumeral(2, 1);
            int numBoost = GetPowerNumeral(3, 3);
            //"1 player may play a {PyreIrradiate} card now. 
            var selectHeroes = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && tt.ToHero().Hand.Cards.Any((Card c) => IsIrradiated(c)), "hero with irradiated cards in hand"), SelectionType.PlayCard, numPlayers, false, numPlayers, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectHeroes, PlayIrradiatedCard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }


            //{Pyre} deals 1 target 1 energy damage. You may destroy a copy of Thermonuclear Core. If you do, increase this damage by 3."
            if (numTargets < 1)
            {
                yield break;
            }

            var coreInPlay = GameController.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "ThermonuclearCore").FirstOrDefault();
            ITrigger previewBoost = null;
            if (coreInPlay != null)
            {
                previewBoost = AddIncreaseDamageTrigger((DealDamageAction dd) => !IsRealAction() && dd.CardSource != null && dd.CardSource.Card == Card && dd.CardSource.PowerSource != null, dd => 3);
            }

            //select the targets
            var targetDecision = new SelectTargetsDecision(GameController,
                                            DecisionMaker,
                                            (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                                            1,
                                            false,
                                            1,
                                            false,
                                            new DamageSource(GameController, CharacterCard),
                                            numDamage,
                                            DamageType.Energy,
                                            selectTargetsEvenIfCannotPerformAction: false,
                                            cardSource: GetCardSource());
            coroutine = GameController.SelectCardsAndDoAction(targetDecision, _ => DoNothing());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (coreInPlay != null)
            {
                RemoveTrigger(previewBoost);
            }

            var selectedTargets = targetDecision.SelectCardDecisions.Select(scd => scd.SelectedCard).Where((Card c) => c != null);
            if (selectedTargets.Count() == 0)
            {
                yield break;
            }

            //potentially destroy Core to boost the upcoming damage
            ITrigger boostTrigger = null;
            bool didDestroyCards = false;
            if (coreInPlay != null && coreInPlay.IsInPlayAndHasGameText)
            {
                var previewAction = new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), selectedTargets.FirstOrDefault(), numDamage, DamageType.Energy);
                var storedYesNo = new List<YesNoCardDecision> { };
                coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, coreInPlay, action: previewAction, storedResults: storedYesNo, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayerAnswerYes(storedYesNo))
                {
                    var destroyStorage = new List<DestroyCardAction>();
                    coroutine = GameController.DestroyCard(DecisionMaker, coreInPlay, storedResults: destroyStorage, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (DidDestroyCard(destroyStorage))
                    {
                        boostTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => dd.DamageSource.IsCard && dd.DamageSource.Card == CharacterCard && dd.CardSource != null && dd.CardSource.Card == this.Card && dd.CardSource.PowerSource != null, dd => GameController.IncreaseDamage(dd, numBoost, false, GetCardSource()), null, TriggerPriority.Medium, false, GetCardSource());
                        AddToTemporaryTriggerList(AddTrigger(boostTrigger));
                        didDestroyCards = true;
                    }
                }
            }

            //actually deal the damage
            coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => selectedTargets.Contains(c), numDamage, DamageType.Energy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (numTargets > 1)
            {
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), numDamage, DamageType.Energy, numTargets - 1, false, numTargets - 1, additionalCriteria: (Card c) => !selectedTargets.Contains(c), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (didDestroyCards)
            {
                RemoveTemporaryTrigger(boostTrigger);
            }
            yield break;
        }

        private IEnumerator PlayIrradiatedCard(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            IEnumerator coroutine = GameController.SelectAndPlayCardFromHand(heroTTC, true, cardCriteria: new LinqCardCriteria((Card c) => IsIrradiated(c), "irradiated"), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

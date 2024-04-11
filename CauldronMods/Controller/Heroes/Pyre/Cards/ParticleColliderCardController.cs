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
            this.ShowIrradiatedCardsInHands(SpecialStringMaker);
            SpecialStringMaker.ShowIfElseSpecialString(() => FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == ThermonuclearCoreIdentifier).Any(), () => "Thermonuclear Core is in play.", () => "Thermonuclear Core is not in play.");
        }

        public readonly string ThermonuclearCoreIdentifier = "ThermonuclearCore";
        public override IEnumerator UsePower(int index = 0)
        {
            int numPlayers = GetPowerNumeral(0, 1);
            int numTargets = GetPowerNumeral(1, 1);
            int numDamage = GetPowerNumeral(2, 1);
            int numBoost = GetPowerNumeral(3, 3);
            //"1 player may play a {PyreIrradiate} card now. 
            var selectHeroes = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) && tt.ToHero().Hand.Cards.Any((Card c) => c.IsIrradiated() && GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay), $"hero with {PyreExtensionMethods.Irradiated} cards in hand"), SelectionType.PlayCard, numPlayers, false, numPlayers, cardSource: GetCardSource());
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

            var coreInPlay = GameController.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && c.Identifier == ThermonuclearCoreIdentifier).FirstOrDefault();
            ITrigger previewBoost = null;
            CardSource cardSource = GetCardSource();
            if (coreInPlay != null)
            {
                previewBoost = AddTrigger((DealDamageAction dd) => !IsRealAction(dd) && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == CharacterCard && dd.CardSource == cardSource, IncreaseDamageDecision, TriggerType.IncreaseDamage, TriggerTiming.Before, isActionOptional: false);
            }

            //checks if we need to try to deal damage for a "prevent the next damage" trigger
            DamageSource source = new DamageSource(GameController, CharacterCard);
            bool hasNextDamagePrevention = false;
            CardSource preventionSource = GameController.CanDealDamage(CharacterCard, considerOutOfPlay: true, GetCardSource());
            if (preventionSource != null)
            {
                int? num = GameController.HowManyTimesIsDamagePrevented(source);
                if (num.HasValue)
                {
                    hasNextDamagePrevention = true;
                }
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
                                            selectTargetsEvenIfCannotPerformAction: hasNextDamagePrevention,
                                            cardSource: cardSource);
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
                //messaging for if damage cannot be dealt at all
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, source, 1, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
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
                        boostTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsCard && dd.DamageSource.Card == CharacterCard && dd.CardSource != null && dd.CardSource.Card == this.Card && dd.CardSource.PowerSource != null, dd => GameController.IncreaseDamage(dd, numBoost, false, GetCardSource()), null, TriggerPriority.Medium, false, GetCardSource());
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
            IEnumerator coroutine = GameController.SelectAndPlayCardFromHand(heroTTC, true, cardCriteria: new LinqCardCriteria((Card c) => c.IsIrradiated(), PyreExtensionMethods.Irradiated), cardSource: GetCardSource());
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

        private IEnumerator IncreaseDamageDecision(DealDamageAction dd)
        {

            //This is a fake method that just makes a decision to fool the preview into properly showing the question mark
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.Custom, base.Card,
                                        cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

    }
}

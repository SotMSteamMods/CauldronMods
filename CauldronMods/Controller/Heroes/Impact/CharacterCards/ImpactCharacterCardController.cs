using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;

namespace Cauldron.Impact
{
    public class ImpactCharacterCardController : ImpactSubCharacterCardController
    {
        public ImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private int _customText_numToDestroy;
        private int _customText_numBoost;

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} deals 1 target 1 infernal damage. You may destroy 1 hero ongoing card to increase this damage by 2."
            int numTargets = GetPowerNumeral(0, 1);
            int numDamage = GetPowerNumeral(1, 1);
            int numToDestroy = GetPowerNumeral(2, 1);
            int numBoost = GetPowerNumeral(3, 2);

            _customText_numBoost = numBoost;
            _customText_numToDestroy = numToDestroy;
            bool enoughOngoingsInPlay = GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsOngoing(c) && IsHero(c) && !c.IsBeingDestroyed && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Count() >= numToDestroy;

            if (numTargets < 1)
            {
                yield break;
            }

            ITrigger previewBoost = null;
            if(enoughOngoingsInPlay)
            {
                previewBoost = AddTrigger((DealDamageAction dd) => !IsRealAction(dd) && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == this.Card && dd.CardSource.Card == this.Card, IncreaseDamageDecision, TriggerType.IncreaseDamage, TriggerTiming.Before, isActionOptional: false);

            }

            //checks if we need to try to deal damage for a "prevent the next damage" trigger
            DamageSource source = new DamageSource(GameController, this.Card);
            bool hasNextDamagePrevention = false;
            CardSource preventionSource = GameController.CanDealDamage(Card, considerOutOfPlay: true, GetCardSource());
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
                                            source,
                                            numDamage,
                                            DamageType.Infernal,
                                            selectTargetsEvenIfCannotPerformAction: hasNextDamagePrevention,
                                            cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(targetDecision, _ => DoNothing());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (enoughOngoingsInPlay)
            {
                RemoveTrigger(previewBoost);
            }

            var selectedTargets = targetDecision.SelectCardDecisions.Select(scd => scd.SelectedCard).Where((Card c) => c != null);
            if (selectedTargets.Count() == 0)
            {
                //messaging for if damage cannot be dealt at all
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, source, 1, DamageType.Infernal, 1, false, 1, cardSource: GetCardSource());
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

            //potentially destroy ongoings to boost the upcoming damage
            ITrigger boostTrigger = null;
            bool didDestroyCards = false;
            if (enoughOngoingsInPlay)
            {
                var previewAction = new DealDamageAction(GetCardSource(), new DamageSource(GameController, this.Card), selectedTargets.FirstOrDefault(), numDamage, DamageType.Infernal);
                var storedYesNo = new List<YesNoCardDecision> { };
                coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.Custom, this.Card, action: previewAction, storedResults: storedYesNo, cardSource: GetCardSource());
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
                    coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && IsOngoing(c) && IsHero(c) && !c.IsBeingDestroyed && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing"), numToDestroy, false, numToDestroy, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    boostTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsCard && dd.DamageSource.Card == this.Card, dd => GameController.IncreaseDamage(dd, numBoost, false, GetCardSource()), null, TriggerPriority.Medium, false, GetCardSource());
                    AddToTemporaryTriggerList(AddTrigger(boostTrigger));
                    didDestroyCards = true;
                }
            }

            //actually deal the damage
            coroutine = GameController.DealDamage(DecisionMaker, this.Card, (Card c) => selectedTargets.Contains(c), numDamage, DamageType.Infernal, cardSource: GetCardSource());
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
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), numDamage, DamageType.Infernal, numTargets - 1, false, numTargets - 1, additionalCriteria: (Card c) => !selectedTargets.Contains(c), cardSource: GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One hero may use a power now.",
                        coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                case 1:
                    {
                        //"Select a hero target. That target deals 1 other target 1 projectile damage.",
                        var storedTarget = new List<SelectCardDecision> { };
                        coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && IsHeroTarget(c)), storedTarget, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedTarget))
                        {
                            Card damageDealer = GetSelectedCard(storedTarget);
                            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, damageDealer), 1, DamageType.Projectile, 1, false, 1, additionalCriteria: (Card c) => c != damageDealer, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //"Damage dealt to environment cards is irreducible until the start of your turn."
                        var envDamageEffect = new MakeDamageIrreducibleStatusEffect();
                        envDamageEffect.TargetCriteria.IsEnvironment = true;
                        envDamageEffect.UntilStartOfNextTurn(this.TurnTaker);
                        envDamageEffect.CardSource = CharacterCard;
                        coroutine = GameController.AddStatusEffect(envDamageEffect, true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            string cardWord = _customText_numToDestroy == 1 ? "card" : "cards";
            string baseMessage = $"destroy {_customText_numToDestroy} hero ongoing {cardWord} to increase this damage by {_customText_numBoost}";
            return new CustomDecisionText($"Do you want to {baseMessage}?", $"Should they {baseMessage}?", $"Vote for if they should {baseMessage}", $"{baseMessage}");
        }
    }
}
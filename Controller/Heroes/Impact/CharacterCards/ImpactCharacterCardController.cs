using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;

namespace Cauldron.Impact
{
    public class ImpactCharacterCardController : HeroCharacterCardController
    {
        public ImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} deals 1 target 1 infernal damage. You may destroy 1 hero ongoing card to increase this damage by 2."
            int numTargets = GetPowerNumeral(0, 1);
            int numDamage = GetPowerNumeral(1, 1);
            int numToDestroy = GetPowerNumeral(2, 1);
            int numBoost = GetPowerNumeral(3, 2);

            bool enoughOngoingsInPlay = GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsOngoing && c.IsHero && !c.IsBeingDestroyed && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Count() >= numToDestroy

            if (numTargets < 1)
            {
                yield break;
            }

            ITrigger previewBoost = null;
            if(enoughOngoingsInPlay)
            {
                previewBoost = AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == this.Card && dd.CardSource.Card == this.Card, numBoost);
            }

            //select the targets
            var targetDecision = new SelectTargetsDecision(GameController,
                                            DecisionMaker,
                                            (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                                            1,
                                            false,
                                            1,
                                            false,
                                            new DamageSource(GameController, this.Card),
                                            numDamage,
                                            DamageType.Infernal,
                                            selectTargetsEvenIfCannotPerformAction: false,
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

            RemoveTrigger(previewBoost);

            var selectedTargets = targetDecision.SelectCardDecisions.Select(scd => scd.SelectedCard).Where((Card c) => c != null);
            if (selectedTargets.Count() == 0)
            {
                yield break;
            }

            //potentially destroy ongoings to boost the upcoming damage
            ITrigger boostTrigger = null;
            bool didDestroyCards = false;
            if (enoughOngoingsInPlay)
            {
                var previewAction = new DealDamageAction(GetCardSource(), new DamageSource(GameController, this.Card), selectedTargets.FirstOrDefault(), numDamage, DamageType.Infernal);
                var storedYesNo = new List<YesNoCardDecision> { };
                coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.IncreaseDamage, this.Card, action: previewAction, storedResults: storedYesNo, cardSource: GetCardSource());
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
                    coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsOngoing && c.IsHero && !c.IsBeingDestroyed && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing"), numToDestroy, false, numToDestroy, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    boostTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => dd.DamageSource.IsCard && dd.DamageSource.Card == this.Card && dd.CardSource != null && dd.CardSource.Card == this.Card, dd => GameController.IncreaseDamage(dd, numBoost, false, GetCardSource()), null, TriggerPriority.Medium, false, GetCardSource());
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
                        coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHero), storedTarget, false, cardSource: GetCardSource());
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
    }
}
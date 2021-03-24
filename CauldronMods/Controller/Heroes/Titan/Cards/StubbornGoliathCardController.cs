using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class StubbornGoliathCardController : CardController
    {
        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (Game.StatusEffects.Any((StatusEffect se) => se.CardSource == this.Card))
                {
                    return false;
                }
                return true;
            }
        }
        public StubbornGoliathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private DealDamageAction _customTextDamageReference;

        public override IEnumerator UsePower(int index = 0)
        {
            int targetNumeral = base.GetPowerNumeral(0, 2);
            int damageNumeral = base.GetPowerNumeral(1, 2);

            //{Titan} deals up to 2 non-hero targets 2 infernal damage each.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Infernal, targetNumeral, false, 0, additionalCriteria: (Card c) => !c.IsHero, addStatusEffect: AddRedirectToSelfEffect, selectTargetsEvenIfCannotDealDamage: true, cardSource: base.GetCardSource());
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

        private IEnumerator AddRedirectToSelfEffect(DealDamageAction dd)
        {
            if (dd.Target != null)
            {
                var onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements,
                                                                            nameof(MaybeRedirectDamageResponse),
                                                                            $"When {dd.Target.Title} would deal damage, {DecisionMaker.Name} may redirect it to themselves.",
                                                                            new TriggerType[] { TriggerType.RedirectDamage, TriggerType.WouldBeDealtDamage },
                                                                            this.TurnTaker,
                                                                            this.Card);

                //Until the start of your next turn...
                onDealDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                //...when those targets would deal damage...
                onDealDamageStatusEffect.SourceCriteria.IsSpecificCard = dd.Target;
                onDealDamageStatusEffect.DamageAmountCriteria.GreaterThan = 0;

                //prevent it from asking to redirect from Titan to Titan
                onDealDamageStatusEffect.TargetCriteria.IsNotSpecificCard = this.CharacterCard;

                onDealDamageStatusEffect.UntilTargetLeavesPlay(dd.Target);
                onDealDamageStatusEffect.TargetLeavesPlayExpiryCriteria.Card = this.CharacterCard;
                onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.Before;
                onDealDamageStatusEffect.CanEffectStack = true;

                IEnumerator coroutine = base.AddStatusEffect(onDealDamageStatusEffect);
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

        public IEnumerator MaybeRedirectDamageResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //...you may redirect that damage to Titan.
            _customTextDamageReference = dd;
            var storedYesNo = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(FindHeroTurnTakerController(hero.ToHero()), SelectionType.Custom, this.Card, action: dd, storedResults: storedYesNo, cardSource: GetCardSource());
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
                coroutine = RedirectDamage(dd, TargetType.SelectTarget, (Card c) => hero.CharacterCards.Contains(c));
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            string customDamageString = $"{ _customTextDamageReference.Amount } { _customTextDamageReference.DamageType} aimed at { _customTextDamageReference.Target.AlternateTitleOrTitle} to { CharacterCard.AlternateTitleOrTitle}";
            return new CustomDecisionText($"Do you want to redirect {customDamageString}?", $"Should they redirect {customDamageString}?", $"Vote for if they should redirect {customDamageString}?", $"redirect {customDamageString}");

        }
    }
}
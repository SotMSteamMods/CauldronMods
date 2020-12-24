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
        public override bool AllowFastCoroutinesDuringPretend {
            get
            {
                if(Game.StatusEffects.Any((StatusEffect se) => se.CardSource == this.Card))
                {
                    return false;
                }
                return true;
            }
        }
        public StubbornGoliathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targetNumeral = base.GetPowerNumeral(0, 2);
            int damageNumeral = base.GetPowerNumeral(1, 2);

            List<SelectCardDecision> storedSelect = new List<SelectCardDecision>();
            //{Titan} deals up to 2 non-hero targets 2 infernal damage each.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Infernal, targetNumeral, false, 0, storedResultsDecisions: storedSelect, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Until the start of your next turn, when those targets would deal damage, you may redirect that damage to {Titan}.
            if (storedSelect.FirstOrDefault() != null)
            {
                List<StatusEffect> redirects = new List<StatusEffect> { };

                foreach (SelectCardDecision decision in storedSelect)
                {
                    if (decision.SelectedCard != null && decision.SelectedCard.IsInPlayAndHasGameText)
                    {
                        OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(this.CardWithoutReplacements,
                                                                                                "MaybeRedirectDamageResponse",
                                                                                                $"When {decision.SelectedCard.Title} would deal damage, {DecisionMaker.Name} may redirect it to themselves.",
                                                                                                new TriggerType[] { TriggerType.RedirectDamage, TriggerType.WouldBeDealtDamage },
                                                                                                this.TurnTaker,
                                                                                                this.Card);

                        //Until the start of your next turn...
                        onDealDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                        //...when those targets would deal damage...
                        onDealDamageStatusEffect.SourceCriteria.IsSpecificCard = decision.SelectedCard;
                        onDealDamageStatusEffect.DamageAmountCriteria.GreaterThan = 0;

                        //prevent it from asking to redirect from Titan to Titan
                        onDealDamageStatusEffect.TargetCriteria.IsNotSpecificCard = this.CharacterCard;

                        onDealDamageStatusEffect.UntilTargetLeavesPlay(decision.SelectedCard);
                        onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.Before;
                        onDealDamageStatusEffect.CanEffectStack = true;

                        redirects.Add(onDealDamageStatusEffect);
                    }
                }

                foreach (StatusEffect effect in redirects)
                {
                    coroutine = base.AddStatusEffect(effect);
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
            yield break;
        }

        public IEnumerator MaybeRedirectDamageResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //...you may redirect that damage to Titan.
            var storedYesNo = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(FindHeroTurnTakerController(hero.ToHero()), SelectionType.RedirectDamage, this.Card, storedResults: storedYesNo, cardSource:GetCardSource());
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
    }
}
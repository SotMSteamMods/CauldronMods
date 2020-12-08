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
            if (storedSelect.FirstOrDefault() != null && storedSelect.FirstOrDefault().SelectedCard != null)
            {
                Card firstCard = storedSelect.FirstOrDefault().SelectedCard;
                Card secondCard = storedSelect.LastOrDefault().SelectedCard;
                List<StatusEffect> redirects = new List<StatusEffect> { };

                if (firstCard.IsInPlay)
                {
                    RedirectDamageStatusEffect redirectDamageStatusEffect = new RedirectDamageStatusEffect();
                    //Until the start of your next turn...
                    redirectDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                    //...when those targets would deal damage...
                    redirectDamageStatusEffect.SourceCriteria.IsSpecificCard = firstCard;
                    //...you may redirect...
                    redirectDamageStatusEffect.IsOptional = true;
                    //...that damage to {Titan}.
                    redirectDamageStatusEffect.RedirectTarget = base.CharacterCard;
                    redirectDamageStatusEffect.UntilTargetLeavesPlay(firstCard);

                    //prevent it from asking to redirect from Titan to Titan
                    redirectDamageStatusEffect.TargetCriteria.IsNotSpecificCard = base.CharacterCard;

                    redirects.Add(redirectDamageStatusEffect);
                }
                if (firstCard != secondCard && secondCard.IsInPlay)
                {
                    RedirectDamageStatusEffect redirectDamageStatusEffect = new RedirectDamageStatusEffect();
                    redirectDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                    redirectDamageStatusEffect.SourceCriteria.IsSpecificCard = secondCard;
                    redirectDamageStatusEffect.IsOptional = true;
                    redirectDamageStatusEffect.RedirectTarget = base.CharacterCard;
                    redirectDamageStatusEffect.UntilTargetLeavesPlay(secondCard);
                    redirectDamageStatusEffect.TargetCriteria.IsNotSpecificCard = base.CharacterCard;
                    redirects.Add(redirectDamageStatusEffect);
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
    }
}
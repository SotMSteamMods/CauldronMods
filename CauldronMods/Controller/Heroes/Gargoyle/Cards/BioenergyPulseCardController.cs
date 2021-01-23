using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    // Whenever {Gargoyle} deals himself damage, he may also deal that to 1 other target.
    // Power
    // {Gargoyle} deals each non-hero target 1 toxic damage.
    // Increase the next damage {Gargoyle} deals by 1.
    public class BioenergyPulseCardController : GargoyleUtilityCardController
    {
        private int ToxicDamageAmount => GetPowerNumeral(0, 1);
        private int IncreaseNextDamageAmount => GetPowerNumeral(1, 1);

        public BioenergyPulseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override void AddTriggers()
        {
            // Whenever {Gargoyle} deals himself damage, he may also deal that to 1 other target.
            base.AddTrigger<DealDamageAction>(this.DealDamageCriteria, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            // {Gargoyle} deals each non-hero target 1 toxic damage.
            coroutine = base.DealDamage(base.CharacterCard, (Card card) => !card.IsHero, ToxicDamageAmount, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Increase the next damage {Gargoyle} deals by 1.
            coroutine = IncreaseGargoyleNextDamage(IncreaseNextDamageAmount);
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

        private bool DealDamageCriteria(DealDamageAction dda)
        {
            return dda.Target == base.CharacterCard && dda.DamageSource.Card == base.CharacterCard && dda.Amount > 0;
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            IEnumerator coroutine;

            // he may also deal that to 1 other target.
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), action.Amount, action.DamageType, 1, false, 0, additionalCriteria: (Card c) => c != CharacterCard, cardSource: base.GetCardSource());

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

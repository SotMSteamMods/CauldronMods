﻿using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class BloodThornAuraCardController : DendronBaseCardController
    {
        //==============================================================
        // Whenever a Tattoo would be dealt damage by a non-villain target,
        // it first deals that target {H  - 2} radiant damage.
        //==============================================================

        public static string Identifier = "BloodThornAura";

        public BloodThornAuraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>(dda => IsTattoo(dda.Target) && !dda.DamageSource.IsVillainTarget,
                this.DealDamageResponse,
                new[]
                {
                    TriggerType.DealDamage
                }, TriggerTiming.Before, null, false, true, true, orderMatters: true);

            base.AddTriggers();
        }

        private IEnumerator DealDamageResponse(DealDamageAction dda)
        {

            if (dda.IsPretend)
            {
                IEnumerator pretend = base.CancelAction(dda);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(pretend);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(pretend);
                }
            }
            else
            {
                int damageToDeal = this.Game.H - 2;

                IEnumerator dealDamageRoutine = this.DealDamage(dda.Target, dda.DamageSource.Card, damageToDeal, DamageType.Radiant);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageRoutine);
                }
            }


        }
    }
}
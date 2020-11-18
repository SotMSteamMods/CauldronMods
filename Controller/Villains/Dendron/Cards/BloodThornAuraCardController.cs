using System.Collections;

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
            base.AddTrigger<DealDamageAction>(dda => IsTattoo(dda.Target),
                this.DealDamageResponse,
                new[]
                {
                    TriggerType.DealDamage
                }, TriggerTiming.Before, null, false, true, true);

            base.AddTriggers();
        }

        private IEnumerator DealDamageResponse(DealDamageAction dda)
        {
            int damageToDeal = this.Game.H - 2;

            IEnumerator dealDamageRoutine = this.GameController.DealDamage(this.HeroTurnTakerController, dda.Target,
                card => card.Equals(dda.DamageSource), damageToDeal, DamageType.Radiant);

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
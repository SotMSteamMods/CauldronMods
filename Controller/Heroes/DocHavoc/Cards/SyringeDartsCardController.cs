using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class SyringeDartsCardController : CardController
    {
        //==============================================================
        // Deals up to 2 targets 2 projectile damage each.
        //==============================================================

        public static readonly string Identifier = "SyringeDarts";
        private const int DamageAmount = 2;

        public SyringeDartsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {

            // Deal up to 2 targets 2 projectile damage each.
            int targets = this.GetPowerNumeral(0, 2);
            int damage = this.GetPowerNumeral(1, DamageAmount);

            IEnumerator dealDamageRoutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(this.GameController, this.CharacterCard), damage, DamageType.Projectile,
                new int?(targets), false,
                new int?(0), cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}

using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class DamnGoodGroundCardController : TangoOneBaseCardController
    {
        //==============================================================
        // {TangoOne} regains 2HP and may deal up to 3 targets 1 projectile damage each.
        //==============================================================

        public static string Identifier = "DamnGoodGround";

        private const int DamageAmount = 1;
        private const int HpGain = 2;

        public DamnGoodGroundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {

            IEnumerator dealDamageRoutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(this.GameController, this.CharacterCard), DamageAmount, DamageType.Projectile,
                new int?(3), false,
                new int?(0), cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(dealDamageRoutine);
            }

            IEnumerator gainHpRoutine = this.GameController.GainHP(this.CharacterCard, 
                HpGain, cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(gainHpRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(gainHpRoutine);
            }
        }
    }
}
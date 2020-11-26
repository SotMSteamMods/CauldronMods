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

        public static readonly string Identifier = "DamnGoodGround";

        private const int DamageAmount = 1;
        private const int HpGain = 2;

        public DamnGoodGroundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerator gainHpRoutine = this.GameController.GainHP(this.CharacterCard,
                HpGain, cardSource: this.GetCardSource());

            IEnumerator dealDamageRoutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(this.GameController, this.CharacterCard), DamageAmount, DamageType.Projectile,
                3, false,
                0, cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(gainHpRoutine);
                yield return this.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(gainHpRoutine);
                this.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}
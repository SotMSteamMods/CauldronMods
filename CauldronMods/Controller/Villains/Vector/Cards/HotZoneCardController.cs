using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class HotZoneCardController : CardController
    {
        //==============================================================
        // {Vector} deals each non-villain target 2 toxic damage.
        //==============================================================

        public static readonly string Identifier = "HotZone";

        private const int DamageToDeal = 2;

        public HotZoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator routine = base.DealDamage(this.CharacterCard, c => c.IsTarget && !IsVillainTarget(c) && c.IsInPlayAndNotUnderCard, DamageToDeal, DamageType.Toxic);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class DisablingShotCardController : TangoOneBaseCardController
    {
        //==============================================================
        // You may destroy 1 ongoing card.
        // {TangoOne} may deal 1 target 2 projectile damage.
        //==============================================================

        public static string Identifier = "DisablingShot";

        public DisablingShotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            return base.Play();
        }
    }
}
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class DisablingShotCardController : TangoOneBaseCardController
    {
        public static string Identifier = "DisablingShot";

        public DisablingShotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
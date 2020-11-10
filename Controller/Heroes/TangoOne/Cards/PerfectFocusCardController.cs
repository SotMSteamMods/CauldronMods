using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PerfectFocusCardController : TangoOneBaseCardController
    {
        public static string Identifier = "PerfectFocus";

        public PerfectFocusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
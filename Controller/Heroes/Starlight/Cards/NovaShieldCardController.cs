using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class NovaShieldCardController : StarlightCardController
    {
        public NovaShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever a constellation enters play next to a target, Starlight deals that target 1 energy damage and regains 1 HP."
        }

    }
}
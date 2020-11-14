using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WarpHaloCardController : StarlightCardController
    {
        public WarpHaloCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"If a hero target next to a constellation would deal damage to a non-hero target next to a constellation, increase that damage by 1."
        }

    }
}
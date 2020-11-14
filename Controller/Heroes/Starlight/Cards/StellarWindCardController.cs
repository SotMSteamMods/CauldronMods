using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class StellarWindCardController : StarlightCardController
    {
        public StellarWindCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //"{Starlight} may deal 2 cold damage to each target next to a constellation.",
            //"Draw 2 cards."
            yield break;
        }

    }
}
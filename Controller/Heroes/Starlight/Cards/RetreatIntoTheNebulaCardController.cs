using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class RetreatIntoTheNebulaCardController : StarlightCardController
    {
        public RetreatIntoTheNebulaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Reduce damage dealt to {Starlight} by 2.",
            //"At the start of your turn, destroy a constellation card or destroy this card."
            yield break;
        }

    }
}
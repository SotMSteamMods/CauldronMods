using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class ExodusCardController : StarlightCardController
    {
        public ExodusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Draw a card, or search your trash and deck for a constellation card, put it into play, then shuffle your deck."
            //"You may play a card."
            yield break;
        }
    }
}
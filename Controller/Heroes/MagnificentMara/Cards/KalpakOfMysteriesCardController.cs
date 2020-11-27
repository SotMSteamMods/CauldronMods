using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class KalpakOfMysteriesCardController : CardController
    {
        public KalpakOfMysteriesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Reveal the top card of each other hero deck. One player may put their revealed card into their hand or into play. Replace or discard the other revealed cards. If a card is put into play this way, destroy this card."

            yield break;
        }
    }
}
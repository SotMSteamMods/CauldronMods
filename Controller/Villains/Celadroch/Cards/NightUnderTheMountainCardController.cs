using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class NightUnderTheMountainCardController : CardController
    {
        /*
         *  "When this card enters play, play the top card of the villain deck.",
			"At the start of the villain turn, destroy this card.",
			"When this card is destroyed, increase damage dealt by villain targets by 2 until the end of the villain turn."
         */

        public NightUnderTheMountainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
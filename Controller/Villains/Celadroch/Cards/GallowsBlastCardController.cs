using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class GallowsBlastCardController : CardController
    {
        /*
         * "{Celadroch} deals each hero 5 infernal damage. Each player discards a card.",
		   "Reduce damage dealt to villain targets by 2 until the start of the villain turn."
         */

        public GallowsBlastCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
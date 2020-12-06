using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LaughingHagCardController : CardController
    {
        /*
         *  "At the end of the villain turn, destroy 1 hero ongoing or equipment card.",
			"This card is immune to fire, lightning, cold, and toxic damage.",
			"Increase damage dealt to hero targets by 1."
         */

        public LaughingHagCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
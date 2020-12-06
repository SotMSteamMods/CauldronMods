using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LeechingBreathCardController : CardController
    {
        /*
         *  "Play this card next to the hero with the highest HP. That hero cannot use powers.",
			"At the end of the villain turn, this card deals the hero next to it {H} toxic damage."
         */

        public LeechingBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
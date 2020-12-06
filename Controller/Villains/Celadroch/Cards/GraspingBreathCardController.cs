using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class GraspingBreathCardController : CardController
    {
        /*
         * 	"Play this card next to the hero with the most cards in hand. That hero cannot draw cards.",
			"At the end of the villain turn, this card deals the hero next to it {H} psychic damage."
         */

        public GraspingBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
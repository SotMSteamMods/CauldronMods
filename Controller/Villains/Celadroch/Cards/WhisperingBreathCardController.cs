using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class WhisperingBreathCardController : CardController
    {
        /*
         * 	"Play this card next to the hero with the most cards in play. That hero cannot play cards.",
			"At the end of the villain turn, this card deals the hero next to it {H} sonic damage."
         */

        public WhisperingBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
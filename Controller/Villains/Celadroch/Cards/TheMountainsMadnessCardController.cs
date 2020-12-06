using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class TheMountainsMadnessCardController : CardController
    {
        /*
         * 	"Shuffle all targets from the villain trash into the villain deck.",
			"Play the top card of the villain deck."
         */

        public TheMountainsMadnessCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
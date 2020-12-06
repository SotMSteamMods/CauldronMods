using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LingeringExhalationCardController : CardController
    {
        /*
         * 	"When this card enters play, play the top card of the villain deck.",
			"Destroy this card when {Celadroch} is dealt 16 damage in a single round.",
			"When this card is destroyed, put all targets from the villain trash into play."
         */

        public LingeringExhalationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
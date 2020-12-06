using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class ScreamingGaleCardController : CeladrochOngoingCardController
    {
        /*
         * 	"When this card enters play, play the top card of the villain deck.",
			"Increase damage dealt by villain targets by 1."
         */

        public ScreamingGaleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
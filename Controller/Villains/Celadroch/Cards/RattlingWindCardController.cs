using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class RattlingWindCardController : CeladrochOngoingCardController
    {
        /*
         * 	"When this card enters play, play the top card of the villain deck.",
			"Whenever a hero draws a card, {Celadroch} deals them 1 projectile damage.",
			"When this card is destroyed, {Celadroch} deals each hero target 1 cold damage."
         */

        public RattlingWindCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
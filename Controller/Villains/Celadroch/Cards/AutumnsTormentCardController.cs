using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class AutumnsTormentCardController : CardController
    {
        /*
         * "When this card enters play, destroy all environment cards.",
		 * "Whenever a hero plays a card, this card deals them 2 lightning damage."
         */
        public AutumnsTormentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
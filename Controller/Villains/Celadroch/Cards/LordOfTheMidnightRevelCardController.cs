using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LordOfTheMidnightRevelCardController : CardController
    {
        /*
         *  "The first time another villain target would be dealt damage each turn, this card deals the source of that damage {H} melee damage.",
			"When this card is destroyed, destroy 1 hero ongoing or equipment card for each other non-character villain target in play."
         */

        public LordOfTheMidnightRevelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
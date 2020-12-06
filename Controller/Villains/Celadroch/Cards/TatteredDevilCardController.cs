using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class TatteredDevilCardController : CardController
    {
        /*
         * 	"At the end of the villain turn, this card deals each hero target 2 infernal damage.",
			"When Hollow Angel is dealt damage, it becomes immune to damage until this card is dealt damage or leaves play."
         */

        public TatteredDevilCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
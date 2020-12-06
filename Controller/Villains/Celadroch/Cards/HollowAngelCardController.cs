using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class HollowAngelCardController : CardController
    {
        /*
         * 	"At the end of the villain turn, this card deals each hero target 2 radiant damage.",
			"When Tattered devil is dealt damage, it becomes immune to damage until this card is dealt damage or leaves play."
         */

        public HollowAngelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
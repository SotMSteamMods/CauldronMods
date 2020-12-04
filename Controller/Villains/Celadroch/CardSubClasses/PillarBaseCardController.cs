using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron
{
    public abstract class PillarBaseCardController : CardController
    {
        protected PillarBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            /* H = 3, 6 usages
             * 6,  5,  4,  3, 2, 1
             * 21, 17, 13, 9, 5, 1
             * 
             * H = 4, 5 usages
             * 5,  4,  3,  2, 1
             * 20, 15, 10, 5, 0
             * 
             * H = 5, 4 usages
             * 4,  3,  2, 1
             * 19, 13, 7, 1
             */


        }
    }
}
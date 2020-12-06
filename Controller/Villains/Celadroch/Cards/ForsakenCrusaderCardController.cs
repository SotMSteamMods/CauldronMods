using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class ForsakenCrusaderCardController : CardController
    {
        public static readonly string Identifier = "ForsakenCrusader";

        /*
         * "At the start of the villain turn, play the top card of the villain deck.",
		   "At the end of the villain turn, this card deals the 2 hero targets with the highest HP {H - 2} melee damage each."
         */
         

        //IE - Mountain's Special Boy
        public ForsakenCrusaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
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


        //IE - Mountain's Special Boy
        public ForsakenCrusaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
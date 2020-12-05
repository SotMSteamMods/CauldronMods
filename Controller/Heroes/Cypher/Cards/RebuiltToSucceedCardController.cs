using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class RebuiltToSucceedCardController : CardController
    {
        //==============================================================
        // Select two Augments in your trash. Put one into your hand and one into play.
        // The hero you augment this way may play a card now.
        //==============================================================

        public static string Identifier = "RebuiltToSucceed";

        public RebuiltToSucceedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
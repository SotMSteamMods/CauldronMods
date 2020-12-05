using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class NaniteSurgeCardController : CardController
    {
        //==============================================================
        // You may draw a card.
        // You may play a card.
        // Each augmented hero regains X HP, where X is the number of Augments next to them.
        //==============================================================

        public static string Identifier = "NaniteSurge";

        public NaniteSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
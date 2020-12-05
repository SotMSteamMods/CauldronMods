using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class HackingProgramCardController : CardController
    {
        //==============================================================
        // Power: {Cypher} deals himself 2 irreducible energy damage.
        // If he takes damage this way, destroy 1 ongoing or environment card.
        //==============================================================

        public static string Identifier = "HackingProgram";

        public HackingProgramCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
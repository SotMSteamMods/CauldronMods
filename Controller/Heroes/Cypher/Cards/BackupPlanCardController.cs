using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class BackupPlanCardController : CardController
    {

        //==============================================================
        // When a non-hero card enters play, you may destroy this card.
        // If you do, select any number of Augments in play and move
        // each one next to a new hero. Then, each augmented hero regains 2HP.
        //==============================================================

        public static string Identifier = "BackupPlan";

        public BackupPlanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
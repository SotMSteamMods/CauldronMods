using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class BreakThroughCardController : CardController
    {
        //==============================================================
        // At the start of your turn, destroy this card.
        // Once during their turn, when 1 of a player's targets would deal damage,
        // they may increase that damage by 2
        //==============================================================

        public static string Identifier = "BreakThrough";

        public BreakThroughCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);

            base.AddTriggers();
        }
    }
}
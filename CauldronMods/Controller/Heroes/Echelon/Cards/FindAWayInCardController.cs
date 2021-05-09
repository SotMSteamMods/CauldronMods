using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class FindAWayInCardController : InOutCardController
    {
        //==============================================================
        // At the start of a player's turn, that player may choose to skip their power Phase.
        // If they do, they may play 1 additional card during their play Phase.
        // At the start of your turn, destroy this card and each hero target regains 1HP.
        //==============================================================

        public static string Identifier = "FindAWayIn";

        public FindAWayInCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, Phase.PlayCard, Phase.UsePower)
        {

        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to skip your Power phase to play an additional card in your Play phase?", "Should they skip their Power phase to play an additional card in their Play phase", "Vote for if they should skip their Power phase to play an additional card in their Play phase?", "skip power for extra play");

        }

    }
}
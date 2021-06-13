using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class NeedAWayOutCardController : InOutCardController
    {
        //==============================================================
        // At the start of a player's turn, that player may choose to skip their play Phase.
        // If they do, they may use 1 additional power during their power Phase.
        // At the start of your turn, destroy this card and each hero target regains 1HP.
        //==============================================================

        public static string Identifier = "NeedAWayOut";

        public NeedAWayOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, Phase.UsePower, Phase.PlayCard)
        {

        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to skip your Play phase to use an additional power in your Power phase?", "Should they skip their Play phase to use an additional power in their Power phase", "Vote for if they should skip their Play phase to use an additional power in their Power phase?", "skip play for extra power");

        }

    }
}
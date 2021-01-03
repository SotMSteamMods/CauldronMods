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

        public NeedAWayOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, SelectionType.UsePowerTwice, Phase.UsePower, Phase.PlayCard)
        {

        }

    }
}
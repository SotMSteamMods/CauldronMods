using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class OutlanderCharacterCardController : CardController
    {
        public OutlanderCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Front:
        //Cards beneath this one are not considered in play. Trace cards are indestructible.
        //When {Outlander} would be destroyed instead flip his villain character cards.
        //Advanced:
        //Whenever {Outlander} flips to this side, he becomes immune to damage until the start of the next villain turn.

        //Back:
        //When {Outlander} flips to this side, restore him to 20 HP, destroy all copies of Anchored Fragment, and put a random Trace into play. Then, if there are fewer than {H} Trace cards in play, flip {Outlander}'s villain character cards.
        //Cards beneath this one are not considered in play. Trace cards are indestructible.
        //Reduce the first damage dealt to {Outlander} each turn by {H}.
        //Advanced:
        //At the end of the villain turn, destroy {H - 2} hero ongoing and/or equipment cards.
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class StrangleholdCardController : DynamoUtilityCardController
    {
        public StrangleholdCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //If Python is in play, destroy {H} hero ongoing and/or equipment cards.
        //Otherwise, search the villain deck and trash for Python and put him into play. If you searched the villain deck, shuffle it.
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class SlipperyThiefCardController : DynamoUtilityCardController
    {
        public SlipperyThiefCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //If Python is in play, he deals each hero target 1 toxic damage, regains {H} HP, and discards the top card of the villain deck.
        //The villain target with the lowest HP deals the hero target with the lowest HP {H - 2} melee damage.
    }
}

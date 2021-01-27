using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class DynamoCharacterCardController : CardController
    {
        public DynamoCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Front:
        //At the start of the villain turn, discard the top card of the villain deck and {Dynamo} deals the hero target with the highest HP {H} energy damage.
        //At the end of the villain turn, if there are at least 6 cards in the villain trash, flip {Dynamo}'s villain character card.
        //Front-Advanced:
        //Whenever a villain target enters play, discard the top card of the villain deck.

        //Back:
        //When Dynamo flips to this side, play the top 2 cards of the villain deck. Then, shuffle the villain trash, put it on the bottom of the villain deck, and flip {Dynamo}'s villain character cards.
        //Back-Advanced:
        //Increase damage dealt by villain targets by 1.
    }
}

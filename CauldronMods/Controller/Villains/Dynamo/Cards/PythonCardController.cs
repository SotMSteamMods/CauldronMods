using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class PythonCardController : DynamoUtilityCardController
    {
        public PythonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //The first time a hero target deals damage to this card each turn, reduce damage dealt by that target by 1 until the start of the next villain turn.
        //Whenever a One-shot enters the villain trash, this card deals the 2 hero targets with the lowest HP {H - 2} toxic damage each.
    }
}

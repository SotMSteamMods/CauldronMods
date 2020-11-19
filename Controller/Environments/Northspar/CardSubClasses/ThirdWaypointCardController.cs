using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class ThirdWaypointCardController : NorthsparCardController
    {

        public ThirdWaypointCardController(Card card, TurnTakerController turnTakerController, string cardToRemoveIdentifier) : base(card, turnTakerController)
        {
            this.CardToRemoveIdentifier = cardToRemoveIdentifier;
        }

        protected string CardToRemoveIdentifier { get; private set; }
    }
}
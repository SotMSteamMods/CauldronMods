using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class OnARollCardController : GyrosaurUtilityCardController
    {
        public OnARollCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, draw a card. Then if you have at least 2 Crash cards in your hand, {Gyrosaur} deals each non-hero target 1 melee damage and this card is destroyed."
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class ChaosButterflyCardController : TheMistressOfFateUtilityCardController
    {
        public ChaosButterflyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the end of the villain turn, this card deals each hero target 3 projectile damage and 3 cold damage.",
            //"When this card is destroyed, the players may swap the position of 2 face up Day cards. Cards under them move as well."
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class ResidualMalusCardController : TheMistressOfFateUtilityCardController
    {
        public ResidualMalusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"The first time a hero card is played each turn, this card deals that hero X melee damage, where X is the number of cards in the environment trash plus 1.",
            //"When this card is destroyed, destroy 1 Warped Malus."
        }
    }
}

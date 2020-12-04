using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MejiGuardCardController : OriphelUtilityCardController
    {
        public MejiGuardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to Guardians by 1.",
            //"At the end of the villain turn, this card deals the hero with the most cards in hand 2 melee damage."
        }
    }
}
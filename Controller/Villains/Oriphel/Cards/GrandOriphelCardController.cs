using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class UmbralJavelinCardController : OriphelUtilityCardController
    {
        public UmbralJavelinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of the villain turn, destroy this card unless it entered play this turn.",
            //"Increase damage dealt by {Oriphel} by 1. Reduce damage dealt to {Oriphel} by 1.",
            //"At the end of the villain turn, {Oriphel} deals each hero target 1 infernal damage and 1 projectile damage."

        }
    }
}
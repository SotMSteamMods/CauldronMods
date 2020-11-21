using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class PersonalDefenseSpinesCardController : TheRamUtilityCardController
    {
        public PersonalDefenseSpinesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{TheRam} deals each Up Close hero target {H + 2} melee damage. Destroy all ongoing and equipment cards belonging to those heroes. {TheRam} deals each other hero target 2 projectile damage."
            yield break;
        }
    }
}
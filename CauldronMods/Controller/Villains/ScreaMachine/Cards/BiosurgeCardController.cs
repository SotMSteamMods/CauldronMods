using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class BiosurgeCardController : ScreaMachineBandCardController
    {
        public BiosurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "BloodlaceCharacter", "Bass")
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {
            throw new NotImplementedException();
        }
    }
}

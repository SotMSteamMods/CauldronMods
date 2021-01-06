using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class MentalLinkCardController : ScreaMachineBandCardController
    {
        public MentalLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Valentine)
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {
            throw new NotImplementedException();
        }
    }
}

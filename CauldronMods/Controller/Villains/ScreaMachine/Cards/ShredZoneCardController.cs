﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class ShredZoneCardController : ScreaMachineBandCardController
    {
        public ShredZoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "SliceCharacter", "Guitar")
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {
            throw new NotImplementedException();
        }
    }
}

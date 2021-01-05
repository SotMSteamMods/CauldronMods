using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class TectonicBeatCardController : ScreaMachineBandCardController
    {
        public TectonicBeatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "RickyGCharacter", "Drum")
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {
            throw new NotImplementedException();
        }
    }
}

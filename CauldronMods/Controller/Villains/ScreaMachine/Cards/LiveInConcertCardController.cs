using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class LiveInConcertCardController : ScreaMachineUtilityCardController
    {
        public LiveInConcertCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG };
    }
}

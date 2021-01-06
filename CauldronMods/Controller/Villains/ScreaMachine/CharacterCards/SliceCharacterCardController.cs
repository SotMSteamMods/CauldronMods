using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class SliceCharacterCardController : ScreaMachineBandCharacterCardController
    {
        public SliceCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Slice)
        {
        }

        protected override string AbilityDescription => throw new NotImplementedException();

        protected override string UltimateFormMessage => throw new NotImplementedException();

        protected override IEnumerator ActivateBandAbility()
        {
            throw new NotImplementedException();
        }

        protected override void AddFlippedSideTriggers()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class RickyGCharacterCardController : ScreaMachineBandCharacterCardController
    {
        public RickyGCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "{Drum}", "drummer")
        {
        }

        protected override string AbilityDescription => throw new NotImplementedException();

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

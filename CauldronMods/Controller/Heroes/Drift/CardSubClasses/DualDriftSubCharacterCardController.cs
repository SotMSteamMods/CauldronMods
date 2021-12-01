using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Drift
{
    public class DualDriftSubCharacterCardController : DriftSubCharacterCardController
    {
        public DualDriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override bool shouldRunSetUp => false;

    }
}
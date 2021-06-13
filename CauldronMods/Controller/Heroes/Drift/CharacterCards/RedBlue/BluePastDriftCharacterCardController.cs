using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Drift
{
    public class BluePastDriftCharacterCardController : PastDriftCharacterCardController
    {
        public BluePastDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override bool shouldRunSetUp => false;

    }
}

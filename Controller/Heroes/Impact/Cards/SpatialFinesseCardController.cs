using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class SpatialFinesseCardController : CardController
    {
        public SpatialFinesseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, draw 2 cards and destroy all other copies of Spatial Finesse.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Whenever one of your Ongoing cards other than Spatial Finesse is destroyed, you may destroy this card to put it back into play."
        }
    }
}
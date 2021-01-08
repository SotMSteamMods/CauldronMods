using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class UnmooredZeppelinCardController : TransportCardController
    {
        public UnmooredZeppelinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateTravel()
        {
            
            yield break;
        }
    }
}

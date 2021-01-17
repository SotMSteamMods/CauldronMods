using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class VagrantHeartPhase2CardController : TheInfernalChoirUtilityCardController
    {
        public VagrantHeartPhase2CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => Card.Identifier);
        }

        public override bool CanBeDestroyed => false;
    }
}

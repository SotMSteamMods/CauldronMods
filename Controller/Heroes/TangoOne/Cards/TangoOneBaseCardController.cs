using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class TangoOneBaseCardController : CardController
    {
        public TangoOneBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsCritical(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "critical");
        }

    }
}
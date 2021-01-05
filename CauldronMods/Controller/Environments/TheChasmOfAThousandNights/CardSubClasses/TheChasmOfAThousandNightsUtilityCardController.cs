using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public abstract class TheChasmOfAThousandNightsUtilityCardController : CardController
    {
        protected TheChasmOfAThousandNightsUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}

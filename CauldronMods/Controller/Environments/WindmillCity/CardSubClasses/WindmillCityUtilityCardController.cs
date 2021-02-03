using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public abstract class WindmillCityUtilityCardController : CardController
    {
        protected WindmillCityUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string ResponderKeyword = "responder";
        public static readonly string EmergencyKeyword = "emergency";
        protected bool IsResponder(Card card)
        {
            return card.DoKeywordsContain(ResponderKeyword);
        }
        protected bool IsEmergency(Card card)
        {
            return card.DoKeywordsContain(EmergencyKeyword);
        }
    }
}

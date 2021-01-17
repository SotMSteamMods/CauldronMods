using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class EmergencyCardController : WindmillCityUtilityCardController
    {

        public EmergencyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


    }
}
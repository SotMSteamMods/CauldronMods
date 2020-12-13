using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class StrategicDeploymentCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "StrategicDeployment";

        public StrategicDeploymentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
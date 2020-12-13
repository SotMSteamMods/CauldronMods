using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class RemoteObservationCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "RemoteObservation";

        public RemoteObservationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
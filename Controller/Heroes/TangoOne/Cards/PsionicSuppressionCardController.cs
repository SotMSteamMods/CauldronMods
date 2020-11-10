using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PsionicSuppressionCardController : TangoOneBaseCardController
    {
        public static string Identifier = "PsionicSuppression";

        public PsionicSuppressionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
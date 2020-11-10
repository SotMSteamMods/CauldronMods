using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class InfiltrateCardController : TangoOneBaseCardController
    {
        public static string Identifier = "Infiltrate";

        public InfiltrateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
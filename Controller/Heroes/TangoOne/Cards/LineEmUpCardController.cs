using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class LineEmUpCardController : TangoOneBaseCardController
    {
        public static string Identifier = "LineEmUp";

        public LineEmUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
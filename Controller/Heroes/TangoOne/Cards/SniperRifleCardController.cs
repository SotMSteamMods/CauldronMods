using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class SniperRifleCardController : TangoOneBaseCardController
    {
        public static string Identifier = "SniperRifle";

        public SniperRifleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class GhostReactorCardController : TangoOneBaseCardController
    {
        public static string Identifier = "GhostReactor";

        public GhostReactorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
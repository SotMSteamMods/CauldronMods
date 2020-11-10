using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class OneShotOneKillCardController : TangoOneBaseCardController
    {
        public static string Identifier = "OneShotOneKill";

        public OneShotOneKillCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
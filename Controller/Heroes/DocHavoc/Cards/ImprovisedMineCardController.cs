using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class ImprovisedMineCardController : CardController
    {
        public static string Identifier = "ImprovisedMine";

        public ImprovisedMineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

    }
}

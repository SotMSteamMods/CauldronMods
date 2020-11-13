using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.BlackwoodForest
{
    public class OvergrownCathedralCardController : CardController
    {

        public static string Identifier = "OvergrownCathedral";

        public OvergrownCathedralCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.BlackwoodForest
{
    public class DontStrayFromThePathCardController : CardController
    {

        public static string Identifier = "DontStrayFromThePath";

        public DontStrayFromThePathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
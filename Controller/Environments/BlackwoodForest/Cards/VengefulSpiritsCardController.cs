using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.BlackwoodForest
{
    public class VengefulSpiritsCardController : CardController
    {

        public static string Identifier = "VengefulSpirits";

        public VengefulSpiritsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
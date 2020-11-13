using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.BlackwoodForest
{
    public class ShadowWeaverCardController : CardController
    {

        public static string Identifier = "ShadowWeaver";

        public ShadowWeaverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
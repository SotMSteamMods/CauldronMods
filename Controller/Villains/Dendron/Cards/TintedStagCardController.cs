using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class TintedStagCardController : CardController
    {
        public static string Identifier = "TintedStag";

        public TintedStagCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class ObsidianSkinCardController : CardController
    {
        public static string Identifier = "ObsidianSkin";

        public ObsidianSkinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
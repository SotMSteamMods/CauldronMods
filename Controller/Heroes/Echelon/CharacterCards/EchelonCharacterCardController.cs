using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron
{
    public class EchelonCharacterCardController : HeroCharacterCardController
    {

        public static string Identifier = "";

        public EchelonCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}
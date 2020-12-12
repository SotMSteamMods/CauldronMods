using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Menagerie
{
    public class MenagerieCharacterCardController : VillainCharacterCardController
    {
        public MenagerieCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
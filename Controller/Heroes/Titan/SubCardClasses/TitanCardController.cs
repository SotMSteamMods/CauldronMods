using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Titan
{
    public class TitanCardController : CardController
    {
        public TitanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public Card GetTitanform()
        {
            return FindCard("Titanform");
        }
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Titan
{
    public class TitanCardController : CardController
    {
        public TitanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria((Card c) => c == this.GetTitanform()));
        }

        public Card GetTitanform()
        {
            return FindCard("Titanform");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class StSimeonsCatacombsCardController : StSimeonsBaseCardController
	{

		public static readonly string Identifier = "StSimeonsCatacombs";

        public StSimeonsCatacombsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsUnderCard && c.Location == base.Card.UnderLocation, "under this card", false));
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
		}


		public override bool AskIfCardIsIndestructible(Card card)
		{
			return card == base.Card || card.Location == base.Card.UnderLocation;
		}
    }
}
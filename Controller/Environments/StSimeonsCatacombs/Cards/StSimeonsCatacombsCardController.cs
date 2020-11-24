using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class StSimeonsCatacombsCardController : CardController
    {

        public StSimeonsCatacombsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsUnderCard && c.Location == base.Card.UnderLocation, "under this card"));
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
		}

		public override IEnumerator Play()
		{
			//Environment cards cannot be played
			StSimeonsCatacombsInstructionsCardController instructions = FindCardController(FindCardsWhere((Card c) => c.Identifier == "StSimeonsCatacombsInstructions",realCardsOnly: false).First()) as StSimeonsCatacombsInstructionsCardController;
			IEnumerator coroutine = instructions.AddCannotPlayCardsEffect(instructions);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);

			}
			yield break;
		}

		public override bool AskIfCardIsIndestructible(Card card)
		{
			return card == base.Card || card.Location == base.Card.UnderLocation;
		}


    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Achievements;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.StSimeonsCatacombs
{
	public class StSimeonsCatacombsTurnTakerController : TurnTakerController
	{
		public StSimeonsCatacombsTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
		{
		}

		public override IEnumerator StartGame()
		{

			//Find all rooms in the deck
			List<Card> rooms = (from c in base.TurnTaker.GetAllCards(true)
								   where c.IsRoom && !c.Location.IsOutOfGame
								   select c).ToList<Card>();

			//Find the instruction card
			Card card = base.TurnTaker.FindCard("StSimeonsCatacombs");

			IEnumerator playCard = base.GameController.PlayCard(this, card);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(playCard);
			}
			else
			{
				base.GameController.ExhaustCoroutine(playCard);
			}

			CardController cardController = base.GameController.FindCardController(card);

			//move all room cards to under the instruction card
			//shuffle the cards under the instruction card
			IEnumerator coroutine = base.GameController.BulkMoveCards(this, rooms, card.UnderLocation, cardSource: cardController.GetCardSource());
			IEnumerator coroutine2 = base.GameController.ShuffleLocation(card.UnderLocation);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(coroutine2);
			}

			yield break;
		}

		


	}
}

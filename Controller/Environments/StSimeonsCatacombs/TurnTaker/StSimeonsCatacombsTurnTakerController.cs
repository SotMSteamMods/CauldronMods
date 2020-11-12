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

			//create status effect for cannot play cards
			CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
			cannotPlayCardsStatusEffect.TurnTakerCriteria.IsEnvironment = true;
			IEnumerator coroutine3 = base.GameController.AddStatusEffect(cannotPlayCardsStatusEffect, true, cardController.GetCardSource());
			this.SideStatusEffects.Add(cannotPlayCardsStatusEffect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}

			yield break;
		}

		public void RemoveSideEffects()
		{
			foreach (StatusEffect effect in this.SideStatusEffects)
			{
				base.GameController.StatusEffectManager.RemoveStatusEffect(effect);
			}
			this.SideStatusEffects.Clear();
		}

		private List<StatusEffect> SideStatusEffects = new List<StatusEffect>();


	}
}

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class RebirthCardController : CardController
    {
		public RebirthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card, null);
		}
		public override IEnumerator Play()
		{
			//When this card enters play, put up to 3 cards from your trash beneath it.
			List<MoveCardDestination> list = new List<MoveCardDestination>();
			list.Add(new MoveCardDestination(base.Card.UnderLocation, false, false, false));
			IEnumerator coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, base.TurnTaker.Trash, new int?(0), 3, new LinqCardCriteria((Card c) => true, "", true, false, null, null, false), list, false, true, false, false, null, null, false, false, false, null, false, false, null, null, base.GetCardSource(null));
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
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood destroys a target, put a card from beneath this one into your hand.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction destroy) => destroy.CardSource != null && destroy.CardToDestroy.CanBeDestroyed && destroy.WasCardDestroyed && destroy.CardSource.Card.Owner == base.TurnTaker && destroy.CardToDestroy.Card.IsTarget, new Func<DestroyCardAction, IEnumerator>(this.MoveCardResponse), new TriggerType[]
			{
				TriggerType.MoveCard
			}, TriggerTiming.After, null, false, true, null, false, null, null, false, false);

			//If this card is destroyed, move all cards under it into the trash
			base.AddBeforeLeavesPlayAction(new Func<GameAction, IEnumerator>(this.MoveCardsUnderThisCardToTrash), TriggerType.MoveCard);
		}

		private IEnumerator MoveCardsUnderThisCardToTrash(GameAction ga)
		{
			//Move all cards under this to the trash
			IEnumerator coroutine = base.GameController.MoveCards(base.TurnTakerController, base.Card.UnderLocation.Cards, base.TurnTaker.Trash, false, false, true, null, false, false, null, base.GetCardSource(null));
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

		private IEnumerator MoveCardResponse(DestroyCardAction destroyCard)
		{
			//Move card from under this card into the hand
			IEnumerator coroutine = base.GameController.SelectAndMoveCard(base.HeroTurnTakerController, (Card c) => base.Card.UnderLocation.HasCard(c), base.HeroTurnTaker.Hand, false, false, false, false, null, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//When there are no cards beneath this one, destroy this card."
			if (base.Card.UnderLocation.Cards.Count<Card>() == 0)
			{
				IEnumerator coroutine2 = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
				yield break;
			}

			yield break;
		}
	}
}

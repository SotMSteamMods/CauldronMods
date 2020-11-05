using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class RebirthCardController : CardController
    {
		public RebirthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card, null);
			this._primed = false;
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
			//this card has cards under it, so mark as primed so that if any future actions result in 0 cards under this one, it is destroyed
			this._primed = true;
			yield break;
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood destroys a target, put a card from beneath this one into your hand.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction destroy) => destroy.CardSource != null && destroy.CardToDestroy.CanBeDestroyed && destroy.WasCardDestroyed && destroy.CardSource.Card.Owner == base.TurnTaker && destroy.CardToDestroy.Card.IsTarget, new Func<DestroyCardAction, IEnumerator>(this.MoveCardResponse), new TriggerType[]
			{
				TriggerType.MoveCard
			}, TriggerTiming.After, null, false, true, null, false, null, null, false, false);

			//When there are no cards beneath this one, destroy this card.
			base.AddTrigger<GameAction>((GameAction action) => this._primed && base.Card.UnderLocation.Cards.Count<Card>() == 0, new Func<GameAction, IEnumerator>(this.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);

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

			yield break;
		}

		private bool _primed;
	}
}

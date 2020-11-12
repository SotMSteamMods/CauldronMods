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
			base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
			this._primed = false;
		}
		public override IEnumerator Play()
		{
			
			//When this card enters play, put up to 3 cards from your trash beneath it.
			List<MoveCardDestination> list = new List<MoveCardDestination>();
			list.Add(new MoveCardDestination(base.Card.UnderLocation));
			IEnumerator coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, base.TurnTaker.Trash, new int?(0), 3, new LinqCardCriteria((Card c) => true), list, cardSource: base.GetCardSource());
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
			Func<DestroyCardAction, bool> moveCriteria = (DestroyCardAction destroy) => destroy.CardSource != null && destroy.CardToDestroy.CanBeDestroyed && destroy.WasCardDestroyed && destroy.CardSource.Card.Owner == base.TurnTaker && destroy.CardToDestroy.Card.IsTarget;
			base.AddTrigger<DestroyCardAction>(moveCriteria, new Func<DestroyCardAction, IEnumerator>(this.MoveCardResponse), new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After);

			//When there are no cards beneath this one, destroy this card.
			Func<GameAction, bool> destroyCriteria = (GameAction action) => this._primed && base.Card.UnderLocation.Cards.Count<Card>() == 0;
			base.AddTrigger<GameAction>(destroyCriteria, new Func<GameAction, IEnumerator>(this.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After);

			//If this card is destroyed, move all cards under it into the trash
			base.AddBeforeLeavesPlayAction(new Func<GameAction, IEnumerator>(this.MoveCardsUnderThisCardToTrash), TriggerType.MoveCard);
		}

		private IEnumerator MoveCardsUnderThisCardToTrash(GameAction ga)
		{
			//Move all cards under this to the trash
			IEnumerator coroutine = base.GameController.MoveCards(base.TurnTakerController, base.Card.UnderLocation.Cards, base.TurnTaker.Trash, cardSource: base.GetCardSource());
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
			IEnumerator coroutine = base.GameController.SelectAndMoveCard(base.HeroTurnTakerController, (Card c) => base.Card.UnderLocation.HasCard(c), base.HeroTurnTaker.Hand, cardSource: base.GetCardSource());
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

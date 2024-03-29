﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class LadyOfTheWoodsRebirthCardController : CardController
    {
		public LadyOfTheWoodsRebirthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
			this._primed = true;
		}

		public override bool ShouldBeDestroyedNow()
		{
			return this.Card.IsInPlayAndHasGameText && _primed && this.Card.UnderLocation.IsEmpty;
		}

		public override void AddStartOfGameTriggers()
        {
			this._primed = true;
			base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cpa) => cpa.CardEnteringPlay == base.Card, this.MarkNotPrimed, TriggerType.Hidden, TriggerTiming.Before);
        }

        private IEnumerator MarkNotPrimed(CardEntersPlayAction cpa)
        {
			this._primed = false;
			yield return null;
			yield break;
        }
		private bool IsPotentialEmptierAction(GameAction ga)
		{
			if (ga is PlayCardAction pc)
			{
				return pc.Origin == this.Card.UnderLocation;
			}
			if (ga is MoveCardAction mc)
			{
				return mc.Origin == this.Card.UnderLocation;
			}
			if (ga is BulkMoveCardsAction bmc)
			{
				return bmc.CardsToMove.Any((Card c) => bmc.FindOriginForCard(c) == this.Card.UnderLocation);
			}
			if (ga is CompletedCardPlayAction ccp)
			{
				return ccp.CardPlayed == this.Card;
			}
			return false;
		}

		public override IEnumerator Play()
		{
			//When this card enters play, put up to 3 cards from your trash beneath it.
			List<MoveCardDestination> list = new List<MoveCardDestination>();
			list.Add(new MoveCardDestination(base.Card.UnderLocation));
			IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.Trash, 0, 3, new LinqCardCriteria((Card c) => true), list,
									playIfMovingToPlayArea: false,
									allowAutoDecide: true,
									selectionType: SelectionType.MoveCardToUnderCard,
									cardSource: GetCardSource());
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
			yield return null;
			yield break;
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood destroys a target, put a card from beneath this one into your hand.
			Func<DestroyCardAction, bool> moveCriteria = (DestroyCardAction dca) => dca.GetCardDestroyer()?.Owner == base.TurnTaker && dca.CardToDestroy.CanBeDestroyed && dca.WasCardDestroyed  && dca.CardToDestroy.Card.IsTarget;
			base.AddTrigger<DestroyCardAction>(moveCriteria, new Func<DestroyCardAction, IEnumerator>(this.MoveCardResponse), new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After);

			//When there are no cards beneath this one, destroy this card.
			Func<GameAction, bool> destroyCriteria = (GameAction action) => this._primed && base.Card.UnderLocation.Cards.Count<Card>() == 0 && IsPotentialEmptierAction(action);
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

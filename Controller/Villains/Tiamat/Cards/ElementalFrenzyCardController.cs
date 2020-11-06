using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class ElementalFrenzyCardController : CardController
    {
        #region Constructors

        public ElementalFrenzyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card, null);
			this._primed = false;
		}

		#endregion Constructors

		#region Properties

		private bool _primed;

		#endregion Properties

		#region Methods


		public override IEnumerator Play()
		{
			//When this card enters play, shuffle all Spell cards from the villain trash and place them beneath this card face down.
			IEnumerable<Card> spellsInTrash = FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("spell") && c.IsInTrash && c.IsVillain), null, false);
			IEnumerator coroutine = base.GameController.BulkMoveCards(this.TurnTakerController, spellsInTrash, base.Card.UnderLocation, false, true, null, false, null);
			IEnumerator coroutine2 = base.GameController.ShuffleLocation(base.Card.UnderLocation, null, null);
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
			//this card has cards under it, so mark as primed so that if any future actions result in 0 cards under this one, it is destroyed
			this._primed = true;
			yield break;
		}

        public override void AddTriggers()
		{
			//At the end of the villain turn play the top card from this pile. 
			base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, (PhaseChangeAction p) => base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, false, 1, false, null, null, null, false, null, false, false, true, null, base.Card.UnderLocation, base.GetCardSource(null)), TriggerType.PlayCard, null, false);
			//When this pile is depleted, destroy this card.
			base.AddTrigger<GameAction>((GameAction action) => this._primed && base.Card.UnderLocation.Cards.Count<Card>() == 0, new Func<GameAction, IEnumerator>(this.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
			//If this card is destroyed, move all cards under it into the trash
			base.AddBeforeLeavesPlayAction(new Func<GameAction, IEnumerator>(this.MoveCardsUnderThisCardToTrash), TriggerType.MoveCard);
			//When this card is destroyed, play the top card of the villain deck.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction dc) => !base.GameController.IsGameOver && dc.CardToDestroy.Card == base.Card, new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), new TriggerType[] { TriggerType.PlayCard }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);

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
		private IEnumerator OnDestroyResponse(DestroyCardAction dc)
		{
			base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, false, 1, false, null, null, null, false, null, false, false, false, null, null, base.GetCardSource(null));
			yield break;
		}

		#endregion Methods
	}
}
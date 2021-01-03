using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementalFrenzyCardController : CardController
    {
        public ElementalFrenzyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
            this._primed = true;
        }

        private bool _primed;

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
            return ga is PlayCardAction || ga is DiscardCardAction || ga is MoveCardAction || ga is DestroyCardAction || ga is BulkMoveCardsAction || ga is CompletedCardPlayAction;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, shuffle all Spell cards from the villain trash and place them beneath this card face down.
            LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.DoKeywordsContain("spell") && base.TurnTaker.Trash.Cards.Contains(c));
            IEnumerable<Card> spellsInTrash = FindCardsWhere(criteria);
            IEnumerator coroutine = base.GameController.BulkMoveCards(this.TurnTakerController, spellsInTrash, base.Card.UnderLocation);
            IEnumerator coroutine2 = base.GameController.ShuffleLocation(base.Card.UnderLocation);
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
            IEnumerator playTopCard = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, showMessage: true, overrideDeck: base.Card.UnderLocation, cardSource: base.GetCardSource());
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, (PhaseChangeAction p) => playTopCard, TriggerType.PlayCard);
            //When this pile is depleted, destroy this card.
            base.AddTrigger<GameAction>((GameAction action) => this._primed && base.Card.UnderLocation.Cards.Count() == 0 && IsPotentialEmptierAction(action), new Func<GameAction, IEnumerator>(this.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After);
            //If this card is destroyed, move all cards under it into the trash
            base.AddBeforeLeavesPlayAction(new Func<GameAction, IEnumerator>(this.MoveCardsUnderThisCardToTrash), TriggerType.MoveCard);
            //When this card is destroyed, play the top card of the villain deck.
            base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), TriggerType.PlayCard);
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

        private IEnumerator OnDestroyResponse(DestroyCardAction dc)
        {
            //play the top card of the villain deck
            IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
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
    }
}
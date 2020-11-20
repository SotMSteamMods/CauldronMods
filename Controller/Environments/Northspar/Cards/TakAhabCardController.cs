using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class TakAhabCardController : NorthsparCardController
    {

        public TakAhabCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            
        }

        public override void AddTriggers()
        {
            //Whenever this card has no cards beneath it, place the top card of each hero deck beneath this one.
            Func<GameAction, bool> criteria = (GameAction ga) => base.Card.UnderLocation.NumberOfCards == 0 && !this._movingCards;
            base.AddTrigger<GameAction>(criteria, (GameAction ga) =>  base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => ttc.IsHero && !ttc.IsIncapacitatedOrOutOfGame && ttc.TurnTaker.Deck.HasCards, 
                (TurnTakerController ttc) => this.MoveCardsUnderResponse(ttc)),
                TriggerType.MoveCard, TriggerTiming.After);
        }

        private IEnumerator MoveCardsUnderResponse(TurnTakerController ttc)
        {
            this._movingCards = true;
            //place the top card of each hero deck beneath this one.
            IEnumerator coroutine = base.GameController.MoveCard(base.DecisionMaker, ttc.TurnTaker.Deck.TopCard, base.Card.UnderLocation, showMessage: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            this._movingCards = false;
            yield break;
        }

        private bool _movingCards = false;

    }
}
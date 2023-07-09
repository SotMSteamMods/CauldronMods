using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class TemporalResetCardController : CardController
    {
        public TemporalResetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, destroy all other environment cards. 
            IEnumerator coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment && c != base.Card && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment"), autoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then shuffle 2 cards from each trash pile back into their deck...
            coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()), MoveCardToDeckResponse);
            //...and each non-character target regains {H} HP.
            IEnumerator coroutine2 = base.GameController.GainHP(this.DecisionMaker, (Card c) => !c.IsCharacter && c.IsTarget && c.IsInPlayAndHasGameText, base.Game.H, cardSource: base.GetCardSource());
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

        private IEnumerator MoveCardToDeckResponse(TurnTakerController turnTakerController)
        {
            TurnTaker turnTaker = turnTakerController.TurnTaker;
            List<MoveCardDestination> list = new List<MoveCardDestination>
            {
                new MoveCardDestination(turnTaker.Deck)
            };
            HeroTurnTakerController decisionMaker;
            if (IsHero(turnTaker))
            {
                decisionMaker = turnTakerController.ToHero();
            }
            else
            {
                decisionMaker = this.DecisionMaker;
            }
            IEnumerator coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(decisionMaker, turnTaker.Trash, 2, 2, new LinqCardCriteria((Card c) => c.Location == turnTaker.Trash, "trash"), list, cardSource: base.GetCardSource());
            IEnumerator coroutine2 = base.ShuffleDeck(this.DecisionMaker, turnTaker.Deck);
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

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

    }
}
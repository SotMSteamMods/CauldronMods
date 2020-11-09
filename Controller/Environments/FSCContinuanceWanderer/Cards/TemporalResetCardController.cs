using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class TemporalResetCardController : CardController
    {
        #region Constructors

        public TemporalResetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //When this card enters play, destroy all other environment cards. 
            IEnumerator coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment && c != base.Card), true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then shuffle 2 cards from each trash pile back into their deck...
            coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController turnTakerController) => true, MoveCardToDeckResponse);
            //...and each non-character target regains {H} HP.
            IEnumerator coroutine2 = base.GameController.GainHP(this.DecisionMaker, (Card c) => !c.IsCharacter, base.Game.H);
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
            IEnumerator coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(turnTakerController.ToHero(), turnTaker.Trash, new int?(0), 2, new LinqCardCriteria((Card c) => c.Location == turnTaker.Trash, "trash"), list, shuffleAfterwards: true, cardSource: base.GetCardSource());
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
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        #endregion Methods
    }
}
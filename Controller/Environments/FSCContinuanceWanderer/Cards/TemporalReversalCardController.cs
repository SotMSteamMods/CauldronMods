using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class TemporalReversalCardController : CardController
    {
        #region Constructors

        public TemporalReversalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties

        private List<Card> actedHeroes;

        #endregion Properties

        #region Methods

        public override IEnumerator Play()
        {
            this.actedHeroes = new List<Card>();
            //When this card enters play, place 1 card in play from each other deck back on top of that deck.
            IEnumerator coroutine = base.GameController.SelectTurnTakersAndDoAction(null, new LinqTurnTakerCriteria((TurnTaker turnTaker) => !turnTaker.IsEnvironment && !turnTaker.IsIncapacitatedOrOutOfGame), SelectionType.ReturnToDeck, (TurnTaker turnTaker) => this.MoveCardToDeckResponse(turnTaker), cardSource: base.GetCardSource());
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

        private IEnumerator MoveCardToDeckResponse(TurnTaker turnTaker)
        {
            if (turnTaker.IsHero)
            {
                IEnumerator coroutine = base.GameController.SelectAndReturnCards(base.FindHeroTurnTakerController(turnTaker.ToHero()), new int?(1), new LinqCardCriteria((Card c) => !c.IsCharacter && c.IsInPlay && c.Owner == turnTaker, "card in play"), false, true, false, new int?(1), cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = base.GameController.SelectAndMoveCard(this.DecisionMaker, (Card c) => c.Owner == turnTaker && c.IsInPlay && !c.IsCharacter, turnTaker.Deck, cardSource: base.GetCardSource());
                IEnumerator coroutine2 = base.GameController.ShuffleLocation(turnTaker.Deck);
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
            }
            yield break;
        }
        private void LogActedCard(Card card)
        {
            if (card.SharedIdentifier != null)
            {
                IEnumerable<Card> collection = base.FindCardsWhere((Card c) => c.SharedIdentifier != null && c.SharedIdentifier == card.SharedIdentifier && c != card, false, null, false);
                this.actedHeroes.AddRange(collection);
            }
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        #endregion Methods
    }
}
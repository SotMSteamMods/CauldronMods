using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class HeartOfTheWandererCardController : CardController
    {
        #region Constructors

        public HeartOfTheWandererCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            IEnumerator coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController) => true, new Func<TurnTakerController, IEnumerator>(this.EachTurnTakerResponse), base.TurnTaker);
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

        private IEnumerator EachTurnTakerResponse(TurnTakerController turnTakerController)
        {
            //...reveal the top card of each deck in turn order and either discard it or replace it.
            List<Card> revealedCards = new List<Card>();
            TurnTaker turnTaker = turnTakerController.TurnTaker;
            IEnumerator coroutine = base.GameController.RevealCards(FindTurnTakerController(turnTaker), turnTaker.Deck, 1, revealedCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card revealedCard = revealedCards.FirstOrDefault<Card>();
            if (revealedCard != null)
            {
                coroutine = base.GameController.SelectLocationAndMoveCard(this.DecisionMaker, revealedCard, new MoveCardDestination[]
                {
                        new MoveCardDestination(turnTaker.Deck),
                        new MoveCardDestination(turnTaker.Trash)
                }, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        #endregion Methods
    }
}
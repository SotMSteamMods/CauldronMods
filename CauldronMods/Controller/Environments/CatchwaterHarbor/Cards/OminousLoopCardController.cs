using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class OminousLoopCardController : CatchwaterHarborUtilityCardController
    {
        public OminousLoopCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeOneshot);
        }

        private static readonly string FirstTimeOneshot = "FirstTimeOneShot";

        public override void AddTriggers()
        {
            //The first time a One-shot card enters the trash each turn, move it beneath this card. Cards beneath this one are not considered in play
            AddTrigger((MoveCardAction mca) => !HasBeenSetToTrueThisTurn(FirstTimeOneshot) && mca.CardToMove != null && mca.CardToMove.IsOneShot && mca.Destination != null && mca.Destination.IsTrash, MoveCardUnderResponse, TriggerType.MoveCard, TriggerTiming.After);
            //At the start of the environment turn, destroy this card and put all cards beneath it into play in the order they were placed there
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyCardResponse, new TriggerType[]
            {
                TriggerType.DestroySelf,
                TriggerType.PutIntoPlay
            });     
        }

        private IEnumerator DestroyCardResponse(PhaseChangeAction arg)
        {
            //destroy this card and put all cards beneath it into play in the order they were placed there
            IEnumerator coroutine;
            foreach(Card under in Card.UnderLocation.Cards)
            {
                coroutine = GameController.FlipCard(FindCardController(under), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                
            }
            coroutine = GameController.MoveCards(TurnTakerController,Card.UnderLocation.Cards, TurnTaker.Revealed, toBottom: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DestroyCard(DecisionMaker, Card, postDestroyAction: PutRevealedIntoPlayResponse, cardSource: GetCardSource());
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

        private IEnumerator PutRevealedIntoPlayResponse()
        {
            IEnumerator coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Revealed, numberOfCards: TurnTaker.Revealed.NumberOfCards, isPutIntoPlay: true, cardSource: GetCardSource());
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

        private IEnumerator MoveCardUnderResponse(MoveCardAction mca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeOneshot);
            //move it beneath this card
            IEnumerator coroutine = GameController.MoveCard(TurnTakerController, mca.CardToMove, Card.UnderLocation, playCardIfMovingToPlayArea: false, flipFaceDown: true, doesNotEnterPlay: true, cardSource: GetCardSource());
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

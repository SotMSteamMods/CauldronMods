using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class SpatialFinesseCardController : CardController
    {
        public SpatialFinesseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, draw 2 cards and destroy all other copies of Spatial Finesse.",
            IEnumerator coroutine = DrawCards(DecisionMaker, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "SpatialFinesse" && c != this.Card), autoDecide: true, cardSource: GetCardSource());
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
            //"Whenever one of your Ongoing cards other than Spatial Finesse is destroyed, you may destroy this card to put it back into play."
            AddTrigger<DestroyCardAction>(DestroyCardCriteria, MayReturnDestroyedResponse, new TriggerType[] { TriggerType.ChangePostDestroyDestination, TriggerType.PutIntoPlay }, TriggerTiming.After, isActionOptional: true);
        }

        private bool DestroyCardCriteria(DestroyCardAction dca)
        {
            var card = dca.CardToDestroy.Card;
            return !this.IsBeingDestroyed && dca.PostDestroyDestinationCanBeChanged && dca.WasCardDestroyed && card.Owner == this.TurnTaker && IsOngoing(card) && card.Identifier != "SpatialFinesse";
        }

        private IEnumerator MayReturnDestroyedResponse(DestroyCardAction dca)
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            List<Card> cardToSave = new List<Card> { dca.CardToDestroy.Card };
            IEnumerator coroutine = base.GameController.DestroyCard(DecisionMaker, this.Card, optional: true, storedResults, associatedCards: cardToSave, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDestroyCard(storedResults))
            {
                dca.PostDestroyDestinationCanBeChanged = false;
                dca.AddAfterDestroyedAction(() => GameController.PlayCard(TurnTakerController, dca.CardToDestroy.Card, isPutIntoPlay: true, optional: false, evenIfAlreadyInPlay: true, cardSource: GetCardSource()), this);
            }
        }
    }
}
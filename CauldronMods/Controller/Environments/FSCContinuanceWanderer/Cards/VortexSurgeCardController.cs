using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.FSCContinuanceWanderer
{
    public class VortexSurgeCardController : CardController
    {

        public VortexSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever a hero card is drawn, 1 player must discard a card.
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => action.IsSuccessful && action.DidDrawCard && IsHero(action.DrawnCard) && GameController.IsCardVisibleToCardSource(action.DrawnCard, GetCardSource()), this.DiscardCardResponse, new TriggerType[] { TriggerType.DiscardCard }, TriggerTiming.After);
            //When another environment card enters play, destroy this card.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction p) => p.CardEnteringPlay.IsEnvironment && p.CardEnteringPlay.Identifier != base.Card.Identifier && GameController.IsCardVisibleToCardSource(p.CardEnteringPlay, GetCardSource()), base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DiscardCardResponse(DrawCardAction action)
        {
            IEnumerator coroutine = base.GameController.SelectHeroToDiscardCard(this.DecisionMaker, optionalDiscardCard: false, cardSource: base.GetCardSource());
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
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.FSCContinuanceWanderer
{
    public class VortexSurgeCardController : CardController
    {
        #region Constructors

        public VortexSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Whenever a hero card is drawn, 1 player must discard a card.
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => action.IsSuccessful && action.DidDrawCard && action.DrawnCard.IsHero, this.DiscardCardResponse, new TriggerType[] { TriggerType.DiscardCard }, TriggerTiming.After);
            //When another environment card enters play, destroy this card.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction p) => p.CardEnteringPlay.IsEnvironment, (base.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DiscardCardResponse(DrawCardAction action)
        {
            IEnumerator coroutine = base.SelectAndDiscardCards(this.DecisionMaker, new int?(1));
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

        #endregion Methods
    }
}
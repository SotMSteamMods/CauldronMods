using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class ResponderCardController : WindmillCityUtilityCardController
    {

        public ResponderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, 1 hero target regains 2HP.

            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, StartOfTurnResponse, TriggerType.DestroyCard);
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //the players may destroy 1 Emergency card. 

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => IsEmergency(c) && c.IsInPlayAndHasGameText, "emergency"), true, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If a card is destroyed this way, DoOnDestroyAction
            if(DidDestroyCard(storedResults))
            {
                coroutine = PerformActionOnDestroy();
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

        protected virtual IEnumerator PerformActionOnDestroy() { return null; }
    }
}
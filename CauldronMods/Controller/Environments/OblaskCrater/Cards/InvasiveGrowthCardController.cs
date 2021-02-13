using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class InvasiveGrowthCardController : OblaskCraterUtilityCardController
    {
        /* 
         * When this card enters play, discard the top 2 cards of the enviroment deck, then put a random target 
         * from the environment trash into play.
         * When an environment target is destroyed, all other environment cards become indestructible until the 
         * start of the next turn.
         */
        public InvasiveGrowthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>(DestroyCardActionCriteria, DestroyCardActionResponse, TriggerType.DestroyCard, TriggerTiming.After);
        }

        private bool DestroyCardActionCriteria(DestroyCardAction destroyCardAction)
        {
            return destroyCardAction.CardToDestroy != null && destroyCardAction.WasCardDestroyed && destroyCardAction.CardToDestroy.Card.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(destroyCardAction.CardToDestroy.Card, GetCardSource());
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            /* 
             * When an environment target is destroyed, all other environment cards become indestructible until the 
             * start of the next turn.
             */
            IEnumerator coroutine;
            MakeIndestructibleStatusEffect makeIndestructibleStatusEffect;

            makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
            makeIndestructibleStatusEffect.CardsToMakeIndestructible.IsEnvironment = true;
            makeIndestructibleStatusEffect.UntilStartOfNextTurn(base.FindNextTurnTaker());

            coroutine = base.AddStatusEffect(makeIndestructibleStatusEffect);
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

        public override IEnumerator Play()
        {
            /* 
             * When this card enters play, discard the top 2 cards of the enviroment deck, then put a random target 
             * from the environment trash into play.
             */
            IEnumerator coroutine;
            Location environmentTrash = FindLocationsWhere(location => location.IsRealTrash && location.IsEnvironment && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();
            TurnTakerController environmentTurnTakerController = FindEnvironment();

            coroutine = base.DiscardCardsFromTopOfDeck(environmentTurnTakerController, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            string message = $"{Card.Title} plays a random target from {environmentTrash.GetFriendlyName()}.";
           
            coroutine = base.GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
         
            coroutine = ReviveCardFromTrash(base.TurnTakerController, (Card c) => c.IsTarget);
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

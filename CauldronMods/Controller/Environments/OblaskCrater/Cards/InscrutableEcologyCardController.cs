using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class InscrutableEcologyCardController : OblaskCraterUtilityCardController
    {
        /* 
         * When this card enters play, reveal cards from the top of the environmnet deck until {H - 1} targets are 
         * revealed, put them into play, and discard the other revealed cards.
         * Reduce damage dealt by environment targets by 1. At the start of the environment turn, destroy this card.
         */
        public InscrutableEcologyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);

            // Reduce damage dealt by environment targets by 1.            
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsEnvironmentTarget, (DealDamageAction dd) => 1);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, numberOfCardToReveal: null, cardCriteria: new LinqCardCriteria(c => c.IsTarget), revealUntilNumberOfMatchingCards: Game.H - 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
     
        }
    }
}

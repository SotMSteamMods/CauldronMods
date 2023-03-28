using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class GameTrailCardController : OblaskCraterUtilityCardController
    {
        /*
         * When this card enters play, play the top card of the environment deck.
         * Whenever a predator card destroys another target, that predator deals each hero {H - 2} melee damage.
        */
        public GameTrailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>((dca) => dca.DealDamageAction != null && IsPredator(dca.DealDamageAction.DamageSource.Card), DestroyCardActionResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            IEnumerator coroutine;

            // Whenever a predator card destroys another target, that predator deals each hero {H - 2} melee damage.
            coroutine = base.DealDamage(destroyCardAction.DealDamageAction.DamageSource.Card, (card) => IsHeroCharacterCard(card), base.H - 2, DamageType.Melee);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // When this card enters play, play the top card of the environment deck.
            coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}

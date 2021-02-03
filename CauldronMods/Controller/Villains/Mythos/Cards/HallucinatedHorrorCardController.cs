using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class HallucinatedHorrorCardController : MythosUtilityCardController
    {
        public HallucinatedHorrorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{MythosMadness}{MythosDanger} When this card enters play, play the top card of the villain deck.
            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier) || base.IsTopCardMatching(MythosDangerDeckIdentifier))
            {
                IEnumerator coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals each hero target 2 sonic damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
            //Destroy this card when a hero is dealt damage by another hero target.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target.IsHero && action.DamageSource.IsHero && action.Target != action.DamageSource.Card, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals each hero target 2 sonic damage.
            IEnumerator coroutine = base.DealDamage(this.Card, (Card c) => c.IsHero, 2, DamageType.Sonic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

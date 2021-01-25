using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class ArchangelCardController : OutlanderUtilityCardController
    {
        public ArchangelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected const string OncePerTurn = "OncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} is dealt 4 or more damage from a single source each turn, play the top card of the villain deck.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Amount >= 4 && action.Target == base.CharacterCard, base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals each non-villain target irreducible 1 projectile damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals each non-villain target irreducible 1 projectile damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => !base.IsVillain(c) && c.IsTarget, 1, DamageType.Projectile);
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

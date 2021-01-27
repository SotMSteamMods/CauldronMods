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
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has been dealt 4 or more damage from a single source this turn.", () => "Outlander has not been dealt 4 or more damage from a single source this turn.");
        }

        protected const string OncePerTurn = "OncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} is dealt 4 or more damage from a single source each turn, play the top card of the villain deck.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn) && action.Amount >= 4 && action.Target == base.CharacterCard, this.OncePerTurnResponse, TriggerType.PlayCard, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals each non-villain target irreducible 1 projectile damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...play the top card of the villain deck.
            IEnumerator coroutine = base.PlayTheTopCardOfTheVillainDeckResponse(action);
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

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals each non-villain target irreducible 1 projectile damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => !base.IsVillain(c) && c.IsTarget, 1, DamageType.Projectile, true);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class ArchangelCardController : OutlanderTraceCardController
    {
        public ArchangelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has been dealt 4 or more damage from a single source this turn.", () => "Outlander has not been dealt 4 or more damage from a single source this turn.");
        }

        protected const string OncePerTurn = "OutlanderArchangelOncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} is dealt 4 or more damage from a single source each turn, play the top card of the villain deck.
            AddTrigger<DealDamageAction>((DealDamageAction action) => !HasBeenSetToTrueThisTurn(OncePerTurn) && action.Amount >= 4 && action.Target == CharacterCard, OncePerTurnResponse, TriggerType.PlayCard, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals each non-villain target irreducible 1 projectile damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(DealDamageAction action)
        {
            SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...play the top card of the villain deck.
            IEnumerator coroutine = PlayTheTopCardOfTheVillainDeckWithMessageResponse(action);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals each non-villain target irreducible 1 projectile damage.
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsVillainTarget(c), 1, DamageType.Projectile, isIrreducible: true);
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

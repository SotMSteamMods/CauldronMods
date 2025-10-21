using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DragonbornCardController : OutlanderTraceCardController
    {
        public DragonbornCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has been dealt damage this turn.", () => "Outlander has not been dealt this turn.");
        }

        protected const string OncePerTurn = "OutlanderDragonBornOncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} is dealt damage each turn, he deals the source of that damage 2 fire damage.
            AddTrigger<DealDamageAction>((DealDamageAction action) => !HasBeenSetToTrueThisTurn(OncePerTurn) && action.Target == CharacterCard && action.DidDealDamage, OncePerTurnResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals each non-villain target 1 fire damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(DealDamageAction action)
        {
            SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...he deals the source of that damage 2 fire damage.
            IEnumerator coroutine = DealDamage(CharacterCard, action.DamageSource.Card, 2, DamageType.Fire, isCounterDamage: true, cardSource: GetCardSource());
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
            //...{Outlander} deals each non-villain target 1 fire damage.
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsVillainTarget(c), 1, DamageType.Fire);
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

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DragonbornCardController : OutlanderUtilityCardController
    {
        public DragonbornCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has been dealt damage this turn.", () => "Outlander has not been dealt this turn.");
        }

        protected const string OncePerTurn = "OncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} is dealt damage each turn, he deals the source of that damage 2 fire damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn) && action.Target == base.CharacterCard && action.DidDealDamage, this.OncePerTurnResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals each non-villain target 1 fire damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...he deals the source of that damage 2 fire damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, action.DamageSource.Card, 2, DamageType.Fire, cardSource: base.GetCardSource());
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
            //...{Outlander} deals each non-villain target 1 fire damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => !base.IsVillain(c) && c.IsTarget, 1, DamageType.Fire);
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

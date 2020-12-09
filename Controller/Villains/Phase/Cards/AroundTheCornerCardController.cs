﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Phase
{
    public class AroundTheCornerCardController : PhaseCardController
    {
        public AroundTheCornerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private const string FirstTimeDamageDealt = "FirstTimeDestroyed";

        public override void AddTriggers()
        {
            //The first time an Obstacle is destroyed each turn, {Phase} deals each hero target 2 radiant damage.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => !base.HasBeenSetToTrueThisTurn("FirstTimeDestroyed") && base.IsObstacle(action.CardToDestroy.Card) && action.WasCardDestroyed, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction action)
        {
            //The first time an Obstacle is destroyed each turn, {Phase} deals each hero target 2 radiant damage.
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDestroyed");
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero, 2, DamageType.Radiant);
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
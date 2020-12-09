﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class DistortionGrenadeCardController : CardController
    {
        public DistortionGrenadeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt to non-villain targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => !action.Target.IsVillain, 1);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals the {H}-1 hero targets with the lowest HP 2 lightning damage each.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => 2, DamageType.Lightning, numberOfTargets: Game.H - 1);
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
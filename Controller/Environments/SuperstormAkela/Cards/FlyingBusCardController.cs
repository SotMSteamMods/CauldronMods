﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class FlyingBusCardController : SuperstormAkelaCardController
    {

        public FlyingBusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.PlayArea, new LinqCardCriteria((Card c) => IsLeftOfThisCard(c, base.Card), "card(s) left of this"));

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the X+1 hero targets with the highest HP {H} projectile damage each, where X is the number of environment cards to the left of this one
             AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            // this card deals the X+1 hero targets with the highest HP {H} projectile damage each, where X is the number of environment cards to the left of this one

            Func<int> numTargets = () => GetNumberOfCardsToTheLeftOfThisOne(base.Card).Value + 1;
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => new int?(Game.H), DamageType.Projectile, numberOfTargets: numTargets);
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
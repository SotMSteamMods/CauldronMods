﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class LostInTheSnowCardController : NorthsparCardController
    {

        public LostInTheSnowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 1 cold damage.
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero && c.IsTarget, TargetType.All, 1, DamageType.Cold);

            //At the start of the environment turn, each hero must destroy 1 ongoing and 1 equipment card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyOngoingOrEquipmentResponse, TriggerType.DestroyCard);
        }

        private IEnumerator DestroyOngoingOrEquipmentResponse(PhaseChangeAction pca)
        {
            //each hero must destroy 1 ongoing and 1 equipment card.
            IEnumerator ongoing = EachPlayerDestroysTheirCards(new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, "heroes with ongoing or equipment cards in play"), new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing"));
            IEnumerator equipment = EachPlayerDestroysTheirCards(new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, "heroes with ongoing or equipment cards in play"), new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(ongoing);
                yield return base.GameController.StartCoroutine(equipment);

            }
            else
            {
                base.GameController.ExhaustCoroutine(ongoing);
                base.GameController.ExhaustCoroutine(equipment);

            }
        }
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class LeftBehindCardController : CatchwaterHarborUtilityCardController
    {
        public LeftBehindCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP(ranking: 2);
        }

        private string LostEndingResultText
        {
            get
            {
                return $"{GetCardThisCardIsNextTo().Title} is stuck in the past! The heroes lose! [b]Game over.[/b]";
            }
        }
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<Card> foundTarget = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(2, (Card c) => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (overridePlayArea == null || c.IsAtLocationRecursive(overridePlayArea)), foundTarget, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card secondHighest = foundTarget.FirstOrDefault();
            if (secondHighest != null && storedResults != null)
            {
                //Play this card next to the hero with the second lowest HP.
                storedResults.Add(new MoveCardDestination(secondHighest.NextToLocation));
            }
            yield break;
        }

        public override void AddTriggers()
        {
            // If the last villain character target is reduced to 0HP by a target other than that hero they are stuck in the past and the heroes lose. [b]Game over.[/b]"

            AddTrigger((DestroyCardAction dca) => DestroyCardGameOverCriteria(dca), this.GameOverResponse, TriggerType.GameOver, TriggerTiming.Before);

            //if that hero is incapacitated, 
            AddTrigger((TargetLeavesPlayAction tlp) => GetCardThisCardIsNextTo() != null && tlp.TargetLeavingPlay == GetCardThisCardIsNextTo(), GameOverResponse, TriggerType.GameOver, TriggerTiming.After);
        }

        private bool DestroyCardGameOverCriteria(DestroyCardAction dca)
        {
            return !GameController.IsGameOver && dca.CardToDestroy != null && dca.CardToDestroy.Card != null && !FindCardsWhere(c => IsVillain(c) && c.IsCharacter && c != dca.CardToDestroy.Card && !c.IsIncapacitatedOrOutOfGame && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Any() && dca.DealDamageAction != null && dca.DealDamageAction is DealDamageAction dd && dd.TargetHitPointsAfterBeingDealtDamage <= 0 && dd.DamageSource != null && GetCardThisCardIsNextTo() != null && dd.DamageSource.Card != GetCardThisCardIsNextTo();
        }

        private IEnumerator GameOverResponse(GameAction ga)
        {
            IEnumerator coroutine = GameController.GameOver(EndingResult.EnvironmentDefeat, LostEndingResultText, showEndingTextAsMessage: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

    }
}

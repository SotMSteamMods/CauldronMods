﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class PainkillersCardController : CardController
    {
        //==============================================================
        // When this card enters play, place it next to a hero target.
        // Damage dealt by that target is irreducible. At the start of that hero's
        // turn, that target may deal itself 2 toxic damage. If no damage is taken this way, this card is destroyed.
        //==============================================================

        public static string Identifier = "Painkillers";

        private const int DamageAmount = 2;

        public PainkillersCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay,
            List<IDecision> decisionSources,
            Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, place it next to a hero target.
            LinqCardCriteria validTargets = new LinqCardCriteria(
                c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText,
                "hero target");

            IEnumerator routine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(
                    (Card c) => validTargets.Criteria(c) &&
                                (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)),
                    validTargets.Description), storedResults, isPutIntoPlay, decisionSources);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            yield break;
        }

        public override void AddTriggers()
        {
            //Damage dealt by that target is irreducible.
            base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.GetCardThisCardIsNextTo()));

            //At the start of that hero's turn, that target may deal itself 2 toxic damage. If no damage is taken this way, this card is destroyed.
            this.AddStartOfTurnTrigger(
                (TurnTaker tt) =>
                    tt == base.GetCardThisCardIsNextTo().Owner,
                new Func<PhaseChangeAction, IEnumerator>(this.DamageOrDestroyResponse), new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.DestroySelf
                });

            base.AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger(null);
        }

        private IEnumerator DamageOrDestroyResponse(PhaseChangeAction phaseChange)
        {
            //that target may deal itself 2 toxic damage. 
            List<DealDamageAction> storedDamageResults = new List<DealDamageAction>();

            CardController cc = base.FindCardController(base.GetCardThisCardIsNextTo());
            IEnumerator coroutine = base.GameController.DealDamage(cc.HeroTurnTakerController, cc.CharacterCard, (Card c) => c.Equals(cc.CharacterCard), DamageAmount, DamageType.Toxic, optional: true, storedResults: storedDamageResults, cardSource: cc.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

           // If no damage is taken this way, this card is destroyed.
            if (base.DidIntendedTargetTakeDamage(storedDamageResults, cc.CharacterCard))
            {
                yield break;
            }


            IEnumerator destroyCardRoutine = base.DestroyThisCardResponse(phaseChange);
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
            }
        }
    }
}

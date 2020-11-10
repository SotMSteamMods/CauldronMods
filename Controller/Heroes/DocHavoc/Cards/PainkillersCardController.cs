using System;
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
            LinqCardCriteria validTargets = new LinqCardCriteria(
                c => c.IsHero && c.IsTarget && !c.IsIncapacitatedOrOutOfGame,
                "targets", false, false, null, null, false);

            IEnumerator routine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(
                    (Card c) => validTargets.Criteria(c) &&
                                (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)),
                    validTargets.Description, true, false, null, null, false), storedResults, isPutIntoPlay,
                decisionSources);

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
            base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.GetCardThisCardIsNextTo()));

            this.AddStartOfTurnTrigger(
                (TurnTaker tt) =>
                    tt == base.FindTurnTakersWhere(stt => stt.CharacterCard.Equals(base.GetCardThisCardIsNextTo())).FirstOrDefault(),
                new Func<PhaseChangeAction, IEnumerator>(this.DamageOrDestroyResponse), new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.DestroySelf
                }, null, false);

            base.AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger(null);

            base.AddTriggers();
        }

        private IEnumerator DamageOrDestroyResponse(PhaseChangeAction phaseChange)
        {
            List<DealDamageAction> storedDamageResults = new List<DealDamageAction>();

            CardController cc = base.FindCardController(base.GetCardThisCardIsNextTo());

            IEnumerator selectTargetsToSelfDamageRoutine = base.GameController.DealDamageToSelf(cc.HeroTurnTakerController, 
                (card => card.Equals(cc.CharacterCard)), DamageAmount,
                DamageType.Toxic, false, requiredDecisions: 1,
                storedResults: storedDamageResults, isOptional: true, cardSource: cc.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectTargetsToSelfDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectTargetsToSelfDamageRoutine);
            }

            if (base.DidIntendedTargetTakeDamage(storedDamageResults, cc.CharacterCard))
            {
                yield break;
            }


            IEnumerator destroyCardRoutine = base.GameController.DestroyCard(this.DecisionMaker, this.Card, 
                false, null, null, null, null, null, 
                null, null, null, base.GetCardSource(null));
            
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

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
            //this.AddIncreaseDamageTrigger((Func<DealDamageAction, bool>)(d => d.Target == this.CharacterCard), DamageDealtIncrease);

            /*
            MakeDamageIrreducibleStatusEffect mdise = new MakeDamageIrreducibleStatusEffect();
            mdise.TargetCriteria.IsSpecificCard = base.GetCardThisCardIsNextTo();
            mdise.UntilCardLeavesPlay();

            this.AddStatusEffect(mdise);
            */

            base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.GetCardThisCardIsNextTo()));



            
            this.AddStartOfTurnTrigger(
                (TurnTaker tt) =>
                    tt == base.FindTurnTakersWhere(stt => stt.CharacterCard.Equals(base.GetCardThisCardIsNextTo())).FirstOrDefault(),
                new Func<PhaseChangeAction, IEnumerator>(this.DamageOrDestroyResponse), new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.DestroySelf
                }, null, false);

            //this.AddMakeDamageIrreducibleStatusEffect(base.GetCardThisCardIsNextTo(), null, Phase.Start);

            //base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.Card 
            //    && base.GetCardThisCardIsNextTo(true) != null && base.GetCardThisCardIsNextTo(true).IsTarget, 
            //    (DealDamageAction dd) => base.GameController.GainHP(base.GetCardThisCardIsNextTo(true), new int?(2), null, null, base.GetCardSource(null)), TriggerType.GainHP, TriggerTiming.After, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);

            base.AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger(null);

            base.AddTriggers();
        }

        /*
        public override IEnumerator Play()
        {
            MakeDamageIrreducibleStatusEffect mdise = new MakeDamageIrreducibleStatusEffect()
            {
                TargetCriteria = { IsTarget = true }
            };

            IEnumerator statusEffectRoutine = this.AddStatusEffect(mdise, true);

            this.AddMakeDamageIrreducibleTrigger(
                (Func<DealDamageAction, bool>)(d => d.Target == base.GetCardThisCardIsNextTo()));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(statusEffectRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(statusEffectRoutine);
            }
        }
        */

        private IEnumerator DamageOrDestroyResponse(PhaseChangeAction phaseChange)
        {



            List<DealDamageAction> storedDamageResults = new List<DealDamageAction>();

            CardController cc = base.FindCardController(base.GetCardThisCardIsNextTo());

            IEnumerator selectTargetsToSelfDamageRoutine = base.GameController.SelectTargetsToDealDamageToSelf(cc.HeroTurnTakerController, 2, DamageType.Toxic, 1,
                true, null, storedResultsDamage: storedDamageResults);


//            IEnumerator routine = base.GameController.DealDamage(cc.HeroTurnTakerController,
//                cc.CharacterCard, (Func<Card, bool>) (c => c.Equals(cc.CharacterCard)),
//                DamageAmount, DamageType.Toxic, storedResults: storedDamageResults, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectTargetsToSelfDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectTargetsToSelfDamageRoutine);
            }

            foreach (DealDamageAction dda in storedDamageResults.Where(dda => !dda.DidDealDamage))
            {
                IEnumerator coroutine2 = base.GameController.DestroyCard(dda.DecisionMaker, this.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }
    }
}

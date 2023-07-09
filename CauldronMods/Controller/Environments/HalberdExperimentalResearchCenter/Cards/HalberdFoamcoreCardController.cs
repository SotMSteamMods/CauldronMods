using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdFoamcoreCardController : TestSubjectCardController
    {
        private ITrigger _reduceDamageTrigger;

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (!base.GameController.PreviewMode)
                {
                    return IsLowestHitPointsUnique((Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText);
                }
                return true;
            }
        }

        private bool? PerformReduction
        {
            get;
            set;
        }

        #region Constructors

        public HalberdFoamcoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithLowestHP().Condition = () => !IsChemicalTriggerInPlay();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //criteria for finding hero target with the lowest hp when no chemical triggers are in play
            Func<DealDamageAction, bool> heroCriteria = (DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && base.CanCardBeConsideredLowestHitPoints(dd.Target, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText);

            //criteria for finding all villain targets when chemical triggers are in play
            Func<DealDamageAction, bool> villainCriteria = (DealDamageAction dd) => base.IsChemicalTriggerInPlay() && IsVillainTarget(dd.Target);

            //If there are no Chemical Triggers in play, reduce damage dealt to the hero target with the lowest HP by 1.
            _reduceDamageTrigger = base.AddTrigger<DealDamageAction>(heroCriteria, this.MaybeReduceDamageToHeroTargetResponse, TriggerType.ReduceDamage, TriggerTiming.Before);
            //Otherwise, reduce damage dealt to villain targets by 1.
            base.AddReduceDamageTrigger(villainCriteria, (DealDamageAction dd) => 1);
        }

        private IEnumerator MaybeReduceDamageToHeroTargetResponse(DealDamageAction dd)
        {
            if (base.GameController.PretendMode)
            {
                List<bool> storedResults = new List<bool>();
                IEnumerator coroutine = base.DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.Target, highest: false, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText, dd, storedResults);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                PerformReduction = storedResults.Count() > 0 && storedResults.First();
            }
            if (PerformReduction.HasValue && PerformReduction.Value)
            {
                IEnumerator coroutine2 = base.GameController.ReduceDamage(dd, 1, _reduceDamageTrigger, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            if (!base.GameController.PretendMode)
            {
                PerformReduction = null;
            }
        }
        #endregion Methods
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdEtherCardController : TestSubjectCardController
    {
        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (!base.GameController.PreviewMode)
                {
                    return IsHighestHitPointsUnique((Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText);
                }
                return true;
            }
        }

        private bool? PerformIncrease
        {
            get;
            set;
        }

        #region Constructors

        public HalberdEtherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !IsChemicalTriggerInPlay();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //If there are no Chemical Triggers in play, increase damage dealt by the hero target with the highest HP by 1.
            //Otherwise, increase damage dealt by villain targets by 1

            //criteria for finding hero target with the highest hp when no chemical triggers are in play
            Func<DealDamageAction, bool> heroCriteria = (DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && dd.DamageSource != null  && dd.DamageSource.Card != null && base.CanCardBeConsideredHighestHitPoints(dd.DamageSource.Card, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText);

            //criteria for finding all villain targets when chemical triggers are in play
            Func<DealDamageAction, bool> villainCriteria = (DealDamageAction dd) => base.IsChemicalTriggerInPlay() && dd.DamageSource != null && dd.DamageSource.Card != null && IsVillainTarget(dd.DamageSource.Card);

            //increase damage dealt by villain targets by 1 if chemical trigger is in play
            base.AddIncreaseDamageTrigger(villainCriteria, 1);

            //increase damage dealt by hero targets by 1 if no chemical trigger is in play
            base.AddTrigger<DealDamageAction>(heroCriteria, MaybeIncreaseDamageByHeroTargetResponse, TriggerType.IncreaseDamage, TriggerTiming.Before);
        }

        private IEnumerator MaybeIncreaseDamageByHeroTargetResponse(DealDamageAction dd)
        {
            if (base.GameController.PretendMode)
            {
                List<bool> storedResults = new List<bool>();
                IEnumerator coroutine = base.DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.DamageSource.Card, highest: true, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText, dd, storedResults);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                PerformIncrease = storedResults.Count() > 0 && storedResults.First();
            }
            if (PerformIncrease.HasValue && PerformIncrease.Value)
            {
                IEnumerator coroutine2 = base.GameController.IncreaseDamage(dd, 1, cardSource: base.GetCardSource());
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
                PerformIncrease = null;
            }
        }
        #endregion Methods
    }
}

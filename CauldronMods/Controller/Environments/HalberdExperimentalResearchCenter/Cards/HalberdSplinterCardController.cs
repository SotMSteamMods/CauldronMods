using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdSplinterCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdSplinterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(numberOfTargets: 3).Condition = () => IsChemicalTriggerInPlay();
            SpecialStringMaker.ShowVillainTargetWithLowestHP(numberOfTargets: 3).Condition = () => !IsChemicalTriggerInPlay();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, if there are no Chemical Triggers in play, this card deals the 3 villain targets with the lowest HP 1 projectile damage each. 
            //Otherwise, this card deal the 3 hero targets with the lowest HP 1 projectile damage each.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            Func<Card, bool> targets;
            //set targets based on whether there are chemical triggers in play
            if (base.IsChemicalTriggerInPlay())
            {
                //find all hero targets
                targets = (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText;
            }
            else
            {
                //find all villain targets
                targets = (Card c) => IsVillainTarget(c) && c.IsInPlayAndHasGameText;
            }

            //deal the 3 hero/villain targets with the lowest HP 1 projectile damage each
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, targets, (Card c) => 1, DamageType.Projectile, numberOfTargets: 3);
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
        #endregion Methods
    }
}
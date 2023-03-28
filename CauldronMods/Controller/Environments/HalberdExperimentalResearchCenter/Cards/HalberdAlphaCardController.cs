using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdAlphaCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdAlphaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP().Condition = () => !IsChemicalTriggerInPlay();
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => IsChemicalTriggerInPlay();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, if there are no Chemical Triggers in play, each Test Subject deals the villain target with the highest HP 1 melee damage. 
            //Otherwise, each Test Subject deals the hero target with the highest HP 1 melee damage.
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

            IEnumerable<Card> testSubjectsInPlay = FindAllTestSubjectsInPlay();
            IEnumerator coroutine;
            foreach (Card testSubject in testSubjectsInPlay)
            {
                //each Test Subject deals the hero/villain target with the highest HP 1 melee damage
                coroutine = base.DealDamageToHighestHP(testSubject, 1, targets, (Card c) => new int?(1), DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        private IEnumerable<Card> FindAllTestSubjectsInPlay()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && base.IsTestSubject(c), "test subject in play"));
        }
        #endregion Methods
    }
}
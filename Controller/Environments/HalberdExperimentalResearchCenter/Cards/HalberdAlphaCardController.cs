using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdAlphaCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdAlphaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

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
            //if there are no Chemical Triggers in play...
            if (!base.IsChemicalTriggerInPlay())
            {
                //each Test Subject deals the villain target with the highest HP 1 melee damage.
                IEnumerable<Card> testSubjectsInPlay = FindAllTestSubjectsInPlay();
                IEnumerator coroutine;
                foreach(Card testSubject in testSubjectsInPlay)
                {

                    coroutine = base.DealDamageToHighestHP(testSubject, 1, (Card c) => c.IsVillainTarget, (Card c) => new int?(1), DamageType.Melee);
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
            else
            {
                //Otherwise, each Test Subject deals the hero target with the highest HP 1 melee damage.
                IEnumerable<Card> testSubjectsInPlay = FindAllTestSubjectsInPlay();
                IEnumerator coroutine2;
                foreach (Card testSubject in testSubjectsInPlay)
                {

                    coroutine2 = base.DealDamageToHighestHP(testSubject, 1, (Card c) => c.IsTarget && c.IsHero, (Card c) => new int?(1), DamageType.Melee);
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
            yield break;
        }

        private IEnumerable<Card> FindAllTestSubjectsInPlay()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && base.IsTestSubject(c), "test subject in play"));
        }
        #endregion Methods
    }
}
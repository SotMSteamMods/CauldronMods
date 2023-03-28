using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdZephyrCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdZephyrCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP().Condition = () => !IsChemicalTriggerInPlay();
            SpecialStringMaker.ShowVillainTargetWithLowestHP().Condition = () => IsChemicalTriggerInPlay();

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, each Test Subject regains 1HP.
            //Then, If there are no Chemical Triggers in play, the hero with the lowest HP regains 3HP. Otherwise, the villain target with the lowest HP regains 3HP.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //each Test Subject regains 1HP.
            IEnumerator coroutine = base.GameController.GainHP(this.DecisionMaker, (Card c) => base.IsTestSubject(c), 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, If there are no Chemical Triggers in play, the hero with the lowest HP regains 3HP. Otherwise, the villain target with the lowest HP regains 3HP.
            Func<Card, bool> targets;
            //set targets based on whether there are chemical triggers in play
            if (!base.IsChemicalTriggerInPlay())
            {
                //find  hero character card with the lowest hitpoints
                targets = (Card c) => base.CanCardBeConsideredLowestHitPoints(c, (Card card) => IsHeroCharacterCard(card));
            }
            else
            {
                //find villain target with the lowest hitpoints
                targets = (Card c) => base.CanCardBeConsideredLowestHitPoints(c, (Card card) => IsVillainTarget(card));
            }

            //the hero/villain target with the lowest HP regains 3HP
            IEnumerator coroutine2 = base.GameController.GainHP(this.DecisionMaker, targets, 3, numberOfCardsToHeal: 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }
        #endregion Methods
    }
}
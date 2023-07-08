using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdPlasmaCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdPlasmaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP().Condition = () => !IsChemicalTriggerInPlay();
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => IsChemicalTriggerInPlay();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, if there are no Chemical Triggers in play, this card deals the villain target with highest HP {H} energy damage.,
            //Otherwise, this card deals the hero target with the highest HP {H} energy damage.
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

            //deals the hero/villain target with the highest HP {H} energy damage
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, targets, (Card c) => base.H, DamageType.Energy);
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
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdOmegaCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdOmegaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, if there are no Chemical Triggers in play, this cards deals each villain target 2 infernal damage. 
            //Otherwise, this cards deals each hero target 2 infernal damage. 
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnResponse, TriggerType.DealDamage);
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

            //this cards deals each hero/villain target 2 infernal damage. 
            IEnumerator coroutine = base.DealDamage(base.Card, targets, 2, DamageType.Infernal);
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
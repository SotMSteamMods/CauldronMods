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
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //if there are no Chemical Triggers in play...
            if (!base.IsChemicalTriggerInPlay())
            {
                //this cards deals each villain target 2 infernal damage. 
                IEnumerator coroutine = base.DealDamage(base.Card, (Card c) => c.IsVillainTarget, 2, DamageType.Infernal);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            else
            {
                //Otherwise, this cards deals each hero target 2 infernal damage. 
                IEnumerator coroutine2 = base.DealDamage(base.Card, (Card c) => c.IsTarget && c.IsHero, 2, DamageType.Infernal);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            yield break;
        }
        #endregion Methods
    }
}
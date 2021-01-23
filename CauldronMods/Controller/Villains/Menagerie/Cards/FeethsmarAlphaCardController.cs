using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class FeethsmarAlphaCardController : MenagerieCardController
    {
        public FeethsmarAlphaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt to Enclosures by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => base.IsEnclosure(action.Target), 1);

            //At the end of the villain turn, this card deals each non-Specimen target 2 cold damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
            base.AddTriggers();

        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals each non-Specimen target 2 cold damage.
            IEnumerator coroutine = base.DealDamage(base.Card, (Card c) => !base.IsSpecimen(c), 2, DamageType.Cold);
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
    }
}
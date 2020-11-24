using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class SacrificialShrineCardController : StSimeonsRoomCardController
    {
        public static readonly string Identifier = "SacrificialShrine";

        public SacrificialShrineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each target 2 psychic damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => this.DealAllTargetsDamage(), TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator DealAllTargetsDamage()
        {
            IEnumerator dealDamage = base.DealDamage(base.Card, (Card c) => c.IsTarget, 2, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamage);
            }

            yield break;
        }
    }
}
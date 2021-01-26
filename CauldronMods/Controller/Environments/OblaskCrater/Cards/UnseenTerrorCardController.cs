using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class UnseenTerrorCardController : OblaskCraterUtilityCardController
    {
        /*
         * At the end of the environment turn, this card deals the 3 other targets with the highest hp {H - 2} cold damage each.
         * Until the end of the next environment turn, this card becomes immune to damage from targets that were not damaged this way.
         */
        public UnseenTerrorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt.IsEnvironment, PhaseChangeActionResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.ImmuneToDamage });
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            List<DealDamageAction> storedDealDamageActions = new List<DealDamageAction>();

            base.RemoveTemporaryTriggers();

            coroutine = base.DealDamageToHighestHP(base.Card, 1, (card) => card != base.Card, (card) => base.H - 2, DamageType.Cold, storedResults: storedDealDamageActions, numberOfTargets: () => 3);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedDealDamageActions != null && storedDealDamageActions.Count() > 0)
            {
                base.AddToTemporaryTriggerList(base.AddPreventDamageTrigger((dda) => !storedDealDamageActions.Select((item) => item.Target).Contains(dda.DamageSource.Card)));
            }

            yield break;
        }
    }
}

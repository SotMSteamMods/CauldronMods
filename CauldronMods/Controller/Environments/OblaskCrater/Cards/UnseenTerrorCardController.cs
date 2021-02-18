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
            SpecialStringMaker.ShowHighestHP(numberOfTargets: () => 3, cardCriteria: new LinqCardCriteria(c => c != Card, "other targets", useCardsSuffix: false));
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt.IsEnvironment, PhaseChangeActionResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.ImmuneToDamage });
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            List<DealDamageAction> storedDealDamageActions = new List<DealDamageAction>();

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
                base.AddToTemporaryTriggerList(base.AddImmuneToDamageTrigger((dda) => dda.Target == base.Card && !storedDealDamageActions.Where(item => item.DidDealDamage).Select((item) => item.Target).Contains(dda.DamageSource.Card)));

                string targets = string.Join(" or ",storedDealDamageActions.Where(dd => dd.DidDealDamage).Select(dd => dd.Target.Title).ToArray());
                string description = $"Until the end of the next environment turn, {Card.Title} is immune to damage from targets that are not {targets}.";
                OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(RemoveTemporaryTriggerResponse), description, new TriggerType[] { TriggerType.RemoveTrigger }, Card);
                effect.TurnTakerCriteria.IsSpecificTurnTaker = TurnTaker;
                effect.TurnPhaseCriteria.Phase = Phase.End;
                effect.TurnIndexCriteria.EqualTo = Game.TurnIndex + Game.TurnTakers.Count();
                //effect.FromTurnPhaseExpiryCriteria.Phase = Phase.End;
                //effect.FromTurnPhaseExpiryCriteria.TurnTaker = TurnTaker;
                //effect.FromTurnPhaseExpiryCriteria.ExcludeRoundNumber = Game.Round;
                coroutine = AddStatusEffect(effect);
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

        public IEnumerator RemoveTemporaryTriggerResponse(PhaseChangeAction _, StatusEffect _2)
        {
            base.RemoveTemporaryTriggers();
            IEnumerator coroutine = GameController.ExpireStatusEffect(_2, GetCardSource());
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

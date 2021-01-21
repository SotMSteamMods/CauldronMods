using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    /*
     * At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 psychic damage.
     * At the start of the villain turn, {Gargoyle} may deal 1 target 0 psychic damage.
    */
    public class TerrorizeCardController : GargoyleUtilityCardController
    {
        public TerrorizeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => true, EndOfTurnResponse, TriggerType.DealDamage);
            base.AddStartOfTurnTrigger((tt) => tt.IsVillain, StartOfVillainTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            List<DealDamageJournalEntry> dealDamageJournalEntries;

            dealDamageJournalEntries = base.GameController.Game.Journal.DealDamageEntriesThisTurn().Where((ddje)=>ddje.SourceCard == base.CharacterCard && (ddje.TargetCard.IsEnvironmentTarget || base.IsVillainTarget(ddje.TargetCard))).ToList();
            
            foreach(DealDamageJournalEntry dealDamageJournalEntry in dealDamageJournalEntries)
            {
                coroutine = base.DealDamage(dealDamageJournalEntry.TargetCard, dealDamageJournalEntry.TargetCard, 1, DamageType.Psychic, cardSource: base.GetCardSource());
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

        private IEnumerator StartOfVillainTurnResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 0, DamageType.Psychic, 1, false, 0, cardSource: base.GetCardSource());
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
}

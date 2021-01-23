using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    /*
     * At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 irreducible psychic damage.
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
            base.AddStartOfTurnTrigger((tt) => IsVillain(tt), StartOfVillainTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            List<Card> dealDamageJournalEntryTargets;

            dealDamageJournalEntryTargets = base.GameController.Game.Journal.DealDamageEntriesThisTurn().Where((ddje)=>ddje.SourceCard == base.CharacterCard && !ddje.TargetCard.IsHero).Select(ddje => ddje.TargetCard).Distinct().ToList();

            coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 1, DamageType.Psychic, null, false, null, isIrreducible: true, additionalCriteria: (Card c) => dealDamageJournalEntryTargets.Contains(c), allowAutoDecide: true, cardSource: GetCardSource());
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

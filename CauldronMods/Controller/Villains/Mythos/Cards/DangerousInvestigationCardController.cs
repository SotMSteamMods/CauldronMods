using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class DangerousInvestigationCardController : MythosUtilityCardController
    {
        public DangerousInvestigationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private TokenPool DangerousInvestigationPool
        {
            get
            {
                return this.Card.FindTokenPool("DangerousInvestigationPool");
            }
        }

        public override void AddTriggers()
        {
            //{MythosClue} At the end of the villain turn, the players may play the top card of the villain deck to add a token to this card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.ClueResponse, TriggerType.PlayCard, (PhaseChangeAction action) => base.IsTopCardMatching(base.MythosClueDeckIdentifier));
            //At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator ClueResponse(PhaseChangeAction action)
        {
            //...the players may play the top card of the villain deck to add a token to this card.
            List<PlayCardAction> storedResults = new List<PlayCardAction>();
            IEnumerator coroutine = base.GameController.PlayTopCard(base.DecisionMaker, base.TurnTakerController, true, storedResults: storedResults, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Any())
            {
                coroutine = base.AddOrRemoveTokens(this.DangerousInvestigationPool, 1);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => 3, DamageType.Infernal, numberOfTargets: () => base.Game.H - (base.Game.Journal.CardEntersPlayEntriesThisTurn().Where((CardEntersPlayJournalEntry entry) => base.IsVillain(entry.Card)).Count() - 1));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

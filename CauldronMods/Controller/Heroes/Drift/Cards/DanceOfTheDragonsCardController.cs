using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DanceOfTheDragonsCardController : DriftUtilityCardController
    {
        public DanceOfTheDragonsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{DriftFuture}
            if (base.IsTimeMatching(base.Future))
            {
                //{Drift} deals up to 3 targets 2 radiant damage each.
                //Increase damage dealt this way by 1 to targets that entered play since the end of your last turn.
                IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), (Card target) => 2 + this.IncreaseDamageIfTargetEnteredPlaySinceLastTurn(target), DamageType.Radiant, () => 3, true, 0, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftL}.
                coroutine = base.ShiftL();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{DriftPast}
            if (base.IsTimeMatching(base.Future))
            {
                //Draw a card. 
                IEnumerator coroutine = base.DrawCard();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //{Drift} regains 1 HP. 
                coroutine = base.GameController.GainHP(base.GetActiveCharacterCard(), 1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftRR}
                coroutine = base.ShiftRR();
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

        private int IncreaseDamageIfTargetEnteredPlaySinceLastTurn(Card target)
        {
            IEnumerable<CardEntersPlayJournalEntry> cardEntries = base.Journal.QueryJournalEntries<CardEntersPlayJournalEntry>((CardEntersPlayJournalEntry e) => e.Card.IsTarget).Where(base.Game.Journal.SinceLastTurn<CardEntersPlayJournalEntry>(base.TurnTaker));
            if (cardEntries.Where(entry => entry.Card == target).Any())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}

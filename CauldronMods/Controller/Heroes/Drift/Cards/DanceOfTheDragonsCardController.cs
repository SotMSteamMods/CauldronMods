using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public class DanceOfTheDragonsCardController : DriftBaseCardController
{
    public DanceOfTheDragonsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
        base.SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsTarget && this.IncreaseDamageIfTargetEnteredPlaySinceLastTurn(c) == 1, "entered play since Drift's last turn"));
    }

    public override IEnumerator Play()
    {
        RemoveTemporaryTriggers();
        AddToTemporaryTriggerList(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.CardSource != null && dd.CardSource.Card == this.Card && this.IncreaseDamageIfTargetEnteredPlaySinceLastTurn(dd.Target) == 1, 1));
        //{DriftFuture}
        if (this.IsTimeMatchingFuture)
        {
            //{Drift} deals up to 3 targets 2 radiant damage each.
            //Increase damage dealt this way by 1 to targets that entered play since the end of your last turn.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), (Card target) => 2, DamageType.Radiant, () => 3, false, 0, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shift {DriftL}.
            coroutine = this.ShiftL();
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
        if (this.IsTimeMatchingPast)
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
            coroutine = base.GameController.GainHP(base.GetActiveCharacterCard(), 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shift {DriftRR}
            coroutine = this.ShiftRR();
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
        IEnumerable<CardEntersPlayJournalEntry> cardEntries = base.Journal.QueryJournalEntries((CardEntersPlayJournalEntry e) => e.Card.IsTarget).Where(base.Game.Journal.SinceLastTurn<CardEntersPlayJournalEntry>(base.TurnTaker));
        if (cardEntries.Any(entry => entry.Card == target))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

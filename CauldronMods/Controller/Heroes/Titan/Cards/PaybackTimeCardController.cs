using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class PaybackTimeCardController : CardController
    {
        public PaybackTimeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => !IsHeroTarget(c) && this.HasTargetDealtDamageToTitanSinceHisLastTurn(c), "non-hero targets who have dealt damge to Titan since the end of his last turn"));
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by {Titan} by 1 to non-hero targets that have dealt him damage since the end of his last turn.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard && !IsHero(action.Target) && this.HasTargetDealtDamageToTitanSinceHisLastTurn(action.Target), 1);
            //At the end of your turn {Titan} regains 1HP.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.GainHPResponse, TriggerType.GainHP);
        }

        private IEnumerator GainHPResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, 1, cardSource: base.GetCardSource());
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

        private bool HasTargetDealtDamageToTitanSinceHisLastTurn(Card target)
        {
            if (target.IsTarget)
            {
                DealDamageJournalEntry dealDamageJournalEntry = (from d in base.Journal.DealDamageEntriesFromTargetToTargetSinceLastTurn(target, base.CharacterCard, base.TurnTaker) where d.Amount > 0 select d).LastOrDefault<DealDamageJournalEntry>();
                if (dealDamageJournalEntry != null && target.IsInPlayAndHasGameText)
                {
                    int? entryIndex = base.GameController.Game.Journal.GetEntryIndex(dealDamageJournalEntry);
                    PlayCardJournalEntry playCardJournalEntry = (from c in base.GameController.Game.Journal.PlayCardEntries() where c.CardPlayed == target select c).LastOrDefault<PlayCardJournalEntry>();
                    if (playCardJournalEntry == null)
                    {
                        return true;
                    }
                    int? entryIndex2 = base.GameController.Game.Journal.GetEntryIndex(playCardJournalEntry);
                    int? num = entryIndex;
                    int? num2 = entryIndex2;
                    if (num.GetValueOrDefault() > num2.GetValueOrDefault() & (num != null & num2 != null))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
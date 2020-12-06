using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SuperstormAkela
{
    public class ToppledSkyscraperCardController : SuperstormAkelaCardController
    {

        public ToppledSkyscraperCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //The first time a villain target would be dealt damage each turn, redirect it to this card.
            AddTrigger((DealDamageAction dd) => dd.Target.IsVillainTarget && !IsPropertyTrue(GeneratePerTargetKey("VillainTargetWouldBeDealtDamage", dd.Target)) && !base.Journal.DealDamageEntriesThisTurn().Any((DealDamageJournalEntry ddje) => ddje.TargetCard == dd.Target), RedirectToThisCardResponse, TriggerType.RedirectDamage, TriggerTiming.Before);

            //At the start of each hero’s turn, they may choose what order to perform that turn's play, power, and draw phases in.
            AddTrigger((PhaseChangeAction p) => p.FromPhase.TurnTaker.IsHero && p.FromPhase.TurnTaker == p.ToPhase.TurnTaker && !p.FromPhase.TurnTaker.IsIncapacitatedOrOutOfGame && !p.FromPhase.IsBeforeStart && !p.FromPhase.IsAfterEnd && !p.ToPhase.IsBeforeStart && !p.ToPhase.IsAfterEnd, (PhaseChangeAction p) => base.GameController.ChooseNextPhase(p, cardSource: GetCardSource()), new TriggerType[1]
            {
                TriggerType.ChangePhaseOrder
            }, TriggerTiming.Before);
        }

        private IEnumerator RedirectToThisCardResponse(DealDamageAction dd)
        {
            //Redirect it to this card.
            SetCardPropertyToTrueIfRealAction(GeneratePerTargetKey("VillainTargetWouldBeDealtDamage", dd.Target));
            IEnumerator coroutine = GameController.RedirectDamage(dd, base.Card, cardSource: GetCardSource());
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

        public override TurnPhase AskIfTurnPhaseShouldBeChanged(TurnPhase fromPhase, TurnPhase toPhase)
        {
            if (GameController.ActiveTurnTaker.IsHero)
            {
                TurnTaker isSpecificTurnTaker = GameController.ActiveTurnTaker;
                return base.GameController.GetNextTurnPhaseIfChoosable(isSpecificTurnTaker.ToHero(), fromPhase, toPhase, GetCardSource());
            }
            return base.AskIfTurnPhaseShouldBeChanged(fromPhase, toPhase);
        }
    }
}
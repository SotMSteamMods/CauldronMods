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
            SpecialStringMaker.ShowHasBeenUsedThisTurn("VillainTargetWouldBeDealtDamage");
        }

        public override void AddTriggers()
        {
            //The first time a villain target would be dealt damage each turn, redirect it to this card.
            AddTrigger((DealDamageAction dd) => dd.Amount > 0 && IsVillainTarget(dd.Target) && !HasBeenSetToTrueThisTurn("VillainTargetWouldBeDealtDamage"), RedirectToThisCardResponse, TriggerType.RedirectDamage, TriggerTiming.Before, actionType: ActionDescription.DamageTaken);

            //At the start of each hero’s turn, they may choose what order to perform that turn's play, power, and draw phases in.
            AddTrigger((PhaseChangeAction p) =>IsHero(p.FromPhase.TurnTaker) && p.FromPhase.TurnTaker == p.ToPhase.TurnTaker && !p.FromPhase.TurnTaker.IsIncapacitatedOrOutOfGame && !p.FromPhase.IsBeforeStart && !p.FromPhase.IsAfterEnd && !p.ToPhase.IsBeforeStart && !p.ToPhase.IsAfterEnd, (PhaseChangeAction p) => base.GameController.ChooseNextPhase(p, cardSource: GetCardSource()), new TriggerType[1]
            {
                TriggerType.ChangePhaseOrder
            }, TriggerTiming.Before);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("VillainTargetWouldBeDealtDamage"), TriggerType.Hidden);
        }

        private IEnumerator RedirectToThisCardResponse(DealDamageAction dd)
        {
            //Redirect it to this card.
            SetCardPropertyToTrueIfRealAction("VillainTargetWouldBeDealtDamage");
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
            if (IsHero(GameController.ActiveTurnTaker))
            {
                TurnTaker isSpecificTurnTaker = GameController.ActiveTurnTaker;
                return base.GameController.GetNextTurnPhaseIfChoosable(isSpecificTurnTaker.ToHero(), fromPhase, toPhase, GetCardSource());
            }
            return base.AskIfTurnPhaseShouldBeChanged(fromPhase, toPhase);
        }
    }
}
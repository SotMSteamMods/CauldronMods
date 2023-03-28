using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Echelon
{
    public class InOutCardController : CardController
    {

        public InOutCardController(Card card, TurnTakerController turnTakerController, Phase phaseToIncrease, Phase phaseToSkip) : base(card, turnTakerController)
        {
            PhaseToIncrease = phaseToIncrease;
            PhaseToSkip = phaseToSkip;
        }

        private const int HpGain = 1;
        private Phase PhaseToIncrease;
        private Phase PhaseToSkip;

        public override void AddTriggers()
        {
            // At the start of your turn, destroy this card and each hero target regains 1HP.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, DestroyAndHealResponse, new[]
            {
                TriggerType.GainHP,
                TriggerType.DestroySelf
            });

            // At the start of a player's turn, that player may choose to skip one of their phases.
            AddStartOfTurnTrigger(tt => tt != base.TurnTaker && IsHero(tt), StartOfTurnPhaseShiftResponse,
                new[] { TriggerType.ReducePhaseActionCount, TriggerType.IncreasePhaseActionCount });
        }

        private IEnumerator DestroyAndHealResponse(PhaseChangeAction pca)
        {

            // each hero target regains 1 HP
            IEnumerator healRoutine = base.GameController.GainHP(this.HeroTurnTakerController,
                 c => IsHeroTarget(c) && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), HpGain, cardSource: GetCardSource());

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, storedResults: storedResults, postDestroyAction: () => healRoutine,
                cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator StartOfTurnPhaseShiftResponse(PhaseChangeAction pca)
        {
            TurnTaker heroTT = pca.GameController.ActiveTurnTaker;
            HeroTurnTakerController heroTTC = pca.GameController.ActiveTurnTakerController.ToHero();

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(heroTTC, SelectionType.Custom, this.Card,
                storedResults: storedResults, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!base.DidPlayerAnswerYes(storedResults))
            {
                yield break;
            }

            var extraAction = new IncreasePhaseActionCountStatusEffect(1);
            extraAction.UntilThisTurnIsOver(Game);
            extraAction.ToTurnPhaseCriteria.Phase = PhaseToIncrease;
            extraAction.ToTurnPhaseCriteria.TurnTaker = GameController.ActiveTurnTaker;
            extraAction.CardSource = this.Card;

            routine = AddStatusEffect(extraAction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            var skippedAction = new PreventPhaseActionStatusEffect();
            skippedAction.UntilThisTurnIsOver(Game);
            skippedAction.ToTurnPhaseCriteria.Phase = PhaseToSkip;
            skippedAction.ToTurnPhaseCriteria.TurnTaker = GameController.ActiveTurnTaker;
            skippedAction.CardSource = this.Card;

            routine = AddStatusEffect(skippedAction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
            yield break;
        }

    }
}
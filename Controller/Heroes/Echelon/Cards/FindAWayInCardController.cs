using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class FindAWayInCardController : CardController
    {
        //==============================================================
        // At the start of a player's turn, that player may choose to skip their power Phase.
        // If they do, they may play 1 additional card during their play Phase.
        // At the start of your turn, destroy this card and each hero target regains 1HP.
        //==============================================================

        public static string Identifier = "FindAWayIn";

        private const int HpGain = 1;

        public FindAWayInCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the start of your turn, destroy this card and each hero target regains 1HP.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, DestroyAndHealResponse, new[]
            {
                TriggerType.GainHP,
                TriggerType.DestroySelf
            });

            // At the start of a player's turn, that player may choose to skip their power Phase.
            AddStartOfTurnTrigger(tt => tt != base.TurnTaker && tt.IsHero, StartOfTurnPhaseShiftResponse,
                new[] {TriggerType.ReducePhaseActionCount, TriggerType.IncreasePhaseActionCount});

            //base.AddAdditionalPhaseActionTrigger(tt => tt != base.TurnTaker && tt.IsHero && this.ShouldIncreasePhaseActionCount(tt), Phase.DrawCard, 1);
            

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnPhaseShiftResponse(PhaseChangeAction pca)
        {
            TurnTaker heroTT = pca.GameController.ActiveTurnTaker;
            HeroTurnTakerController heroTTC = pca.GameController.ActiveTurnTakerController.ToHero();

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(heroTTC, SelectionType.PlayExtraCard, this.Card,
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

            var extraPlay = new IncreasePhaseActionCountStatusEffect(1);
            extraPlay.UntilThisTurnIsOver(Game);
            extraPlay.ToTurnPhaseCriteria.Phase = Phase.PlayCard;
            extraPlay.ToTurnPhaseCriteria.TurnTaker = GameController.ActiveTurnTaker;
            extraPlay.CardSource = this.Card;

            routine = AddStatusEffect(extraPlay);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            var skipPower = new PreventPhaseActionStatusEffect();
            skipPower.UntilThisTurnIsOver(Game);
            skipPower.ToTurnPhaseCriteria.Phase = Phase.UsePower;
            skipPower.ToTurnPhaseCriteria.TurnTaker = GameController.ActiveTurnTaker;
            skipPower.CardSource = this.Card;

            routine = AddStatusEffect(skipPower);
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

        private IEnumerator DestroyAndHealResponse(PhaseChangeAction pca)
        {

            // each hero target regains 1 HP
           IEnumerator healRoutine = base.GameController.GainHP(this.HeroTurnTakerController,
                c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), HpGain, cardSource: GetCardSource());

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, storedResults: storedResults,postDestroyAction: () => healRoutine,
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
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class NeedAWayOutCardController : CardController
    {
        //==============================================================
        // At the start of a player's turn, that player may choose to skip their play Phase.
        // If they do, they may use 1 additional power during their power Phase.
        // At the start of your turn, destroy this card and each hero target regains 1HP.
        //==============================================================

        public static string Identifier = "NeedAWayOut";

        private const int HpGain = 1;

        public NeedAWayOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the start of a player's turn, that player may choose to skip their play Phase.
            // If they do, they may use 1 additional power during their power Phase.


            // At the start of your turn, destroy this card and each hero target regains 1HP.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, DestroyAndHealResponse, new[]
            {
                TriggerType.GainHP,
                TriggerType.DestroySelf
            });

            // At the start of a player's turn, that player may choose to skip their power Phase.
            AddStartOfTurnTrigger(tt => tt != base.TurnTaker && tt.IsHero, StartOfTurnPhaseShiftResponse,
                new[] { TriggerType.ReducePhaseActionCount, TriggerType.IncreasePhaseActionCount });

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnPhaseShiftResponse(PhaseChangeAction pca)
        {
            TurnTaker heroTT = pca.GameController.ActiveTurnTaker;
            HeroTurnTakerController heroTTC = pca.GameController.ActiveTurnTakerController.ToHero();

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(heroTTC, SelectionType.UsePowerTwice, this.Card,
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

            var extraPower = new IncreasePhaseActionCountStatusEffect(1);
            extraPower.UntilThisTurnIsOver(Game);
            extraPower.ToTurnPhaseCriteria.Phase = Phase.UsePower;
            extraPower.ToTurnPhaseCriteria.TurnTaker = GameController.ActiveTurnTaker;
            extraPower.CardSource = this.Card;

            routine = AddStatusEffect(extraPower);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            var skipPlay = new PreventPhaseActionStatusEffect();
            skipPlay.UntilThisTurnIsOver(Game);
            skipPlay.ToTurnPhaseCriteria.Phase = Phase.PlayCard;
            skipPlay.ToTurnPhaseCriteria.TurnTaker = GameController.ActiveTurnTaker;
            skipPlay.CardSource = this.Card;

            routine = AddStatusEffect(skipPlay);
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
            IEnumerator routine = base.GameController.GainHP(this.HeroTurnTakerController,
                c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), HpGain, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, storedResults: storedResults,
                cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!storedResults.Any() || !base.DidDestroyCard(storedResults.First()))
            {
                yield break;
            }
        }
    }
}
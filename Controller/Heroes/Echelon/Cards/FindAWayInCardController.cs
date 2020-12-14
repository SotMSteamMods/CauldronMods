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
            //base.AddStartOfTurnTrigger(tt => tt != base.TurnTaker && tt.IsHero, StartOfTurnResponse,
                //new[] {TriggerType.ReducePhaseActionCount, TriggerType.IncreasePhaseActionCount});

            base.AddAdditionalPhaseActionTrigger(tt => tt != base.TurnTaker && tt.IsHero && this.ShouldIncreasePhaseActionCount(tt), Phase.DrawCard, 1);
            

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            TurnTaker heroTT = pca.GameController.ActiveTurnTaker;
            HeroTurnTakerController heroTTC = pca.GameController.ActiveTurnTakerController.ToHero();

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(heroTTC, SelectionType.DrawExtraCard, this.Card,
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

            routine = base.ReducePhaseActionCountIfInPhase(tt => tt == heroTT, Phase.UsePower, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            routine = base.IncreasePhaseActionCountIfInPhase(tt => tt == heroTT, Phase.DrawCard, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            //routine = base.AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == pca.DecisionMaker.TurnTaker, Phase.DrawCard, 1);
        }

        private IEnumerator DestroyAndHealResponse(PhaseChangeAction pca)
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card, storedResults: storedResults,
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

            // each hero target regains 2 HP
            routine = base.GameController.GainHP(this.HeroTurnTakerController,
                c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()), HpGain, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {



            return true;
            //return tt == base.TurnTaker;
        }

        private bool DidHeroUsePowerDuringPowerPhaseThisTurn(TurnTaker tt)
        {
            return tt.IsHero && base.Journal.UsePowerEntriesThisTurn().Any(p => p.PowerUser == tt && p.TurnPhase.TurnTaker == tt && p.TurnPhase.IsUsePower);
        }
    }
}
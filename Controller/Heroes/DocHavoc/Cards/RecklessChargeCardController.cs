using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class RecklessChargeCardController : CardController
    {
        //==============================================================
        // Increase damage dealt to Doc Havoc by 1. You may draw an extra card during your draw phase.
        //==============================================================

        public static string Identifier = "RecklessCharge";
        private const int DamageDealtIncrease = 1;
        private const int ExtraDrawPhaseCount = 1;

        public RecklessChargeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddIncreaseDamageTrigger((Func<DealDamageAction, bool>)(d => d.Target == this.CharacterCard), DamageDealtIncrease);
            
            this.AddAdditionalPhaseActionTrigger((Func<TurnTaker, bool>)(this.ShouldIncreasePhaseActionCount), Phase.DrawCard, ExtraDrawPhaseCount);
            
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator increasePhaseActionRoutine 
                = this.IncreasePhaseActionCountIfInPhase(new Func<TurnTaker, bool>(ttc => ttc == this.TurnTaker), Phase.DrawCard, ExtraDrawPhaseCount);

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(increasePhaseActionRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(increasePhaseActionRoutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Destroy this card
            IEnumerator destroyCardRoutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(destroyCardRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(destroyCardRoutine);
            }
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount() => this.GameController.ActiveTurnPhase.IsDrawCard && this.ShouldIncreasePhaseActionCount(this.GameController.ActiveTurnTaker);


        private bool ShouldIncreasePhaseActionCount(TurnTaker tt) => tt == this.TurnTaker;
    }
}

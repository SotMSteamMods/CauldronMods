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
            //Increase damage dealt to DocHavoc by 1
            this.AddIncreaseDamageTrigger(d => d.Target == this.CharacterCard, DamageDealtIncrease);

            //You may draw an extra card during your draw phase.
            this.AddAdditionalPhaseActionTrigger(this.ShouldIncreasePhaseActionCount, Phase.DrawCard, ExtraDrawPhaseCount);

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

using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class LivingInkCardController : CardController
    {
        //==============================================================
        // At the end of the villain turn, each villain target regains 3 HP.
        //==============================================================

        public static string Identifier = "LivingInk";

        private const int HpToGain = 3;

        public LivingInkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, this.GainHpResponse, TriggerType.GainHP);

            base.AddTriggers();
        }

        private IEnumerator GainHpResponse(PhaseChangeAction pca)
        {
            // Villain targets regain 3 HP
            IEnumerator gainHpRoutine = this.GameController.GainHP(this.HeroTurnTakerController, card => card.IsVillainTarget && card.IsInPlay,
                HpToGain);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(gainHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(gainHpRoutine);
            }
        }
    }
}
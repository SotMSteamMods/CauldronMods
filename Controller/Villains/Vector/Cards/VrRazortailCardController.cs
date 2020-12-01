using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class VrRazortailCardController : CardController
    {
        //==============================================================
        // Reduce damage dealt to this card by 1.
        // At the start of the villain turn, {Vector} regains {H} HP.
        //==============================================================

        public static readonly string Identifier = "VrRazortail";

        private const int ReduceDamageAmount = 1;

        public VrRazortailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the start of the villain turn, {Vector} regains {H} HP.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnResponse, TriggerType.GainHP);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Reduce damage dealt to this card by 1.
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(ReduceDamageAmount)
            {
                TargetCriteria = {IsSpecificCard = this.Card}
            };

            IEnumerator routine = this.AddStatusEffect(rdse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            int hpToGain = this.Game.H;

            IEnumerator routine = this.GameController.GainHP(this.CharacterCard, hpToGain);
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
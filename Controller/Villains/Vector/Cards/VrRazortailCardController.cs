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

            // Reduce damage dealt to this card by 1.
            base.AddReduceDamageTrigger((Card c) => c == base.Card, 1);

            base.AddTriggers();
        }

       

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            int hpToGain = this.Game.H;

            IEnumerator routine = this.GameController.GainHP(this.CharacterCard, hpToGain, cardSource: GetCardSource());
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
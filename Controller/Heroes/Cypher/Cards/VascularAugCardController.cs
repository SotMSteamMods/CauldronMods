using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class VascularAugCardController : AugBaseCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // That hero regains 1HP at the end of their turn.
        //==============================================================

        public static string Identifier = "VascularAug";

        private const int HpToGain = 1;

        public VascularAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger(tt => GetCardThisCardIsNextTo() != null && tt == GetCardThisCardIsNextTo().Owner, this.GainHpResponse, TriggerType.GainHP);

            base.AddTriggers();
        }

        private IEnumerator GainHpResponse(PhaseChangeAction pca)
        {
            IEnumerator routine = base.GameController.GainHP(DecisionMaker, card => card == base.GetCardThisCardIsNextTo() && card.IsInPlay, HpToGain, cardSource: GetCardSource());

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
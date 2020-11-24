using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PerfectFocusCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Increase the next damage dealt by {TangoOne} by 3.
        // You may play a card.
        //==============================================================

        public static readonly string Identifier = "PerfectFocus";

        private const int DamageIncrease = 3;

        public PerfectFocusCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Increase the next damage dealt by {TangoOne} by 3
            var effect = new IncreaseDamageStatusEffect(DamageIncrease);
            effect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            effect.CardSource = Card;
            effect.Identifier = IncreaseDamageIdentifier;
            effect.NumberOfUses = 1;

            IEnumerator increaseDamageRoutine = base.AddStatusEffect(effect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamageRoutine);
            }

            // Play a card
            IEnumerator playCardRoutine = base.SelectAndPlayCardFromHand(DecisionMaker, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }
    }
}
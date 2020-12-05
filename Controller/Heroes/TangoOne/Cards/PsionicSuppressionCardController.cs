using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PsionicSuppressionCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Select a target.
        // That target may not deal damage until the start of your next turn.
        //==============================================================

        public static readonly string Identifier = "PsionicSuppression";

        public PsionicSuppressionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Select target
            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();

            IEnumerator selectCardRoutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTargetNoDamage,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlayAndHasGameText, "targets in play", false), selectCardResults, false, false, null, true, base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCardRoutine);
            }

            if (DidSelectCard(selectCardResults))
            {
                Card selectedCard = GetSelectedCard(selectCardResults);

                // Apply cannot deal damage status effect to chosen card
                CannotDealDamageStatusEffect effect = new CannotDealDamageStatusEffect();
                effect.CardSource = Card;
                effect.SourceCriteria.IsSpecificCard = selectedCard;
                effect.UntilStartOfNextTurn(base.TurnTaker);
                effect.UntilTargetLeavesPlay(selectedCard);

                IEnumerator cannotDealDamageRoutine = base.AddStatusEffect(effect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(cannotDealDamageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(cannotDealDamageRoutine);
                }
            }
        }
    }
}
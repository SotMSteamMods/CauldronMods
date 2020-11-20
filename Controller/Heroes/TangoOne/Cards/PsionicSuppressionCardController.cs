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

        public static string Identifier = "PsionicSuppression";

        public PsionicSuppressionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Select target
            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();
            IEnumerator selectCardRoutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTargetNoDamage,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay, "targets in play", false),
                selectCardResults, false, false, null, true, base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCardRoutine);
            }

            Card selectedCard = GetSelectedCard(selectCardResults);

            // Apply cannot deal damage status effect to chosen card
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect
            {
                SourceCriteria = { IsSpecificCard = selectedCard}
            };
            cannotDealDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            cannotDealDamageStatusEffect.CardDestroyedExpiryCriteria.Card = base.CharacterCard;

            IEnumerator cannotDealDamageRoutine = base.AddStatusEffect(cannotDealDamageStatusEffect);
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
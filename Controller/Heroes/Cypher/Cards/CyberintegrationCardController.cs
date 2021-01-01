using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class CyberintegrationCardController : CypherBaseCardController
    {
        //==============================================================
        // Destroy any number of Augments.
        // When an Augment is destroyed this way, the hero it was next to regains 3HP.
        //==============================================================

        public static string Identifier = "Cyberintegration";

        private const int HpGain = 3;

        public CyberintegrationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowSpecialStringNumberOfAugmentsInPlay();
        }

        public override IEnumerator Play()
        {
            // Destroy any number of Augments.
            IEnumerator routine = GameController.SelectCardsAndDoAction(DecisionMaker, base.AugmentCardCriteria(c => c.IsInPlay), SelectionType.DestroyCard, DestroyAugAndHeal,
                requiredDecisions: 0,
                cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
            yield break;
        }

        private IEnumerator DestroyAugAndHeal(Card card)
        {
            Card heroToHeal = card.Location.OwnerCard;
            var storedDestroy = new List<DestroyCardAction> { };
            IEnumerator routine = GameController.DestroyCard(DecisionMaker, card, storedResults: storedDestroy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (DidDestroyCard(storedDestroy) && heroToHeal != null)
            {
                // When an Augment is destroyed this way, the hero it was next to regains 3HP
                routine = GameController.GainHP(heroToHeal, HpGain, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            yield break;
        }
    }
}
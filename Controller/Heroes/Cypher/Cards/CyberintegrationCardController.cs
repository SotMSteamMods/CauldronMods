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
        }

        public override IEnumerator Play()
        {
            // Destroy any number of Augments.
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator routine = base.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria(IsAugment),
                null, true, storedResultsAction: storedResults, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!storedResults.Any())
            {
                yield break;
            }

            // When an Augment is destroyed this way, the hero it was next to regains 3HP
            List<Card> ownerCards = storedResults.Select(dca => dca.CardToDestroy.Card.NextToLocation.OwnerCard).Distinct().ToList();

            routine = this.GameController.GainHP(DecisionMaker, c => c.IsHero && c.IsInPlayAndHasGameText 
                                    && ownerCards.Contains(c), HpGain, cardSource: GetCardSource());
            
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
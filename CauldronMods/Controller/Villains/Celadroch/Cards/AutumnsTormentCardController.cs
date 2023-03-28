using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class AutumnsTormentCardController : CardController
    {
        /*
         * "When this card enters play, destroy all environment cards.",
		 * "Whenever a hero plays a card, this card deals them 2 lightning damage."
         */
        public AutumnsTormentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<PlayCardAction>(pca => !pca.IsPutIntoPlay && IsHero(pca.TurnTakerController.TurnTaker) && pca.WasCardPlayed, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        public override IEnumerator Play()
        {
            var coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment, "environment"), autoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageResponse(PlayCardAction pca)
        {
            List<Card> result = new List<Card>();
            IEnumerator coroutine = base.FindCharacterCardToTakeDamage(pca.TurnTakerController.TurnTaker, result, Card, 2, DamageType.Lightning);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var target = result.FirstOrDefault();
            if (target != null)
            {
                coroutine = DealDamage(Card, target, 2, DamageType.Lightning, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}
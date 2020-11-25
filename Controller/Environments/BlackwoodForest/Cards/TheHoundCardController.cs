using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class TheHoundCardController : CardController
    {
        //==============================================================
        // When this card enters play, it deals the non-environment target
        // with the lowest HP {H + 1} melee damage and
        // destroys 1 hero ongoing or equipment card. Then, play
        // the top card of the environment deck
        // and shuffle this card back into the environment deck.
        //==============================================================

        public static readonly string Identifier = "TheHound";

        private const int TargetsToHit = 1;

        public TheHoundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator findTargetWithLowestHpRoutine = base.GameController.FindTargetsWithLowestHitPoints(1, TargetsToHit,
                c => c.IsTarget && !c.Owner.IsEnvironment, storedResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetWithLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetWithLowestHpRoutine);
            }

            if (storedResults.Any())
            {
                // Deals the non-environment target with the lowest HP {H + 1} melee damage
                int damageToDeal = Game.H + 1;

                IEnumerator dealDamageRoutine = this.DealDamage(this.Card, storedResults.First(), damageToDeal, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageRoutine);
                }
            }

            // Destroy 1 hero ongoing or equipment card
            IEnumerator destroyCardRoutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker,
                new LinqCardCriteria(card => card.Owner.IsHero && (IsEquipment(card) || card.IsOngoing)), false, cardSource: this.GetCardSource());

            // Play the top card of the environment deck
            IEnumerator playCardRoutine = this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController);

            // Shuffle The Hound back into the environment deck
            IEnumerator shuffleCardIntoRoutine = this.GameController.ShuffleCardIntoLocation(this.DecisionMaker, this.Card, this.TurnTaker.Deck, false, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCardRoutine);
                yield return base.GameController.StartCoroutine(playCardRoutine);
                yield return base.GameController.StartCoroutine(shuffleCardIntoRoutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCardRoutine);
                base.GameController.ExhaustCoroutine(playCardRoutine);
                base.GameController.ExhaustCoroutine(shuffleCardIntoRoutine);
            }
        }
    }
}
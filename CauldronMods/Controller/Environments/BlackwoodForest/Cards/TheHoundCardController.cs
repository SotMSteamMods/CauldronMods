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
        // destroys 1 hero ongoing or equipment card.
        // Then, play the top card of the environment deck
        // and shuffle this card back into the environment deck.
        //==============================================================

        public static readonly string Identifier = "TheHound";

        private const int TargetsToHit = 1;

        public TheHoundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP();
        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            var damageInfo = new DealDamageAction(GameController, new DamageSource(GameController, Card), null, H + 1, DamageType.Melee);
            var coroutine = base.GameController.FindTargetsWithLowestHitPoints(1, TargetsToHit,
                                c => c.IsTarget && !c.IsEnvironment, storedResults,
                                dealDamageInfo: new[] { damageInfo },
                                evenIfCannotDealDamage: true,
                                cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Any())
            {
                // Deals the non-environment target with the lowest HP {H + 1} melee damage
                coroutine = this.DealDamage(Card, storedResults.First(), H + 1, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // Destroy 1 hero ongoing or equipment card
            coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker,
                            new LinqCardCriteria(card => IsHero(card) && (IsEquipment(card) || IsOngoing(card)), "hero ongoing or equipment"),
                            false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Play the top card of the environment deck
            coroutine = this.GameController.PlayTopCard(DecisionMaker, TurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Shuffle The Hound back into the environment deck
            coroutine = this.GameController.ShuffleCardIntoLocation(DecisionMaker, Card, Card.NativeDeck, false, cardSource: GetCardSource());
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
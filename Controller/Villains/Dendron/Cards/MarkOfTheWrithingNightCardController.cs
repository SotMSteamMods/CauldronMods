using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class MarkOfTheWrithingNightCardController : CardController
    {

        //==============================================================
        // {Dendron} deals the hero with the highest HP 5 projectile damage
        // and the hero with the lowest HP 2 irreducible infernal damage.
        // Destroy 1 equipment and 1 ongoing from the hero with the fewest
        // cards in play, and 2 equipment and 2 ongoing cards from the
        // hero with the most cards in play.
        //==============================================================

        public static string Identifier = "MarkOfTheWrithingNight";

        private const int DamageToDealHighestHp = 5;
        private const int DamageToDealLowestHp = 2;
        private const int CardsToDestroyFewestCards = 1;
        private const int CardsToDestroyMostCards = 2;

        public MarkOfTheWrithingNightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Find hero with the highest HP
            List<Card> storedHighestHp = new List<Card>();
            IEnumerator getHighestHpRoutine = this.GameController.FindTargetWithHighestHitPoints(1,
                card => card.IsHero && !card.IsIncapacitatedOrOutOfGame, storedHighestHp);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(getHighestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(getHighestHpRoutine);
            }

            if (storedHighestHp.Any())
            {
                // Deal the hero with the highest HP 5 projectile damage
                IEnumerator dealDamageToHighestHpRoutine
                    = this.DealDamage(this.TurnTaker.CharacterCard, storedHighestHp.First(), DamageToDealHighestHp, DamageType.Projectile);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageToHighestHpRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageToHighestHpRoutine);
                }
            }


            // Find hero with the lowest HP
            List<Card> storedLowestHp = new List<Card>();
            IEnumerator getLowestHpRoutine = this.GameController.FindTargetWithLowestHitPoints(1,
                card => card.IsHero && !card.IsIncapacitatedOrOutOfGame, storedLowestHp);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(getLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(getLowestHpRoutine);
            }

            if (storedLowestHp.Any())
            {
                // Deal the hero with the lowest HP 2 irreducible infernal damage
                IEnumerator dealDamageToLowestHpRoutine
                    = this.DealDamage(this.TurnTaker.CharacterCard, storedLowestHp.First(), DamageToDealLowestHp, DamageType.Infernal, true);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageToLowestHpRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageToLowestHpRoutine);
                }
            }

            // Find hero with fewest cards in play
            List<TurnTaker> fewestCardsInPlayResults = new List<TurnTaker>();
            IEnumerator heroWithLeastCardsInPlayRoutine = base.FindHeroWithFewestCardsInPlay(fewestCardsInPlayResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(heroWithLeastCardsInPlayRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(heroWithLeastCardsInPlayRoutine);
            }

            if (fewestCardsInPlayResults.Any())
            {
                // Destroy 1 equipment and 1 ongoing from the hero with the fewest cards in play

                HeroTurnTakerController fewestCardsHttc = FindTurnTakerController(fewestCardsInPlayResults.First().CharacterCard.Owner).ToHero();

                IEnumerator destroyOneOngoingRoutine = base.GameController.SelectAndDestroyCards(fewestCardsHttc, 
                    new LinqCardCriteria(card => card.IsOngoing && card.Owner == fewestCardsHttc.HeroTurnTaker), CardsToDestroyFewestCards, cardSource: this.GetCardSource());

                IEnumerator destroyOneEquipmentRoutine = base.GameController.SelectAndDestroyCards(fewestCardsHttc, 
                    new LinqCardCriteria(card => IsEquipment(card) && card.Owner == fewestCardsHttc.HeroTurnTaker), CardsToDestroyFewestCards, cardSource: this.GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyOneOngoingRoutine);
                    yield return base.GameController.StartCoroutine(destroyOneEquipmentRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyOneOngoingRoutine);
                    base.GameController.ExhaustCoroutine(destroyOneEquipmentRoutine);
                }
            }

            // Find hero with most cards in play
            List<TurnTaker> mostCardsInPlayResults = new List<TurnTaker>();
            IEnumerator heroWithMostCardsInPlayRoutine = base.FindHeroWithMostCardsInPlay(mostCardsInPlayResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(heroWithMostCardsInPlayRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(heroWithMostCardsInPlayRoutine);
            }

            if (mostCardsInPlayResults.Any())
            {
                // Destroy 2 equipment and 2 ongoing from the hero with the most cards in play

                HeroTurnTakerController mostCardsHttc = FindTurnTakerController(mostCardsInPlayResults.First().CharacterCard.Owner).ToHero();

                IEnumerator destroyTwoOngoingRoutine = base.GameController.SelectAndDestroyCards(mostCardsHttc, 
                    new LinqCardCriteria(card => card.IsOngoing && card.Owner == mostCardsHttc.HeroTurnTaker), CardsToDestroyMostCards, cardSource: this.GetCardSource());

                IEnumerator destroyTwoEquipmentRoutine = base.GameController.SelectAndDestroyCards(mostCardsHttc, 
                    new LinqCardCriteria(card => IsEquipment(card) && card.Owner == mostCardsHttc.HeroTurnTaker), CardsToDestroyMostCards, cardSource: this.GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyTwoOngoingRoutine);
                    yield return base.GameController.StartCoroutine(destroyTwoEquipmentRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyTwoOngoingRoutine);
                    base.GameController.ExhaustCoroutine(destroyTwoEquipmentRoutine);
                }

            }

        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
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
        private const int DamageToDealLowesttHp = 2;
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

            // Find hero with the lowest HP
            List<Card> storedLowestHp = new List<Card>();
            IEnumerator getLowestHpRoutine = this.GameController.FindTargetWithHighestHitPoints(1,
                card => card.IsHero && !card.IsIncapacitatedOrOutOfGame, storedLowestHp);

            // Find hero with fewest cards in play
            List<TurnTaker> leastCardsInPlayResults = new List<TurnTaker>();
            IEnumerator heroWithLeastCardsInPlayRoutine = base.FindHeroWithMostCardsInPlay(leastCardsInPlayResults);

            // Find hero with most cards in play
            List<TurnTaker> mostCardsInPlayResults = new List<TurnTaker>();
            IEnumerator heroWithMostCardsInPlayRoutine = base.FindHeroWithMostCardsInPlay(mostCardsInPlayResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(getHighestHpRoutine);
                yield return base.GameController.StartCoroutine(getLowestHpRoutine);
                yield return base.GameController.StartCoroutine(heroWithLeastCardsInPlayRoutine);
                yield return base.GameController.StartCoroutine(heroWithMostCardsInPlayRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(getHighestHpRoutine);
                base.GameController.ExhaustCoroutine(getLowestHpRoutine);
                base.GameController.ExhaustCoroutine(heroWithLeastCardsInPlayRoutine);
                base.GameController.ExhaustCoroutine(heroWithMostCardsInPlayRoutine);
            }


        }
    }
}
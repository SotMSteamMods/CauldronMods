using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
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

        public static readonly string Identifier = "MarkOfTheWrithingNight";

        private const int DamageToDealHighestHp = 5;
        private const int DamageToDealLowestHp = 2;
        private const int CardsToDestroyFewestCards = 1;
        private const int CardsToDestroyMostCards = 2;

        public MarkOfTheWrithingNightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
            SpecialStringMaker.ShowSpecialString(ShowHeroWithFewestCardsInPlay);
            SpecialStringMaker.ShowHeroWithMostCards(false);
        }

        public override IEnumerator Play()
        {
            // Deal the hero with the highest HP 5 projectile damage
            IEnumerator dealDamageToHighestHpRoutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) =>  IsHeroCharacterCard(c), (Card c) => DamageToDealHighestHp, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageToHighestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageToHighestHpRoutine);
            }
            // Deal the hero with the lowest HP 2 irreducible infernal damage
            IEnumerator dealDamageToLowestHpRoutine = DealDamageToLowestHP(CharacterCard, 1, (Card c) =>  IsHeroCharacterCard(c), (Card c) => DamageToDealLowestHp, DamageType.Infernal, isIrreducible: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageToLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageToLowestHpRoutine);
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
                TurnTaker fewestTt = fewestCardsInPlayResults.First();
                HeroTurnTakerController fewestCardsHttc = FindHeroTurnTakerController(fewestTt.ToHero());

                IEnumerator destroyOneOngoingRoutine = base.GameController.SelectAndDestroyCards(fewestCardsHttc,
                    new LinqCardCriteria(card => IsOngoing(card) && card.IsInPlay && card.Owner == fewestTt), CardsToDestroyFewestCards, cardSource: this.GetCardSource());

                IEnumerator destroyOneEquipmentRoutine = base.GameController.SelectAndDestroyCards(fewestCardsHttc,
                    new LinqCardCriteria(card => IsEquipment(card) && card.IsInPlay && card.Owner == fewestTt), CardsToDestroyFewestCards, cardSource: this.GetCardSource());

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
                TurnTaker mostTt = mostCardsInPlayResults.First();
                HeroTurnTakerController mostCardsHttc = FindHeroTurnTakerController(mostTt.ToHero());

                IEnumerator destroyTwoOngoingRoutine = base.GameController.SelectAndDestroyCards(mostCardsHttc,
                    new LinqCardCriteria(card => IsOngoing(card) && card.IsInPlay && card.Owner == mostTt), CardsToDestroyMostCards, cardSource: this.GetCardSource());

                IEnumerator destroyTwoEquipmentRoutine = base.GameController.SelectAndDestroyCards(mostCardsHttc,
                    new LinqCardCriteria(card => IsEquipment(card) && card.IsInPlay && card.Owner == mostTt), CardsToDestroyMostCards, cardSource: this.GetCardSource());

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

        public string ShowHeroWithFewestCardsInPlay()
        {
        
            IEnumerable<TurnTaker> enumerable = GameController.FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame, BattleZone);
            List<string> list = new List<string>();
            int num = 999;
            foreach (HeroTurnTaker hero in enumerable)
            {
                IEnumerable<Card> cardsWhere = hero.GetCardsWhere((Card c) => c.IsInPlay && c.Location.OwnerTurnTaker == hero);
                List<Card> source = cardsWhere.ToList();
                if (source.Count() < num)
                {
                    list.RemoveAll((string htt) => true);
                    list.Add(hero.Name);
                    num = source.Count();
                }
                else if (source.Count() == num)
                {
                    list.Add(hero.Name);
                }
            }
            string text = list.Count().ToString_SingularOrPlural("Hero", "Heroes");
            string text2 = " in play";
            string text3 = " cards";
                
            return (list.Count() > 0) ? string.Format("{0} with the fewest{3}{2}: {1}.", text, list.ToRecursiveString(), text2, text3) : "Warning: No heroes found";
            
        }
    }
}
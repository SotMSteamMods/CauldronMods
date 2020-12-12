using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cypher
{
    public abstract class CypherBaseCardController : CardController
    {
        public CypherBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsAugment(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "augment");
        }

        protected bool IsAugmented(Card hero)
        {
            return hero.IsHeroCharacterCard && hero.IsInPlayAndHasGameText && !hero.IsIncapacitatedOrOutOfGame
                && hero.NextToLocation.HasCards && hero.GetAllNextToCards(false).Any(IsAugment);
        }

        protected List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && IsAugment(c)).ToList();
        }

        protected List<Card> GetAugmentedHeroCards()
        {
            return FindCardsWhere(c => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame
                && c.NextToLocation.HasCards && c.NextToLocation.Cards.Any(IsAugment)).ToList();
        }

        protected List<TurnTaker> GetAugmentedHeroTurnTakers()
        {
            return FindTurnTakersWhere(tt =>
                tt.IsHero && tt.CharacterCard.NextToLocation.HasCards &&
                tt.CharacterCard.NextToLocation.Cards.Any(IsAugment)).ToList();
        }

        protected List<Card> GetAugmentsForHero(Card hero)
        {
            return !IsAugmented(hero) ? new List<Card>() : hero.GetAllNextToCards(false).Where(IsAugment).ToList();
        }

        protected LinqCardCriteria AugmentedHeroes()
        {
            return new LinqCardCriteria(IsAugmented);
        }

        protected IEnumerator MoveAugment(Card card)
        {
            SelectCardDecision scd = new SelectCardDecision(GameController, DecisionMaker,
                SelectionType.MoveCardNextToCard, GetAugmentsInPlay(), true, cardSource: GetCardSource());
            scd.SelectedCard = card;

            return MoveAugment(scd);
        }

        protected IEnumerator MoveAugment(SelectCardDecision scd)
        {
            if (scd.SelectedCard == null)
            {
                yield break;
            }

            List<MoveCardDestination> otherHeroLocations = FindCardsWhere(c => c != scd.SelectedCard.Location.OwnerCard && c.IsHeroCharacterCard 
                                && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame).ToList()
                .Select(h => new MoveCardDestination(h.NextToLocation, showMessage: true)).ToList();

            IEnumerator routine = GameController.SelectLocationAndMoveCard(this.DecisionMaker, scd.SelectedCard,
                otherHeroLocations, cardSource: GetCardSource());
            
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
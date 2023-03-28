using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class HarkinParishJrCardController : CatchwaterHarborUtilityCardController
    {
        public HarkinParishJrCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} melee damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, Game.H, DamageType.Melee);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, the hero with the highest HP must discard a card. Each other player must discard a card that shares a keyword with that card.
            List<Card> storeHighest = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => IsHeroCharacterCard(c)&& !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storeHighest, cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            if (storeHighest != null && storeHighest.Count > 0)
            {
                Card highestHero = storeHighest.First();
                if (storeHighest.Count > 1)
                {

                    List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                    coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DiscardCard, new LinqCardCriteria((Card c) => storeHighest.Contains(c)), storedResults, false, cardSource: GetCardSource());
                    if (this.UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (DidSelectCard(storedResults))
                    {
                        highestHero = GetSelectedCard(storedResults);
                    }
                }

                //the hero with the highest HP must discard a card
                HeroTurnTakerController httc = FindHeroTurnTakerController(highestHero.Owner.ToHero());
                List<DiscardCardAction> storedDiscard = new List<DiscardCardAction>();
                coroutine = GameController.SelectAndDiscardCard(httc, storedResults: storedDiscard, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }

                //Each other player must discard a card that shares a keyword with that card.
                if (DidDiscardCards(storedDiscard))
                {
                    Card discardedCard = storedDiscard.First().CardToDiscard;
                    List<Card> list = new List<Card>();
                    list.Add(discardedCard);
                    IEnumerable<string> keywords = GameController.GetAllKeywords(discardedCard);
                    Func<Card, bool> cardCriteria = (Card c) => GameController.GetAllKeywords(c).Intersect(keywords).Any();
                    var ttCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && tt != httc.TurnTaker && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()));
                    coroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, ttCriteria, SelectionType.DiscardCard, (TurnTaker tt) => GameController.SelectAndDiscardCard(FindHeroTurnTakerController(tt.ToHero()), additionalCriteria: cardCriteria, cardSource: GetCardSource()), allowAutoDecide: true, associatedCards: list, cardSource: GetCardSource());
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
            yield break;
        }


    }
}

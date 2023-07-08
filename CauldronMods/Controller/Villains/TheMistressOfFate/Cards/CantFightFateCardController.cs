using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class CantFightFateCardController : TheMistressOfFateUtilityCardController
    {
        public CantFightFateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //"The hero with the highest HP...",
            var storedHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c), storedHero, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var heroCard = storedHero.FirstOrDefault();

            if(heroCard == null)
            {
                yield break;
            }


            //"...may discard 3 cards that share a keyword."
            var hero = heroCard.Owner.ToHero();
            var heroTTC = FindHeroTurnTakerController(hero);
            var keywordsInHand = hero.Hand.Cards.SelectMany((Card c) => GameController.GetAllKeywords(c)).Distinct().OrderBy(s => s);
            var viableKeywords = keywordsInHand.Where((string keyword) => hero.Hand.Cards.Count((Card c) => GameController.DoesCardContainKeyword(c, keyword)) >= 3);

            bool takingDamage = true;

            if (viableKeywords.Any())
            {
                List<SelectWordDecision> storedKeyword = new List<SelectWordDecision>();
                coroutine = GameController.SelectWord(heroTTC, viableKeywords, SelectionType.DiscardCard, storedKeyword, true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(DidSelectWord(storedKeyword))
                {
                    string selectedKeyword = GetSelectedWord(storedKeyword);
                    List<DiscardCardAction> storedDiscard = new List<DiscardCardAction>();
                    coroutine = GameController.SelectAndDiscardCards(heroTTC, 3, false, 3, storedDiscard, cardCriteria: new LinqCardCriteria((Card c) => GameController.DoesCardContainKeyword(c, selectedKeyword), selectedKeyword), cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    if(DidDiscardCards(storedDiscard, 3))
                    {
                        takingDamage = false;
                    }
                }
            }
            else
            {
                coroutine = GameController.SendMessageAction($"{hero.Name} does not have 3 cards that share a keyword in their hand!", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"If they do not, {TheMistressOfFate} deals each target in that hero's play area 20 psychic damage."
            if(takingDamage)
            {
                coroutine = DealDamage(CharacterCard, (Card c) => c.Location.HighestRecursiveLocation == hero.PlayArea, 20, DamageType.Psychic);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}

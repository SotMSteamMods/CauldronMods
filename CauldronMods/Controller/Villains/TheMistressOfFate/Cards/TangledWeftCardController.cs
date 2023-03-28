using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class TangledWeftCardController : TheMistressOfFateUtilityCardController
    {
        public TangledWeftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => NumFaceUpDays() == 1, () => $"There is 1 face up day card.", () => $"There are {NumFaceUpDays()} face up day cards.");
        }

        public override IEnumerator Play()
        {
            var failingHeroes = new List<TurnTaker>();
            //"Each player may discard a card that shares a keyword with at least one of their cards in play.",
            var selectTurnTakers = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame), SelectionType.DiscardCard, allowAutoDecide: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectTurnTakers, tt => DiscardMatchingCard(tt, failingHeroes), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"Then, {TheMistressOfFate} deals each hero that did not discard a card this way 5 infernal damage for each face up Day card."
            var numFaceUpDays = NumFaceUpDays();

            if (numFaceUpDays == 1)
            {
                coroutine = DealDamage(CharacterCard, (Card c) =>  IsHeroCharacterCard(c) && failingHeroes.Contains(c.Owner), 5, DamageType.Infernal);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else if (numFaceUpDays > 1)
            {
                var damages = new List<DealDamageAction>();
                for (int i = 0; i < numFaceUpDays; i++)
                {
                    damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 5, DamageType.Infernal));
                }
                
                coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(damages, (Card c) =>  IsHeroCharacterCard(c) && failingHeroes.Contains(c.Owner));
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

        private IEnumerator DiscardMatchingCard(TurnTaker hero, List<TurnTaker> storedResults)
        {
            var heroTTC = FindHeroTurnTakerController(hero.ToHero());
            if(heroTTC != null)
            {
                var storedDiscards = new List<DiscardCardAction>();
                var keywords = hero.GetCardsWhere((Card c) => c.IsInPlay).SelectMany((Card c) => GameController.GetAllKeywords(c)).Distinct();
                IEnumerator coroutine = SelectAndDiscardCards(heroTTC, 1, false, 0, storedDiscards, cardCriteria: new LinqCardCriteria((Card c) => GameController.GetAllKeywords(c).Any(key => keywords.Contains(key)), "matching card"));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(!DidDiscardCards(storedDiscards))
                {
                    storedResults.Add(heroTTC.TurnTaker);
                }
            }
            yield break;
        }

        private int NumFaceUpDays()
        {
            return GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsDay(c)).Count();
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class FadingRealitiesCardController : TheMistressOfFateUtilityCardController
    {
        public FadingRealitiesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //"The hero with the second lowest HP...
            var storedHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(2, (Card c) => c.IsHeroCharacterCard, storedHero, cardSource: GetCardSource());
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

            var hero = heroCard.Owner.ToHero();
            var heroTTC = FindTurnTakerController(hero).ToHero();

            int numCardsInPlay = hero.GetCardsWhere((Card c) => c.IsInPlay && c.IsRealCard && !c.IsCharacter).Count();

            bool skipSelection = false;

            if(numCardsInPlay == 0 || !hero.HasCardsInHand)
            {
                var storedYesNo = new List<YesNoCardDecision>();
                if(numCardsInPlay == 0 && !hero.HasCardsInHand)
                {
                    coroutine = GameController.SendMessageAction($"{hero.Name} has no cards in their hand or in play to select!", Priority.Medium, GetCardSource());
                }
                else if(!hero.HasCardsInHand)
                {
                    //If there were a more generic "select cards to no effect" I would use it here
                    coroutine = GameController.MakeYesNoCardDecision(heroTTC, SelectionType.DiscardCardsToNoEffect, Card, storedResults: storedYesNo, cardSource: GetCardSource());
                }
                else //numCardsInPlay == 0
                {
                    coroutine = GameController.MakeYesNoCardDecision(heroTTC, SelectionType.DiscardCardsToNoEffect, Card, storedResults: storedYesNo, cardSource: GetCardSource());
                }

                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(DidPlayerAnswerYes(storedYesNo))
                {
                    skipSelection = false;
                }
                else
                {
                    skipSelection = true;
                }
            }

            var removedCards = new List<Card>();
            if(!skipSelection)
            {
                //...may select 1 card in their hand and 1 of their cards in play. Remove the selected cards from the game."
                var fromHandSelection = new SelectCardDecision(GameController, heroTTC, SelectionType.RemoveCardFromGame, hero.Hand.Cards, true, cardSource: GetCardSource());
                coroutine = GameController.SelectCardAndDoAction(fromHandSelection, scd => RemoveCardFromGame(scd, removedCards));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                var cardsInPlay = hero.GetCardsWhere((Card c) => c.IsInPlay && c.IsRealCard && !c.IsCharacter);
                var fromPlaySelection = new SelectCardDecision(GameController, heroTTC, SelectionType.RemoveCardFromGame, cardsInPlay, true, cardSource: GetCardSource());
                coroutine = GameController.SelectCardAndDoAction(fromPlaySelection, scd => RemoveCardFromGame(scd, removedCards));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

            }

            //"If 2 cards were not removed from the game this way, {TheMistressOfFate} deals that hero 20 infernal damage."
            if(removedCards.Count() < 2)
            {
                coroutine = DealDamage(CharacterCard, heroCard, 20, DamageType.Infernal, cardSource: GetCardSource());
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

        private IEnumerator RemoveCardFromGame(SelectCardDecision scd, List<Card> removalStorage)
        {
            var card = scd.SelectedCard;
            if(card != null)
            {
                var storedResults = new List<MoveCardAction>();
                IEnumerator coroutine = GameController.MoveCard(DecisionMaker, card, card.Owner.OutOfGame, storedResults: storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(DidMoveCard(storedResults) && card.Location.IsOutOfGame)
                {
                    removalStorage.Add(card);
                }
            }
            yield break;
        }
    }
}

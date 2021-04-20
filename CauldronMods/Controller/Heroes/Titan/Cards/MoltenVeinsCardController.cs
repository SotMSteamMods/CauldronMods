using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class MoltenVeinsCardController : TitanCardController
    {
        public MoltenVeinsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private readonly string TitanformIdentifier = "Titanform";


        public override IEnumerator Play()
        {
            //{Titan} regains 2HP.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, 2, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may search your deck and trash for a copy of the card Titanform and put it into your hand. If you searched your deck, shuffle it.
            coroutine = SearchForTitanform();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            //You may play a card.
            coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        protected IEnumerator SearchForTitanform()
        {
            if (base.TurnTaker.Deck.Cards.Any((Card c) => c.Identifier == TitanformIdentifier) || base.TurnTaker.Trash.Cards.Any((Card c) => c.Identifier == TitanformIdentifier))
            {
                // Search Titan's deck and trash for a copy of Titanform..
                IEnumerable<Card> originDeck = TurnTaker.Deck.Cards;
                IEnumerable<Card> choices = TurnTaker.Deck.Cards.Concat(TurnTaker.Trash.Cards);
                LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.Identifier == TitanformIdentifier, "titanform", useCardsSuffix: false);
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                IEnumerator coroutine = GameController.SelectAndMoveCard(base.DecisionMaker, (Card c) =>  choices.Contains(c) && c.Identifier == TitanformIdentifier, toLocation: HeroTurnTaker.Hand, optional: true, storedResults: storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (DidSelectCard(storedResults))
                {
                    Card selectedCard = GetSelectedCard(storedResults);
                    if (originDeck.Contains(selectedCard) && selectedCard.Location == HeroTurnTaker.Hand)
                    {
                        // then shuffle the deck...
                        IEnumerator coroutine3 = base.ShuffleDeck(base.DecisionMaker, base.TurnTaker.Deck);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                    }
                }

            }
            else
            {
                IEnumerator coroutine2 = base.GameController.SendMessageAction("There are no copies of Titanform in the deck or trash!", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            yield break;
        }
    }
}
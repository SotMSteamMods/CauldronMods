using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class YourDarkestSecretsCardController : MythosUtilityCardController
    {
        public YourDarkestSecretsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Discard the top card of each hero deck.
            List<MoveCardAction> discardedCards = new List<MoveCardAction>();
            IEnumerator coroutine = base.GameController.DiscardTopCardsOfDecks(this.DecisionMaker, (Location deck) => deck.IsHero, 1, storedResultsMove: discardedCards, showCards: (Card c) => true, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            foreach (MoveCardAction cardAction in discardedCards)
            {
                //{Mythos} deals each hero 1 infernal damage for each card in their hand that shares a keyword with the card discarded from their deck.
                Card card = cardAction.CardToMove;
                var heroHand = card.Owner.ToHero().Hand.Cards;
                int consideredCardCount = 0;

                Func<Card, bool> sharesKeywordWithDiscarded = delegate (Card c)
                {
                    return base.GameController.GetAllKeywords(c).Intersect(base.GameController.GetAllKeywords(card)).Any();
                };

                while(consideredCardCount < heroHand.Count((Card handCard) => sharesKeywordWithDiscarded(handCard)))
                {
                    consideredCardCount += 1;
                    Card target;
                    List<Card> storedResults = new List<Card>();
                    coroutine = FindCharacterCardToTakeDamage(card.Owner, storedResults, CharacterCard, 1, DamageType.Infernal);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    target = storedResults.FirstOrDefault();

                    if (target != null)
                    {
                        coroutine = base.DealDamage(base.CharacterCard, target, 1, DamageType.Infernal, cardSource: base.GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                }
            }
            yield break;
        }
    }
}

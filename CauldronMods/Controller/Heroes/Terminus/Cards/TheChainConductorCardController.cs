using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class TheChainConductorCardController : TerminusBaseCardController
    {
        /* 
         * Reveal cards from the top of your deck until you reveal a Memento or Equipment card. Put it into play or into 
         * your hand. Shuffle the rest of the revealed cards into your deck. 
         * If no card entered play this way, add 3 tokens to your Wrath pool and {Terminus} deals 1 target 2 cold damage.
         */
        public TheChainConductorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override IEnumerator Play()
        {
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            IEnumerator revealCardsRoutine;
            IEnumerator returnCardsRoutine;
            IEnumerator coroutine;
            Card matchedCard;
            List<Card> otherCards;
            List<Function> playOrHandList = new List<Function>();
            List<PlayCardAction> playCardActions = new List<PlayCardAction>();
            SelectFunctionDecision selectFunction;

            // Reveal cards from the top of your deck until you reveal a Memento or Equipment card.        
            revealCardsRoutine = base.GameController.RevealCards(DecisionMaker, base.TurnTaker.Deck, card => base.IsEquipment(card) || card.DoKeywordsContain("memento"), 1, revealedCards, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            matchedCard = GetRevealedCards(revealedCards).FirstOrDefault(card => base.IsEquipment(card) || card.DoKeywordsContain("memento"));
            if (matchedCard != null)
            {
                // Put it into play or into  your hand
                playOrHandList.Add(new Function(DecisionMaker, "Put into play", SelectionType.PutIntoPlay, () => base.GameController.PlayCard(DecisionMaker, matchedCard, storedResults: playCardActions, cardSource: base.GetCardSource())));
                playOrHandList.Add(new Function(DecisionMaker, "Put into hand", SelectionType.MoveCardToHand, () => base.GameController.MoveCard(DecisionMaker, matchedCard, base.HeroTurnTaker.Hand, cardSource:base.GetCardSource())));
                selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, playOrHandList, false, null, null, null, FindCardController(matchedCard).GetCardSource());
                coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                // If no card entered play this way, 
                if (!DidPlayCards(playCardActions))
                {
                    // add 3 tokens to your Wrath pool 
                    coroutine = base.AddWrathTokens(3); 
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    // and {Terminus} deals 1 target 2 cold damage.
                    coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Cold, 1, false, 1, cardSource: base.GetCardSource());
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

            otherCards = GetRevealedCards(revealedCards).Where(card => !(base.IsEquipment(card) || card.DoKeywordsContain("memento"))).ToList();
            if (otherCards.Any())
            {
                // Shuffle the rest of the revealed cards into your deck. 
                returnCardsRoutine = base.GameController.MoveCards(DecisionMaker, otherCards, base.HeroTurnTaker.Deck, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(returnCardsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(returnCardsRoutine);
                }

                coroutine = base.ShuffleDeck(DecisionMaker, base.HeroTurnTaker.Deck);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(returnCardsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(returnCardsRoutine);
                }
            }
        }
    }
}

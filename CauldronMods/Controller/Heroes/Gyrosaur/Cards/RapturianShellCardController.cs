using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RapturianShellCardController : GyrosaurUtilityCardController
    {
        public RapturianShellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowCrashInHandCount();
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, if you have 0 Crash cards in your hand, {Gyrosaur} deals 1 other hero 2 psychic damage.",
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnNoCrashSadnessResponse, TriggerType.DealDamage);
        }
        private IEnumerator EndOfTurnNoCrashSadnessResponse(PhaseChangeAction pc)
        {
            //If you have 0 Crash cards in your hand...
            var storedModifier = new List<int>();
            IEnumerator coroutine = EvaluateCrashInHand(storedModifier, () => TrueCrashInHand <= 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int crashMod = storedModifier.FirstOrDefault();

            if (TrueCrashInHand + crashMod <= 0)
            {
                //{Gyrosaur} deals 1 other hero 2 psychic damage.
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Psychic, 1, false, 1, additionalCriteria: (Card c) =>  IsHeroCharacterCard(c) && c != CharacterCard, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Play a Crash card, or discard cards from the top of your deck until you discard a Crash card and put it into your hand."
            var functions = new List<Function>
            {
                new Function(DecisionMaker, "Play a Crash card", SelectionType.PlayCard, PlayCrashCard, onlyDisplayIfTrue: HeroTurnTaker.Hand.Cards.Any((Card c) => IsCrash(c) && GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay)),
                new Function(DecisionMaker, "Discard cards from your deck until you discard a Crash card and put it in your hand", SelectionType.DiscardFromDeck, DiscardUntilFindCrashCard, forcedActionMessage: $"{TurnTaker.Name} cannot play a crash card, so they will discard cards until they find a crash card.")
            };

            var functionDecision = new SelectFunctionDecision(GameController, DecisionMaker, functions, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(functionDecision);
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

        private IEnumerator PlayCrashCard()
        {
            IEnumerator coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, false, cardCriteria: new LinqCardCriteria((Card c) => IsCrash(c), "crash"), cardSource: GetCardSource());
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
        private IEnumerator DiscardUntilFindCrashCard()
        {
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            IEnumerator coroutine = GameController.RevealCards(DecisionMaker, TurnTaker.Deck, (Card c) => IsCrash(c), 1, revealedCards, RevealedCardDisplay.ShowMatchingCards, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(revealedCards.FirstOrDefault() == null)
            {
                yield break;
            }

            //discards first, in case it matters (eg. Team La Capitan's Chiquito)
            coroutine = GameController.MoveCards(DecisionMaker, revealedCards.FirstOrDefault().NonMatchingCards, DecisionMaker.TurnTaker.Trash, isDiscard: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var matchingCard = revealedCards.FirstOrDefault()?.MatchingCards.FirstOrDefault();
            if(matchingCard != null)
            {
                coroutine = GameController.MoveCard(DecisionMaker, matchingCard, HeroTurnTaker.Hand, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = GameController.SendMessageAction($"There were no crash cards in {DecisionMaker.Name}'s deck.", Priority.Medium, GetCardSource());
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class IllusionOfFreeWillCardController : TheMistressOfFateUtilityCardController
    {
        public IllusionOfFreeWillCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP().Condition = () => !Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck);
            _isStoredCard = false;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to the hero with the lowest HP.
            var storedHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c), storedHero, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            var hero = storedHero.FirstOrDefault();
            if(hero != null)
            {
                storedResults?.Add(new MoveCardDestination(hero.NextToLocation));
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //Destroy this card if {TheMistressOfFate} flips or there are ever less than 5 cards in the villain deck.",
            AddTrigger((FlipCardAction fc) => fc.CardToFlip?.Card == CharacterCard, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
            AddTrigger((GameAction ga) => (IsDeckEmptier(ga) || ga is CompletedCardPlayAction) && TurnTaker.Deck.NumberOfCards < 5, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);

            //"Redirect damage dealt by this hero’s cards to the hero with the highest HP. 
            AddTrigger((DealDamageAction dd) => dd.IsRedirectable && dd.CardSource != null && dd.CardSource.TurnTakerController.TurnTaker == GetCardThisCardIsNextTo()?.Owner,
                            (DealDamageAction dd) => RedirectDamage(dd, TargetType.HighestHP, (Card c) =>  IsHeroCharacterCard(c)),
                            TriggerType.RedirectDamage,
                            TriggerTiming.Before);

            //At the end of this hero's turn, play the top 2 cards of their deck."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == GetCardThisCardIsNextTo()?.Owner, PlayCardsResponse, TriggerType.PlayCard);
        }

        private IEnumerator PlayCardsResponse(PhaseChangeAction pc)
        {
            var deck = GetCardThisCardIsNextTo()?.Owner.Deck;
            if (deck != null)
            {
                IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} plays the top 2 cards of {deck.GetFriendlyName()}.", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.PlayTopCardOfLocation(DecisionMaker, deck, numberOfCards: 2, requiredNumberOfCards: 2, cardSource: GetCardSource(), showMessage: true);
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

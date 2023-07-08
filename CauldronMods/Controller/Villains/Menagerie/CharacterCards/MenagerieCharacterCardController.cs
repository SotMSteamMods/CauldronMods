using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class MenagerieCharacterCardController : VillainCharacterCardController
    {
        public MenagerieCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                return !base.Card.IsFlipped;
            }
        }

        private bool IsEnclosure(Card c)
        {
            return c.DoKeywordsContain("enclosure", true, true);
        }

        public bool IsCaptured(TurnTaker tt)
        {
            Card prize = FindCard("PrizedCatch");
            return prize != null && prize.Location.IsNextToCard && tt.GetAllCards().Contains(prize.Location.OwnerCard);
        }

        public bool HasEnclosure(TurnTaker tt)
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsEnclosure(c) && tt == c.Location.OwnerTurnTaker)).Any();
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            { //Front
              //Cards beneath villain cards are not considered in play. When an enclosure leaves play, put it under this card... 
                base.Card.UnderLocation.OverrideIsInPlay = false;

                base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => this.IsEnclosure(action.CardToDestroy.Card) && action.PostDestroyDestinationCanBeChanged && action.CardToDestroy.CanBeDestroyed && action.WasCardDestroyed, this.PutUnderThisCardResponse, TriggerType.MoveCard, TriggerTiming.Before));
                base.AddSideTrigger(base.AddTrigger<MoveCardAction>((MoveCardAction action) => this.IsEnclosure(action.CardToMove) && action.CanChangeDestination && action.Origin.IsInPlayAndNotUnderCard, this.PutUnderThisCardResponse, TriggerType.MoveCard, TriggerTiming.Before));
                //...discarding all cards beneath it. Put any discarded targets into play.
                /**logic added to enclosures**/

                //At the end of the villain turn, reveal cards from the top of the villain deck until an enclosure is revealed, play it, and shuffle the other revealed cards back into the deck. Then if {H} enclosures are beneath this card, flip {Menagerie}'s character cards.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.PlayEnclosureMaybeFlipResponse, new TriggerType[] { TriggerType.PlayCard }));

                //Prized Catch is Indestructible. The heroes lose if the captured hero is incapacitated.
                /**This is on Prized Catch**/
                if (base.Game.IsAdvanced)
                { //Front - Advanced
                    //When an enclosure enters play, put the top card of the villain deck beneath it.
                    base.AddSideTrigger(base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => action.IsSuccessful && this.IsEnclosure(action.CardEnteringPlay), this.EncloseExtraCard, TriggerType.MoveCard, TriggerTiming.After));
                }
            }
            else
            { //Back
              //When an enclosure enters play, move it next to the active hero with the fewest enclosures in their play area. Heroes with enclosures in their play area may not damage cards in other play areas.
                /**logic added to enclosures**/

                //Cards beneath enclosures are not considered in play. When an enclosure leaves play, discard all cards beneath it.
                /**logic added to enclosures**/
                base.Card.UnderLocation.OverrideIsInPlay = null;

                //At the end of the villain turn, play the top card of the villain deck. Then, for each enclosure in play, Menagerie deals the hero next to it X projectile damage, where X is the number of cards beneath that enclosure.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.PlayCardDealDamageResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.DealDamage }));
                if (base.Game.IsAdvanced)
                { //Back - Advanced
                    //At the start of the villain turn, if each active hero has an enclosure in their play area, the heroes lose the game.
                    base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.MaybeGameOverResponse, TriggerType.GameOver));
                }
            }

            if(Game.IsChallenge)
            {
                //At the end of the villain turn, the captured hero and each hero next to an Enclosure deals themself X irreducible psychic damage, where X is the number of Enclosures in play.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.ChallengeEndOfTurnResponse, new TriggerType[] { TriggerType.DealDamage }));
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator ChallengeEndOfTurnResponse(PhaseChangeAction arg)
        {
            //the captured hero and each hero next to an Enclosure deals themself X irreducible psychic damage, where X is the number of Enclosures in play.
            int X = FindCardsWhere(c => IsEnclosure(c) && c.IsInPlayAndHasGameText).Count();
            IEnumerator coroutine = GameController.DealDamageToSelf(DecisionMaker, (Card c) => IsHero(c.Owner)  &&  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && (IsCaptured(c.Owner) || HasEnclosure(c.Owner)), X, DamageType.Psychic, isIrreducible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator PutUnderThisCardResponse(DestroyCardAction action)
        {
            action.SetPostDestroyDestination(base.Card.UnderLocation, cardSource: base.GetCardSource());
            yield break;
        }

        private IEnumerator PutUnderThisCardResponse(MoveCardAction action)
        {
            action.SetDestination(base.Card.UnderLocation);
            yield break;
        }

        private IEnumerator PlayEnclosureMaybeFlipResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine;
            if (base.FindCardsWhere((new LinqCardCriteria((Card c) => this.IsEnclosure(c) && c.IsInDeck))).Any())
            {
                //...reveal cards from the top of the villain deck until an enclosure is revealed, play it, and shuffle the other revealed cards back into the deck.
                var playStorage = new List<Card>();
                coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => this.IsEnclosure(c), "enclosure"), 1, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards, storedPlayResults: playStorage, shuffleReturnedCards: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(playStorage.Any((Card c) => c.Location.IsRevealed))
                {
                    var enclosure = playStorage.FirstOrDefault();
                    coroutine = GameController.SendMessageAction($"Menagerie shuffles {enclosure.Title} into her deck.", Priority.Medium, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.MoveCard(TurnTakerController, enclosure, TurnTaker.Deck, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
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

            var enclosureCount = base.FindCardsWhere((new LinqCardCriteria((Card c) => this.IsEnclosure(c) && c.Location == base.Card.UnderLocation))).Count();
            //Then if {H} enclosures are beneath this card, flip {Menagerie}'s character cards.
            if (enclosureCount >= Game.H)
            {
                coroutine = base.FlipThisCharacterCardResponse(action);
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

        private IEnumerator EncloseExtraCard(CardEntersPlayAction action)
        {
            //...enclosure enters play, put the top card of the villain deck beneath it.
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, action.CardEnteringPlay.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource());
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

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.MakeTargettable(base.Card, base.Card.Definition.FlippedHitPoints.Value, base.Card.Definition.FlippedHitPoints.Value, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //When Menagerie flips to this side, shuffle the villain trash and all enclosurese beneath this card into the villain deck. 
            coroutine = GameController.SendMessageAction($"{TurnTaker.Name} shuffles the villain trash and all enclosures beneath her into the villain deck.", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.BulkMoveCards(base.TurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => (this.IsEnclosure(c) && c.Location == base.Card.UnderLocation) || c.Location == base.TurnTaker.Trash)), base.TurnTaker.Deck, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Remove Prized Catch from the game.
            coroutine = base.GameController.MoveCard(base.TurnTakerController, base.FindCard("PrizedCatch"), base.TurnTaker.OutOfGame, cardSource: base.GetCardSource());
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

        private IEnumerator PlayCardDealDamageResponse(PhaseChangeAction action)
        {
            //...play the top card of the villain deck. 
            IEnumerator coroutine = base.PlayTheTopCardOfTheVillainDeckResponse(action);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, for each enclosure in play, Menagerie deals the hero next to it X projectile damage, where X is the number of cards beneath that enclosure.
            IEnumerable<Card> enclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsEnclosure(c) && c.IsInPlayAndHasGameText && c.Location.IsHeroPlayAreaRecursive));
            foreach (Card enclosure in enclosures)
            {
                coroutine = base.DealDamage(base.Card, (Card c) => c == enclosure.Location.OwnerCard, (Card c) => enclosure.UnderLocation.NumberOfCards, DamageType.Projectile);
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

        private IEnumerator MaybeGameOverResponse(PhaseChangeAction action)
        {
            //...if each active hero has an enclosure in their play area, the heroes lose the game.
            int capturedHeroes = 0;
            IEnumerable<Card> enclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => this.IsEnclosure(c)));
            foreach (TurnTaker hero in base.Game.HeroTurnTakers)
            {
                foreach (Card enclosure in enclosures)
                {
                    if (enclosure.Location.OwnerTurnTaker == hero)
                    {
                        capturedHeroes++;
                        break;
                    }
                }
            }
            if (capturedHeroes == Game.H)
            {
                IEnumerator coroutine = base.GameController.GameOver(EndingResult.AlternateDefeat, "Menagerie has captured all of the heroes!", cardSource: base.GetCardSource());
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    // This action exists so effects that trigger when the top card of a deck is face up are "aware" that
    // Radio Plaza has flipped the triggering card face up. It should have no effect in game.

    // Both extant examples (Ambuscade and Johnny Rocket) listen to ShuffleCardsAction,
    // so we can create a fake one that doesn't actually shuffle the deck but allows triggers to respond.
    public class FakeShuffleCardsAction : ShuffleCardsAction
    {
        public FakeShuffleCardsAction(CardSource cardSource, Location location)
            : base(cardSource, location)
        {
            ShowOutput = false;
        }
        protected override IEnumerator DoActionOnSuccess()
        {
            yield break;
        }
    }
    public class RadioPlazaCardController : CatchwaterHarborUtilityCardController
    {
        public RadioPlazaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"The top card of {TurnTaker.Deck.GetFriendlyName()} is {TurnTaker.Deck.TopCard.Title}.").Condition = () => Card.IsInPlayAndHasGameText && TurnTaker.Deck.HasCards;
        }

        public override void AddStartOfGameTriggers()
        {
            BuildTopDeckSpecialStrings();
        }

        private IEnumerator FlipOverCards()
        {
            var decks = FindLocationsWhere(l => l.IsDeck && l.BattleZone == BattleZone);
            foreach (var deck in decks)
            {
                if (
                    GameController.IsTurnTakerVisibleToCardSource(deck.OwnerTurnTaker, GetCardSource()) &&
                    deck.NumberOfCards > 0 &&
                    ! (deck.TopCard.IsFaceUp && deck.TopCard.IsPositionKnown)
                )
                {
                    deck.TopCard.SetFaceUp(true);
                    deck.TopCard.SetIsPositionKnown(true);
                    var fsa = new FakeShuffleCardsAction(GetCardSource(), deck);
                    var coroutine = GameController.DoAction(fsa);
                    if (base.UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }

        private void BuildTopDeckSpecialStrings()
        {
            //this needs to be all turntakers in all zones.
            IEnumerable<TurnTaker> activeTurnTakers =  FindTurnTakersWhere((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && !tt.IsEnvironment, true);
            foreach(TurnTaker tt in activeTurnTakers)
            {
                foreach(Location deck in tt.Decks.Where(deck => deck.IsRealDeck))
                {
                    var ss = SpecialStringMaker.ShowSpecialString(() => $"The top card of {deck.GetFriendlyName()} is {deck.TopCard.Title}.", relatedCards: () => tt.CharacterCards.Where(c => c.IsInPlayAndHasGameText));
                    ss.Condition = () => Card.IsInPlayAndHasGameText && deck.HasCards && GameController.IsLocationVisibleToSource(deck, GetCardSource());
                }
            }
        }

        public override void AddTriggers()
        {
            AddTrigger<GameAction>(ga => ga.CardSource != GetCardSource(), (a) => FlipOverCards(), TriggerType.Hidden, TriggerTiming.After);

            //Damage dealt to hero targets is irreducible.
            AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => IsHeroTarget(dd.Target) && GameController.IsCardVisibleToCardSource(dd.Target, GetCardSource()));
            //At the start of the environment turn, destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            return FlipOverCards();
        }
    }
}

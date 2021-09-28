using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class TheBlackTreeCardController : CardController
    {
        //==============================================================
        // When this card enters play, place the top card of each hero
        // and villain deck face-down beneath it.
        // At the end of the environment turn, play a random card from
        // beneath this one. Then if there are no cards remaining, this card is destroyed.
        // When this card is destroyed, discard any remaining cards beneath it.
        //==============================================================

        public static readonly string Identifier = "TheBlackTree";

        public TheBlackTreeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsUnderCard(Card);
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, play a random card from beneath this one.
            // Then if there are no cards remaining, this card is destroyed.
            AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnPlayCardBeneathResponse, TriggerType.PlayCard);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            List<Location> decks = new List<Location>();
            foreach (TurnTaker tt in Game.TurnTakers)
            {
                if (tt.IsIncapacitatedOrOutOfGame) continue;
                if (tt.Deck.IsRealDeck && GameController.IsLocationVisibleToSource(tt.Deck, GetCardSource()) && (tt.Deck.IsHero || tt.Deck.IsVillain))
                {
                    decks.Add(tt.Deck);
                }
                decks = decks.Concat(tt.SubDecks.Where(l => l.IsRealDeck && GameController.IsLocationVisibleToSource(l, GetCardSource()) && (l.IsHero || l.IsVillain))).ToList();
            }
            IEnumerator coroutine = MoveCardsFromDecksResponse(decks);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator MoveCardsFromDecksResponse(List<Location> decks)
        {
            foreach (Location deck in decks)
            {
                Card top = deck.TopCard;
                if (top != null)
                {
                    var coroutine = GameController.FlipCard(FindCardController(top), cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.MoveCard(TurnTakerController, top, Card.UnderLocation, false, false, false, null, false, null, base.TurnTaker,
                        cardSource: GetCardSource());
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
        }

        private IEnumerator EndOfTurnPlayCardBeneathResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            // Play a random card from beneath
            // Using Random() here will desync multiplayer, always use Game.RNG
            var enumerable = this.Card.UnderLocation.Cards.ToList();
            if (enumerable.Count() > 0)
            {
                Card cardToPlay = enumerable.ElementAt(Game.RNG.Next(0, enumerable.Count()));

                coroutine = GameController.MoveCard(TurnTakerController, cardToPlay, cardToPlay.Owner.Revealed, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SendMessageAction($"{Card.Title} plays {cardToPlay.Title} from beneath it.", Priority.High, GetCardSource(), new[] { cardToPlay });
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayCard(TurnTakerController, cardToPlay, reassignPlayIndex: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (!Card.UnderLocation.Cards.Any())
            {
                // No cards left underneath, destroy this card
                coroutine = base.GameController.DestroyCard(HeroTurnTakerController, Card, cardSource: GetCardSource());
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
    }
}
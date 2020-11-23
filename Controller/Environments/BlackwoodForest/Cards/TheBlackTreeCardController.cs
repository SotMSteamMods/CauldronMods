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

        public static string Identifier = "TheBlackTree";

        private const int CardsToDrawFromEachDeck = 1;

        public TheBlackTreeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, play a random card from beneath this one.
            // Then if there are no cards remaining, this card is destroyed.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnPlayCardBeneathResponse,
                TriggerType.PlayCard, null);


            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator drawCardsRoutine = base.DoActionToEachTurnTakerInTurnOrder(
                ttc => !ttc.Equals(this.TurnTakerController) && !ttc.IsIncapacitatedOrOutOfGame,
                DrawCardFromEachDeckResponse);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardsRoutine);
            }
        }

        private IEnumerator DrawCardFromEachDeckResponse(TurnTakerController ttc)
        {
            List<Card> revealedCards = new List<Card>();
            IEnumerator revealCardRoutine = this.GameController.RevealCards(ttc, ttc.CharacterCard.Owner.Deck, 
                CardsToDrawFromEachDeck, revealedCards);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardRoutine);
            }

            //IEnumerator bulkMoveRoutine = base.GameController.BulkMoveCards(this.TurnTakerController, revealedCards, this.Card.UnderLocation);
            foreach (var moveCardsRoutine in revealedCards.Select(revealedCard 
                => this.GameController.MoveCard(ttc, revealedCard, this.Card.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource())))
            {
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(moveCardsRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(moveCardsRoutine);
                }
            }
        }

        private IEnumerator EndOfTurnPlayCardBeneathResponse(PhaseChangeAction pca)
        {
            // Play a random card from beneath
            var r = Game.RNG; // Using Random() here will desync multiplayer, always use Game.RNG
            Card[] enumerable = this.Card.UnderLocation.Cards as Card[] ?? this.Card.UnderLocation.Cards.ToArray();
            Card cardToPlay = enumerable.ElementAt(r.Next(0, enumerable.Count()));


            IEnumerator routine = base.GameController.MoveCard(base.TurnTakerController, cardToPlay, cardToPlay.Owner.Revealed, false, 
                false, true, null, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            routine = base.GameController.PlayCard(base.TurnTakerController, cardToPlay, true, null, false, 
                null, null, false, null, cardSource: base.GetCardSource());
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (AreCardsRemainingBelowSelf())
            {
                // Still cards remaining underneath, return
                yield break;
            }

            // No cards left underneath, destroy this card
            IEnumerator destroyRoutine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
            }
        }

        private bool AreCardsRemainingBelowSelf()
        {
            return this.Card.UnderLocation.Cards.Any();
        }
    }
}
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class FlashReconCardController : CardController
    {
        public FlashReconCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            var coroutine = GameController.SelectLocationsAndDoAction(DecisionMaker, SelectionType.RevealTopCardOfDeck, l => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, RevealTopCardAndReturn, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            var sld = new SelectLocationDecision(GameController, HeroTurnTakerController, Game.TurnTakers.Select(tt => new LocationChoice(tt.Deck)), SelectionType.PlayTopCard, false, cardSource: GetCardSource());
            coroutine = GameController.SelectLocationAndDoAction(sld, loc => GameController.PlayTopCardOfLocation(DecisionMaker, loc, isPutIntoPlay: true, cardSource: GetCardSource(), showMessage: true));
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

        private IEnumerator RevealTopCardAndReturn(Location loc)
        {
            List<Card> result = new List<Card>();
            var coroutine = GameController.RevealCards(this.TurnTakerController, loc, 1, result, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.CleanupCardsAtLocations(new List<Location> { loc.OwnerTurnTaker.Revealed }, loc, cardsInList: result);
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
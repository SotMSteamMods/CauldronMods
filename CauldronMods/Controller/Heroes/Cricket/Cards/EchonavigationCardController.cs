using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cricket
{
    public class EchonavigationCardController : CardController
    {
        public EchonavigationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal the top card of each deck. You may replace or discard each card.",
            IEnumerator coroutine = GameController.SelectLocationsAndDoAction(DecisionMaker, SelectionType.RevealTopCardOfDeck, l => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && l.IsRealDeck, (Location loc) => this.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.FindTurnTakerController(loc.OwnerTurnTaker), loc, false), cardSource: GetCardSource());
            //IEnumerator coroutine = base.GameController.SelectTurnTakersAndDoAction(base.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => true), SelectionType.RevealTopCardOfDeck, (TurnTaker tt) => this.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.FindTurnTakerController(tt), tt.Deck, false), allowAutoDecide: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //One player may draw a card now.
            coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class GraveyardBridgeCardController : CardController
    {
        public GraveyardBridgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => true));
        }

        public override IEnumerator Play()
        {
            MoveCardDestination turnTakerDeck = new MoveCardDestination(base.TurnTaker.Deck, true);
            MoveCardDestination turnTakerPlayArea = new MoveCardDestination(base.TurnTaker.PlayArea, true);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

            //You may shuffle a card from your trash into your deck... 
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsInTrash), turnTakerDeck.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, showOutput: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier, "card with the same name", false);
            //...If you do...
            if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
            {
                //...put a card with the same name from your trash into play.
                IEnumerable<Card> list = FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier && c.IsInTrash, "card with the same name", false));
                coroutine = base.GameController.PlayCard(this.TurnTakerController, list.FirstOrDefault(), responsibleTurnTaker: this.TurnTaker, cardSource: base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shuffle all copies of that card from your trash into your deck.
                IEnumerable<Card> cardsWithSameName = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier && c.Location.IsTrash));
                coroutine = base.GameController.ShuffleCardsIntoLocation(base.HeroTurnTakerController, cardsWithSameName, base.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //actually shuffle deck
                coroutine = base.GameController.ShuffleLocation(this.TurnTaker.Deck);
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
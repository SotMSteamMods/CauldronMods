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
        #region Constructors

        public GraveyardBridgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            MoveCardDestination turnTakerDeck = new MoveCardDestination(base.TurnTaker.Deck, true, false, false);
            MoveCardDestination turnTakerPlayArea = new MoveCardDestination(base.TurnTaker.PlayArea, true, false, false);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

            //You may shuffle a card from your trash into your deck... 
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsInTrash), turnTakerDeck.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, false, true, null, false, true, null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier, "card with the same name", false, false, null, null, false);
            //...If you do...
            if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
            {
                //...put a card with the same name from your trash into play.
                IEnumerable<Card> list = FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier && c.IsInTrash, "card with the same name", false, false, null, null, false));
                coroutine = base.GameController.PlayCard(this.TurnTakerController, list.FirstOrDefault(), false, null, false, null, this.TurnTaker, false, null, null, null, false, false, true, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                
                //Shuffle all copies of that card from your trash into your deck.
                coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController, base.TurnTaker.Trash, list.Count<Card>(), this.TurnTaker.Trash.NumberOfCards, cardCriteria, turnTakerDeck.ToEnumerable<MoveCardDestination>(), false, true, false, false, null, null, false, false, false, this.TurnTaker, false, false, null, SelectionType.MoveCardOnDeck, base.GetCardSource(null));
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

        #endregion Methods
    }
}
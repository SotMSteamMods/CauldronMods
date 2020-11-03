using System;
using System.Collections;
using System.Collections.Generic;
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
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, null, turnTakerDeck.ToEnumerable<MoveCardDestination>(), false, true, true, true, storedResults, false, true, null, false, true, null, null, base.GetCardSource(null));
            SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier, "card with the same name", true, false, null, null, false);
            //...If you do...
            if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
            {
                //...put a card with the same name from your trash into play.
                IEnumerator coroutine2 = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, cardCriteria, turnTakerPlayArea.ToEnumerable<MoveCardDestination>(), true, true, true, true, storedResults, false, true, null, false, true, null, null, base.GetCardSource(null));

                //Shuffle all copies of that card from your trash into your deck.
                IEnumerator coroutine3 = base.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController, base.TurnTaker.Trash, new int?(0), base.TurnTaker.Deck.NumberOfCards, cardCriteria, turnTakerDeck.ToEnumerable<MoveCardDestination>(), false, true, true, false, storedResults, null, true, false, false, this.TurnTaker, false, true, null, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                    base.GameController.ExhaustCoroutine(coroutine3);
                }
            }

            yield break;
        }

        #endregion Methods
    }
}
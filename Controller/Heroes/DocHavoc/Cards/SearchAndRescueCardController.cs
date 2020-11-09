using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class SearchAndRescueCardController : CardController
    {

        //==============================================================
        // Each player may discard a card.
        // Any player that does may reveal the top 3 cards of their deck, put 1 in their hand, 1 on the bottom of their deck, and 1 in their trash.
        //==============================================================

        public static string Identifier = "SearchAndRescue";

        public SearchAndRescueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator discardCardRoutine 
                = this.GameController.EachPlayerDiscardsCards(0, 1, storedResults);

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            foreach (DiscardCardAction dca in storedResults.Where(dca => dca.WasCardDiscarded))
            {
                Debug.WriteLine("card was discarded");
                
                IEnumerator orderedDestinationsRoutine = this.RevealCardsFromDeckToMoveToOrderedDestinations(
                    (TurnTakerController)dca.HeroTurnTakerController, 
                    dca.ResponsibleTurnTaker.Deck, 
                    (IEnumerable<MoveCardDestination>)new List<MoveCardDestination>()
                {
                    new MoveCardDestination(dca.HeroTurnTakerController.HeroTurnTaker.Hand),
                    new MoveCardDestination(dca.ResponsibleTurnTaker.Deck, true),
                    new MoveCardDestination(dca.ResponsibleTurnTaker.Trash)
                }, sendCleanupMessageIfNecessary: true);

                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(orderedDestinationsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(orderedDestinationsRoutine);
                }
            }
        }
    }
}

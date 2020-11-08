using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((Func<TurnTaker, bool>)(tt => tt == this.TurnTaker), 
                new Func<PhaseChangeAction, IEnumerator>(this.DiscardToDrawResponse), (IEnumerable<TriggerType>)new TriggerType[1]
            {
                TriggerType.DiscardCard
            });

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            //this.EachPlayerMayDiscardOneCardToPerformAction()

            //this.EachPlayerMayDiscardOneCardToPerformAction(this.ac)

            return base.Play();
        }

        private IEnumerator DiscardToDrawResponse(PhaseChangeAction p)
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine1 = this.GameController.SelectAndDiscardCard(this.DecisionMaker, true, storedResults: storedResults, 
                responsibleTurnTaker: this.TurnTaker, cardSource: this.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine1);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine1);
            }

            IEnumerator coroutine2;
            if (this.DidDiscardCards((IEnumerable<DiscardCardAction>)storedResults, new int?(1)))
            {
                Card cardToDiscard = storedResults.First<DiscardCardAction>().CardToDiscard;
                coroutine2 = this.SelectAndPlayCardFromHand(this.HeroTurnTakerController);
            }
            else
                coroutine2 = this.GameController.SendMessageAction(this.TurnTaker.Name + " did not discard a card, so no card will be played.", 
                    Priority.Medium, this.GetCardSource(), showCardSource: true);

            if (this.UseUnityCoroutines)
                yield return this.GameController.StartCoroutine(coroutine2);
            else
                this.GameController.ExhaustCoroutine(coroutine2);
        }

    }
}

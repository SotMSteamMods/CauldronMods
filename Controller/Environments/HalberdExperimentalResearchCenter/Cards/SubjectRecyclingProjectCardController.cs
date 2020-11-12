using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class SubjectRecyclingProjectCardController : TestSubjectCardController
    {
        #region Constructors

        public SubjectRecyclingProjectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //Reduce damage dealt to that Test Subject by 1.
            base.AddReduceDamageTrigger((Card c) => c == base.GetCardThisCardIsNextTo(true), 1);
			//If that Test Subject leaves play, this card is destroyed.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction destroy) => destroy.CardToDestroy != null && destroy.CardToDestroy.Card == base.GetCardThisCardIsNextTo(true), (DestroyCardAction destroy) => base.DestroyThisCardResponse(destroy), TriggerType.DestroySelf, TriggerTiming.After);
		}

        public override IEnumerator Play()
        {
			
			//check to see if there are any Test Subjects to retrieve
			bool tryPlaying = true;
			string message = base.Card.Title + " plays a random Test Subject card from the environment Trash.";
			if (!base.TurnTaker.Trash.Cards.Any((Card c) => base.IsTestSubject(c)))
			{
				tryPlaying = false;
				message = "There are no Test Subjects in the environment trash to play.";
			}
			IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (tryPlaying)
			{
				//put a random Test Subject from the environment trash into play
				List<Card> storedResults = new List<Card>();
				IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Trash, false, true, false, new LinqCardCriteria((Card c) => base.IsTestSubject(c), "test subject"), new int?(1), storedPlayResults: storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}

				//...and place this card next to it
				if(storedResults != null)
				{
					Card cardToMoveNextTo = storedResults.First();
					IEnumerator coroutine3 = base.GameController.MoveCard(base.TurnTakerController, base.Card, cardToMoveNextTo.NextToLocation, cardSource: base.GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine3);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine3);
					}
				}
			}
			else
			{
				//...otherwise destroy this card.
				IEnumerator coroutine4 = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: base.GetCardSource(null));
				if (this.UseUnityCoroutines)
				{
					yield return this.GameController.StartCoroutine(coroutine4);
				}
				else
				{
					this.GameController.ExhaustCoroutine(coroutine4);
				}
			}
			yield break;

        }
        #endregion Methods
    }
}
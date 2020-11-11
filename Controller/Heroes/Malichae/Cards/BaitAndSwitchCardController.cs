using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class BaitAndSwitchCardController : MalichaeCardController
	{
		public BaitAndSwitchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
			var coroutine = base.SelectAndDiscardCards(this.DecisionMaker, 1,
								optional: false,
								requiredDecisions: 0,
								storedResults: storedResults,
								cardCriteria: new LinqCardCriteria(c => IsDjinn(c), "djinn"),
								selectionType: SelectionType.DiscardCard);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidDiscardCards(storedResults, 1))
            {
				//Option 1: You may put a Djinn card from your Trash into play
				//Hide if no Djinn in Trash
				var response1 = base.GameController.SelectAndMoveCard(this.DecisionMaker, c => c.IsInTrash && IsDjinn(c), this.DecisionMaker.TurnTaker.PlayArea,
					optional: true,
					isPutIntoPlay: true,
					cardSource: GetCardSource());
				var op1 = new Function(this.DecisionMaker, "You may put a Djinn card from your Trash into play", SelectionType.PutIntoPlay, () => response1,
					onlyDisplayIfTrue: this.GetTrashDestination().Cards.Any(c => IsDjinn(c)));

				//Option 2: Draw 2 cards.
				var response2 = base.DrawCards(this.DecisionMaker, 2);
				var op2 = new Function(this.DecisionMaker, $"Draw 2 cards", SelectionType.DrawCard, () => response2);

				//Execute
				var options = new Function[] { op1, op2 };
				var selectFunctionDecision = new SelectFunctionDecision(base.GameController, this.DecisionMaker, options, false, cardSource: base.GetCardSource());
				coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			coroutine = base.GameController.SelectAndUsePower(this.DecisionMaker, cardSource: GetCardSource());
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
	}
}

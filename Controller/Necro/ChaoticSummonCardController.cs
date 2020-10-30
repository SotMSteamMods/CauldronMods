using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace SotMWorkshop.Controller.Necro
{
	public class ChaoticSummonCardController : CardController
    {
		public ChaoticSummonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		
		}
		public override IEnumerator Play()
		{
			//Put the top 2 cards of your deck into play.

			IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, false, 2, false, null, null, null, false, null, false, false, false, null, null, base.GetCardSource(null));
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

using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
	public class AlterationCardController : CardController
	{
		public AlterationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{

		}

		public override IEnumerator Play()
		{
			//Play the top card of the environment deck. Play the top card of the villain deck.
			IEnumerator message1 = GameController.SendMessageAction(Card.Title + " plays the top card of the environment deck...", Priority.High, GetCardSource(), showCardSource: true);
			IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.FindEnvironment(), cardSource: base.GetCardSource());
			IEnumerator message2 = GameController.SendMessageAction(Card.Title + " plays the top card of the villain deck...", Priority.High, GetCardSource(), showCardSource: true);
			IEnumerator coroutine2 = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(message1);
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(message2);
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(message1);
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(message2);
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}
	}
}
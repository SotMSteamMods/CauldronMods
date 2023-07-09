using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class ToOverbrookCardController : TransportCardController
    {
        public ToOverbrookCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

		public override IEnumerator UniqueOnPlayEffect()
		{
			//play its top card.
			IEnumerator coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
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
		public override IEnumerator ActivateTravel()
        {
			//Play the top card of each other deck in turn order, starting with the villain deck.
			IEnumerator coroutine = base.GameController.SendMessageAction(base.Card.Title + " plays the top card of each villain and hero deck.", Priority.Medium, GetCardSource(), showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = PlayTopCardOfEachDeckInTurnOrder((TurnTakerController ttc) => ttc.IsVillain && !ttc.TurnTaker.IsScion, (Location l) => l.IsVillain, TurnTaker);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = PlayTopCardOfEachDeckInTurnOrder((TurnTakerController ttc) => IsHero(ttc.TurnTaker), (Location l) => l.IsHero, TurnTaker);
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

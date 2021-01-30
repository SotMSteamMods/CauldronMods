using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public abstract class TerminusUtilityCardController : CardController
    {
        protected TokenPool WrathPool => base.CharacterCard.FindTokenPool("TerminusWrathPool");

        protected TerminusUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
		}

		protected IEnumerator SendMessageAboutInsufficientTokens(int numberRemoved, string suffix)
		{
			string text = "There are no tokens to remove";
			if (numberRemoved == 1)
			{
				text = "Only one token was removed";
			}
			else if (numberRemoved > 1)
			{
				text = $"Only {numberRemoved} tokens were removed";
			}
			string message = text + ", so " + suffix;
			IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), null, showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}
	}
}

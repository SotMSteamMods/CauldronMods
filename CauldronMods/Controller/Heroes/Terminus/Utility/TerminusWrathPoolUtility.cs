using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public static class TerminusWrathPoolUtility
	{
		public static TokenPool GetWrathPool(CardController cardController)
        {
			return cardController.CharacterCard.FindTokenPool("TerminusWrathPool");
		}

		public static IEnumerator AddWrathTokens(CardController cardController, int amountToAdd)
		{
			IEnumerator coroutine;

			coroutine = AddWrathTokens<GameAction>(cardController, amountToAdd, null);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		public static IEnumerator AddWrathTokens<TAdd>(CardController cardController, int amountToAdd, Func<TAdd, IEnumerator> addTokenResponse, TAdd addTokenGameAction = null)
			where TAdd : GameAction
		{
			IEnumerator coroutine;

			coroutine = AddWrathTokenResponse(cardController, amountToAdd, addTokenResponse, addTokenGameAction);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}
		public static IEnumerator RemoveWrathTokens<TRemove>(CardController cardController, int amountToRemove, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse = null, TRemove removeTokenGameAction = null, string insufficientTokenMessage = null, bool optional = false)
			where TRemove : GameAction
		{
			IEnumerator coroutine;
			List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();

			coroutine = RemoveWrathTokenResponse(cardController, amountToRemove, removeTokenResponse, removeTokenGameAction, storedResults, insufficientTokenMessage, optional);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		public static IEnumerator AddOrRemoveWrathTokens<TAdd, TRemove>(CardController cardController, int amountToAdd, int amountToRemove, Func<TAdd, IEnumerator> addTokenResponse = null, TAdd addTokenGameAction = null, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse = null, TRemove removeTokenGameAction = null, string insufficientTokenMessage = null)
			where TAdd : GameAction
			where TRemove : GameAction
		{
			IEnumerator coroutine;
			List<Function> list = new List<Function>();
			List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();
			SelectFunctionDecision selectFunction;
			String addToken = "one token";

			if (amountToAdd > 1)
			{
				addToken = $"{amountToAdd} tokens";
			}
			list.Add(new Function(cardController.DecisionMaker, $"Add {addToken} to {GetWrathPool(cardController).Name}", SelectionType.AddTokens, () => AddWrathTokenResponse(cardController, amountToAdd, addTokenResponse, addTokenGameAction)));
			list.Add(new Function(cardController.DecisionMaker, $"Remove 3 tokens from {GetWrathPool(cardController).Name}", SelectionType.RemoveTokens, () => RemoveWrathTokenResponse(cardController, amountToRemove, removeTokenResponse, removeTokenGameAction, storedResults, insufficientTokenMessage)));
			selectFunction = new SelectFunctionDecision(cardController.GameController, cardController.DecisionMaker, list, false, null, null, null, cardController.GetCardSource());

			coroutine = cardController.GameController.SelectAndPerformFunction(selectFunction);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		public static IEnumerator AddWrathTokenResponse<TAdd>(CardController cardController, int amountToAdd, Func<TAdd, IEnumerator> addTokenResponse, TAdd addTokenGameAction)
			where TAdd : GameAction
		{
			IEnumerator coroutine;

			coroutine = cardController.GameController.AddTokensToPool(GetWrathPool(cardController), amountToAdd, cardController.GetCardSource());
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			coroutine = SendMessageWrathTokensAdded(cardController, amountToAdd);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			if (addTokenResponse != null)
			{
				coroutine = addTokenResponse(addTokenGameAction);
				if (cardController.UseUnityCoroutines)
				{
					yield return cardController.GameController.StartCoroutine(coroutine);
				}
				else
				{
					cardController.GameController.ExhaustCoroutine(coroutine);
				}
			}

			yield break;
		}

		public static IEnumerator RemoveWrathTokenResponse<TRemove>(CardController cardController, int amountToRemove, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse, TRemove removeTokenGameAction, List<RemoveTokensFromPoolAction> storedResults, string insufficientTokenMessage, bool optional = false)
			where TRemove : GameAction
		{
			IEnumerator coroutine;

			coroutine = cardController.GameController.RemoveTokensFromPool(GetWrathPool(cardController), amountToRemove, storedResults, optional: optional, null, cardController.GetCardSource());
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			if ((storedResults.FirstOrDefault()?.NumberOfTokensActuallyRemoved ?? 0) >= amountToRemove)
			{
				coroutine = SendMessageWrathTokensRemoved(cardController, amountToRemove, storedResults);
			}
			else
			{
				coroutine = SendMessageAboutInsufficientWrathTokens(cardController, (storedResults.FirstOrDefault()?.NumberOfTokensActuallyRemoved ?? 0), insufficientTokenMessage);
			}
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			if (removeTokenResponse != null)
			{
				coroutine = removeTokenResponse(removeTokenGameAction, storedResults);
				if (cardController.UseUnityCoroutines)
				{
					yield return cardController.GameController.StartCoroutine(coroutine);
				}
				else
				{
					cardController.GameController.ExhaustCoroutine(coroutine);
				}
			}

			yield break;
		}

		public static IEnumerator SendMessageWrathTokensAdded(CardController cardController, int numberAdded)
		{
			IEnumerator coroutine;
			string message = "No tokens added";

			if (numberAdded == 1)
			{
				message = "One token was added";
			}
			else
			{
				if (numberAdded > 1)
				{
					message = $"{numberAdded} tokens were added";
				}
			}

			coroutine = cardController.GameController.SendMessageAction(message, Priority.Medium, cardController.GetCardSource(), null, showCardSource: true);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		public static IEnumerator SendMessageWrathTokensRemoved(CardController cardController, int numberRemoved, List<RemoveTokensFromPoolAction> removeTokensFromPoolActions)
		{
			IEnumerator coroutine;
			string message = "No tokens removed";

			if (numberRemoved == 1)
			{
				message = "One token was removed";
			}
			else
			{
				if (numberRemoved > 1)
				{
					message = $"{numberRemoved} tokens were removed";
				}
			}

			coroutine = cardController.GameController.SendMessageAction(message, Priority.Medium, cardController.GetCardSource(), null, showCardSource: true);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		public static IEnumerator SendMessageAboutInsufficientWrathTokens(CardController cardController, int numberRemoved, string suffix = null)
		{
			IEnumerator coroutine;
			string message = "There are no tokens to remove";

			if (numberRemoved == 1)
			{
				message = "Only one token was removed";
			}
			else if (numberRemoved > 1)
			{
				message = $"Only {numberRemoved} tokens were removed";
			}

			if (suffix != null && !string.IsNullOrEmpty(suffix.Trim()))
			{
				message = $"{message}, so {suffix}";
			}

			coroutine = cardController.GameController.SendMessageAction(message, Priority.Medium, cardController.GetCardSource(), null, showCardSource: true);
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}
		}

	}
}

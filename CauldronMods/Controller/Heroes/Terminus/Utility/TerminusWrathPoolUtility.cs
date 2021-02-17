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
			//Check Character Card first
			var prospect = cardController.CharacterCard.FindTokenPool("TerminusWrathPool");
			if (prospect != null)
			{
				return prospect;
			}

			//If not, look for a "Terminus" TurnTaker and get their character card
			var terminus = cardController.GameController.Game.HeroTurnTakers.Where(htt => htt.Identifier == "Terminus").FirstOrDefault();
			if (terminus != null)
			{
				return terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
			}

			//If not there, try the card itself (for Representative of Earth purposes)
			prospect = cardController.CardWithoutReplacements.FindTokenPool("TerminusWrathPool");
			if(prospect != null)
            {
				return prospect;
            }

			//If not, we have failed to find it - error handle!
			return null;
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

		public static IEnumerator AddOrRemoveWrathTokens<TAdd, TRemove>(CardController cardController, int amountToAdd, int amountToRemove, Func<TAdd, IEnumerator> addTokenResponse = null, TAdd addTokenGameAction = null, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse = null, TRemove removeTokenGameAction = null, string insufficientTokenMessage = null, string removeEffectDescription = null, GameAction triggerAction = null)
			where TAdd : GameAction
			where TRemove : GameAction
		{
			IEnumerator coroutine;

			if(GetWrathPool(cardController) == null)
            {
				coroutine = WrathPoolErrorMessage(cardController);
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
			List<Function> list = new List<Function>();
			List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();
			SelectFunctionDecision selectFunction;
			String addToken = "1 token";
			String removeDescriber = "";
			if (amountToAdd > 1)
			{
				addToken = $"{amountToAdd} tokens";
			}
			if(GetWrathPool(cardController).CurrentValue < amountToRemove)
            {
				removeDescriber = " to no effect";
            }
			else if (removeEffectDescription != null)
            {
				removeDescriber = " to " + removeEffectDescription;
            }
			list.Add(new Function(cardController.DecisionMaker, $"Add {addToken} to {GetWrathPool(cardController).Name}", SelectionType.AddTokens, () => AddWrathTokenResponse(cardController, amountToAdd, addTokenResponse, addTokenGameAction)));
			list.Add(new Function(cardController.DecisionMaker, $"Remove 3 tokens from {GetWrathPool(cardController).Name}" + removeDescriber, SelectionType.RemoveTokens, () => RemoveWrathTokenResponse(cardController, amountToRemove, removeTokenResponse, removeTokenGameAction, storedResults, insufficientTokenMessage)));
			selectFunction = new SelectFunctionDecision(cardController.GameController, cardController.DecisionMaker, list, false, triggerAction, null, null, cardController.GetCardSource());

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
			if (GetWrathPool(cardController) == null)
			{
				coroutine = WrathPoolErrorMessage(cardController);
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
			if (GetWrathPool(cardController) == null)
			{
				coroutine = WrathPoolErrorMessage(cardController);
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

			coroutine = cardController.GameController.RemoveTokensFromPool(GetWrathPool(cardController), amountToRemove, storedResults, optional: optional, null, cardController.GetCardSource());
			if (cardController.UseUnityCoroutines)
			{
				yield return cardController.GameController.StartCoroutine(coroutine);
			}
			else
			{
				cardController.GameController.ExhaustCoroutine(coroutine);
			}

			if (storedResults.FirstOrDefault() != null)
			{
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
			if (GetWrathPool(cardController) == null)
			{
				coroutine = WrathPoolErrorMessage(cardController);
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

			string message = "No tokens were added";

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
			
			message += $" to {GetWrathPool(cardController).Name}.";
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
			if (GetWrathPool(cardController) == null)
			{
				coroutine = WrathPoolErrorMessage(cardController);
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
			string message = "No tokens were removed";

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

			message += $" from {GetWrathPool(cardController).Name}.";
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
			if (GetWrathPool(cardController) == null)
			{
				coroutine = WrathPoolErrorMessage(cardController);
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
			string message = "There were no tokens to remove";

			if (numberRemoved == 1)
			{
				message = "Only one token was removed";
			}
			else if (numberRemoved > 1)
			{
				message = $"Only {numberRemoved} tokens were removed";
			}

			message += $" from {GetWrathPool(cardController).Name}";
			if (suffix != null && !string.IsNullOrEmpty(suffix.Trim()))
			{
				message = $"{message}, so {suffix}";
			}
			else
			{
				message += ".";
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

		public static IEnumerator WrathPoolErrorMessage(CardController cardController)
        {
			return cardController.GameController.SendMessageAction("No appropriate Wrath Pool could be found.", Priority.High, cardController.GetCardSource());
        }
	}
}

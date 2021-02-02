using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
	public abstract class TerminusBaseCardController : CardController
	{
		protected TokenPool WrathPool => base.CharacterCard.FindTokenPool("TerminusWrathPool");

		protected TerminusBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		protected IEnumerator AddWrathTokens(int amountToAdd)
        {
			IEnumerator coroutine;

			coroutine = TerminusWrathPoolUtility.AddWrathTokens(this, amountToAdd);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		protected IEnumerator AddWrathTokens<TAdd>(int amountToAdd, Func<TAdd, IEnumerator> addTokenResponse, TAdd addTokenGameAction = null)
			where TAdd : GameAction
		{
			IEnumerator coroutine;

			coroutine = TerminusWrathPoolUtility.AddWrathTokenResponse<TAdd>(this, amountToAdd, addTokenResponse, addTokenGameAction);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		protected IEnumerator RemoveWrathTokens<TRemove>(int amountToRemove, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse = null, TRemove removeTokenGameAction = null, string insufficientTokenMessage = null, bool optional = false)
			where TRemove : GameAction
		{
			IEnumerator coroutine;

			coroutine = TerminusWrathPoolUtility.RemoveWrathTokens<TRemove>(this, amountToRemove, removeTokenResponse, removeTokenGameAction, insufficientTokenMessage, optional);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		protected IEnumerator AddOrRemoveWrathTokens<TAdd, TRemove>(int amountToAdd, int amountToRemove, Func<TAdd, IEnumerator> addTokenResponse = null, TAdd addTokenGameAction = null, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse = null, TRemove removeTokenGameAction = null, string insufficientTokenMessage = null) 
			where TAdd: GameAction 
			where TRemove: GameAction
		{
			IEnumerator coroutine;

			coroutine = TerminusWrathPoolUtility.AddOrRemoveWrathTokens<TAdd, TRemove>(this, amountToAdd, amountToRemove, addTokenResponse, addTokenGameAction, removeTokenResponse, removeTokenGameAction, insufficientTokenMessage);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			yield break;
		}

		protected IEnumerator AddWrathTokenResponse<TAdd>(int amountToAdd, Func<TAdd, IEnumerator> addTokenResponse, TAdd addTokenGameAction)
			where TAdd: GameAction
		{
			IEnumerator coroutine;

			coroutine = TerminusWrathPoolUtility.AddWrathTokenResponse<TAdd>(this, amountToAdd, addTokenResponse, addTokenGameAction);
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

		protected IEnumerator RemoveWrathTokenResponse<TRemove>(int amountToRemove, Func<TRemove, List<RemoveTokensFromPoolAction>, IEnumerator> removeTokenResponse, TRemove removeTokenGameAction, List<RemoveTokensFromPoolAction> storedResults, string insufficientTokenMessage, bool optional = false)
			where TRemove:GameAction
		{
			IEnumerator coroutine;

			coroutine = TerminusWrathPoolUtility.RemoveWrathTokenResponse<TRemove>(this, amountToRemove, removeTokenResponse, removeTokenGameAction, storedResults, insufficientTokenMessage, optional);
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

		protected IEnumerable<Card> GetTargetsThatHaveDealtDamageToTerminusSinceHerLastTurn()
		{
			return (from e in base.GameController.Game.Journal.DealDamageEntriesToTargetSinceLastTurn(base.CharacterCard, base.TurnTaker)
					where e.SourceCard != null && HasTargetDealtDamageToTerminusSinceHerLastTurn(e.SourceCard)
					select e.SourceCard).Distinct();
		}

		protected bool HasTargetDealtDamageToTerminusSinceHerLastTurn(Card target)
		{
			DealDamageJournalEntry dealDamageJournalEntry;
			PlayCardJournalEntry playCardJournalEntry;
			int? entryIndex;
			bool dealtDamage = false;

			if (target.IsTarget)
			{
				dealDamageJournalEntry = (from d in base.GameController.Game.Journal.DealDamageEntriesFromTargetToTargetSinceLastTurn(target, base.CharacterCard, base.TurnTaker)
										  where d.Amount > 0
										  select d).LastOrDefault();
				if (dealDamageJournalEntry != null && target.IsInPlayAndHasGameText)
				{
					entryIndex = base.GameController.Game.Journal.GetEntryIndex(dealDamageJournalEntry);
					playCardJournalEntry = (from c in base.GameController.Game.Journal.PlayCardEntries()
											where c.CardPlayed == target
											select c).LastOrDefault();
					if (playCardJournalEntry == null)
					{
						dealtDamage = true;
					}
					else
					{
						if (entryIndex > base.GameController.Game.Journal.GetEntryIndex(playCardJournalEntry))
						{
							dealtDamage = true;
						}
					}
				}
			}
			return dealtDamage;
		}
	}
}

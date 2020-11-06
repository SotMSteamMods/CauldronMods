using System;
using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementalFormCardController : CardController
    {
        #region Constructors

        public ElementalFormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		#endregion Constructors

		#region Methods

		public override void AddTriggers()
		{
			//Whenever a Head takes damage, it becomes immune to damage of that type until the start of the next villain turn.
			CreateTriggers(base.TurnTaker.GetCardByIdentifier("InfernoTiamatCharacterCard"));
			CreateTriggers(base.TurnTaker.GetCardByIdentifier("StormTiamatCharacterCard"));
			CreateTriggers(base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacterCard"));
		}

		private void CreateTriggers(Card characterCard)
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => dealDamage.Target == characterCard, new Func<DealDamageAction, IEnumerator>(this.ImmuneResponse), TriggerType.ImmuneToDamage, TriggerTiming.Before, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);
			base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => characterCard.IsInPlayAndHasGameText && dealDamage.Target == characterCard && dealDamage.DidDealDamage, new Func<DealDamageAction, IEnumerator>(this.WarningMessageResponse), TriggerType.Hidden, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		private IEnumerator WarningMessageResponse(DealDamageAction dealDamage)
		{
			if (base.IsFirstOrOnlyCopyOfThisCardInPlay())
			{
				IEnumerator coroutine = base.GameController.SendMessageAction("Tiamat used Elemental Form to become immune to " + dealDamage.DamageType.ToString() + " damage.", Priority.High, base.GetCardSource(null), null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				Log.Debug("Tiamat used Elemental Form to become immune to " + dealDamage.DamageType.ToString() + " damage. ");
			}
			yield return null;
			yield break;
		}

		private IEnumerator ImmuneResponse(DealDamageAction dealDamage)
		{
			DamageType? damageTypeThatOmnitronIsImmuneTo = this.GetDamageTypeThatOmnitronIsImmuneTo();
			if (damageTypeThatOmnitronIsImmuneTo != null && base.IsFirstOrOnlyCopyOfThisCardInPlay() && dealDamage.DamageType == damageTypeThatOmnitronIsImmuneTo.Value)
			{
				IEnumerator coroutine = base.GameController.ImmuneToDamage(dealDamage, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}

		private DamageType? GetDamageTypeThatOmnitronIsImmuneTo()
		{
			DealDamageJournalEntry dealDamageJournalEntry = base.GameController.Game.Journal.MostRecentDealDamageEntry((DealDamageJournalEntry e) => e.TargetCard == base.CharacterCard && e.Amount > 0);
			PlayCardJournalEntry playCardJournalEntry = base.GameController.Game.Journal.QueryJournalEntries<PlayCardJournalEntry>((PlayCardJournalEntry e) => e.CardPlayed == base.Card).LastOrDefault<PlayCardJournalEntry>();
			if (playCardJournalEntry != null)
			{
				int? entryIndex = base.GameController.Game.Journal.GetEntryIndex(dealDamageJournalEntry);
				int? entryIndex2 = base.GameController.Game.Journal.GetEntryIndex(playCardJournalEntry);
				if (entryIndex != null && entryIndex2 != null && entryIndex.Value > entryIndex2.Value)
				{
					return new DamageType?(dealDamageJournalEntry.DamageType);
				}
			}
			return null;
		}
		#endregion Methods
	}
}
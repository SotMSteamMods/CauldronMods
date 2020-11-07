using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Cauldron.Necro
{
	public class FinalRitualCardController : NecroCardController
	{
		public FinalRitualCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//Search your trash for up to 2 Undead and put them into play. Necro deals each of those cards 2 toxic damage.

			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = base.SearchForCards(this.DecisionMaker, false, true, new int?(0), 2, new LinqCardCriteria((Card c) => this.IsUndead(c), "undead", true, false, null, null, false), true, false, false, false, storedResults, false, null, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			int numUndeadPlayed = 0;

			//Necro deals each of those cards 2 toxic damage.
			IEnumerator coroutine2;
			foreach (SelectCardDecision cardDecision in storedResults)
			{
				Card undeadTarget = cardDecision.SelectedCard;
				if(undeadTarget != null)
				{
					numUndeadPlayed++;
				}
				coroutine2 = base.DealDamage(base.CharacterCard, (Card card) => card == undeadTarget, 2, DamageType.Toxic, false, false, null, null, null, false, null, null, false, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}

			//Then Necro deals himself X toxic damage, where X is 2 times the number of cards put into play this way."

			IEnumerator coroutine3 = base.DealDamage(base.CharacterCard, (Card card) => card == base.CharacterCard, 2 * numUndeadPlayed, DamageType.Toxic, false, false, null, null, null, false, null, null, false, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}
			yield break;
		}
	}
}

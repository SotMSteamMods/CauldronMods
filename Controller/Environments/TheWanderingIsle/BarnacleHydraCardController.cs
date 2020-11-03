using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
	public class BarnacleHydraCardController : CardController
	{
		public BarnacleHydraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//When this card is destroyed, it deals Teryx { H} toxic damage
			base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.DestroyCardResponse), TriggerType.DealDamage);
			//At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage. Then, if Submerge is in play, this card regains 6HP.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageReponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.GainHP }, null, false);
		}

		private IEnumerator DealDamageReponse(PhaseChangeAction pca)
		{
			//this card deals the non-environment target with the lowest HP 2 projectile damage. 
			IEnumerator dealDamage = base.DealDamageToLowestHP(base.Card, 1, (Card c) => c.IsTarget && !c.IsEnvironment, (Card c) => new int?(2), DamageType.Projectile);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(dealDamage);
			}
			else
			{
				base.GameController.ExhaustCoroutine(dealDamage);
			}

			//Then, if Submerge is in play, this card regains 6HP.
			if (this.IsCardInPlay(base.TurnTakerController, "Submerge"))
			{
				IEnumerator gainHp = base.GameController.GainHP(base.Card, new int?(6));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(gainHp);
				}
				else
				{
					base.GameController.ExhaustCoroutine(gainHp);
				}
			}

			yield break;
		}

		private IEnumerator DestroyCardResponse(DestroyCardAction dca)
		{
			if (this.IsCardInPlay(base.TurnTakerController, "Teryx"))
			{
				//it deals Teryx { H} toxic damage
				IEnumerator dealDamage = base.DealDamage(base.Card, (Card c) => c.IsInPlay && c.Identifier == "Teryx", base.H, DamageType.Toxic, false, false, null, null, null, false, null, null, false, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(dealDamage);
				}
				else
				{
					base.GameController.ExhaustCoroutine(dealDamage);
				}
			}
			yield break;
		}

		private bool IsCardInPlay(TurnTakerController ttc, string identifier)
		{
			var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsCard(c, identifier));
			var numCardsInPlay = cardsInPlay.Count();

			return numCardsInPlay > 0;
		}
		private bool IsCard(Card card, string identifier)
		{
			return card.Identifier == identifier;
		}
	}
}

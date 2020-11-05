using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cauldron.Anathema
{
	public class DoppelgangerStrikeCardController : CardController
    {
		public DoppelgangerStrikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//The Hero target with the highest HP deals the Hero Character with the lowest HP X toxic damage, where X is the number of villain targets in play. A Hero dealt damage this way must discard {H-2} cards.

			//Find the hero target with the most HP
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetsWithHighestHitPoints(1, 1, (Card card) => card.IsHero, storedResults, null, null, false, false, null, false, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card heroTarget = storedResults.FirstOrDefault<Card>();
			if (heroTarget != null)
			{

				//The Hero target with the highest HP deals the Hero Character with the lowest HP X toxic damage, where X is the number of villain targets in play. 
				int X = this.NumberOfVillainTargetsInPlay;
				List<DealDamageAction> targetResults = new List<DealDamageAction>();
				IEnumerator coroutine2 = base.DealDamageToLowestHP(heroTarget, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => new int?(X), DamageType.Toxic, false, false, targetResults, 1, null, null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}

				//A Hero dealt damage this way must discard {H-2} cards.

				//currently only discards 1 card
				DealDamageAction dealDamageAction = targetResults.FirstOrDefault<DealDamageAction>();
				if (dealDamageAction != null && dealDamageAction.Target != null && dealDamageAction.Target.IsHero && dealDamageAction.DidDealDamage)
				{
					HeroTurnTakerController hero = base.FindHeroTurnTakerController(dealDamageAction.Target.Owner.ToHero());
					IEnumerator discard = base.GameController.SelectAndDiscardCards(hero, (base.H - 2), false, (base.H - 2), null, false, null, null, null, null, SelectionType.DiscardCard, null, base.GetCardSource(null));

					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(discard);
					}
					else
					{
						base.GameController.ExhaustCoroutine(discard);
					}
				}
			}

			//Play the top card of the Villain Deck
			IEnumerator play = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, false, 1, false, null, null, null, false, null, false, false, false, null, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(play);
			}
			else
			{
				base.GameController.ExhaustCoroutine(play);
			}
			yield break;
		}

		//number of villain targets in play 
		private int NumberOfVillainTargetsInPlay
		{
			get
			{
				return base.FindCardsWhere((Card c) => base.IsVillainTarget(c) && c.IsInPlay, false, null, false).Count<Card>();
			}
		}

	}
}

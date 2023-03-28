using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
    public class DoppelgangerStrikeCardController : CardController
    {
		public DoppelgangerStrikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroTargetWithHighestHP();
			SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
			SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsVillainTarget(c), useCardsSuffix: false, singular: "villain target", plural: "villain targets"));
		}

		public override IEnumerator Play()
		{
			//The Hero target with the highest HP deals the Hero Character with the lowest HP X toxic damage, where X is the number of villain targets in play. A Hero dealt damage this way must discard {H-2} cards.

			//Find the hero target with the most HP
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetsWithHighestHitPoints(1, 1, (Card card) => IsHeroTarget(card), storedResults, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card heroTarget = storedResults.FirstOrDefault();
			if (heroTarget != null)
			{

				//The Hero target with the highest HP deals the Hero Character with the lowest HP X toxic damage, where X is the number of villain targets in play. 
				int X = this.NumberOfVillainTargetsInPlay;
				List<DealDamageAction> targetResults = new List<DealDamageAction>();
				IEnumerator coroutine2 = base.DealDamageToLowestHP(heroTarget, 1, (Card c) =>  IsHeroCharacterCard(c), (Card c) => new int?(X), DamageType.Toxic, storedResults: targetResults);
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
				DealDamageAction dealDamageAction = targetResults.FirstOrDefault();
				if (dealDamageAction != null && dealDamageAction.Target != null && IsHeroCharacterCard(dealDamageAction.Target) && dealDamageAction.DidDealDamage)
				{
					HeroTurnTakerController httc = base.FindHeroTurnTakerController(dealDamageAction.Target.Owner.ToHero());
					IEnumerator discard = base.GameController.SelectAndDiscardCards(httc, base.H - 2, false, base.H - 2,cardSource: base.GetCardSource());

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
			IEnumerator play = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
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
				return base.FindCardsWhere((Card c) => base.IsVillainTarget(c) && c.IsInPlay).Count();
			}
		}

	}
}

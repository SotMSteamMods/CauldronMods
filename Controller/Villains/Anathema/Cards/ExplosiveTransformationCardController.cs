using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class ExplosiveTransformationCardController : CardController
    {
		public ExplosiveTransformationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//Anathema deals each Hero target 1 projectile damage.
			IEnumerator coroutine = base.DealDamage(base.Card, (Card card) => card.IsHero, 1, DamageType.Projectile, false, false, null, null, null, false, null, null, false, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Reveal the top {H} cards of the Villain Deck. 
			List<Card> revealedCards = new List<Card>();
			coroutine = base.GameController.RevealCards(base.TurnTakerController, base.Card.Owner.Deck, 4, revealedCards, false, RevealedCardDisplay.ShowRevealedCards, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Put the first revealed arm, body, and head into play. 
			bool firstArm = true;
			bool firstHead= true;
			bool firstBody = true;
			List<Card> cardsToPlay = new List<Card>();
			List<Card> cardsToShuffle = new List<Card>();
			foreach(Card c in revealedCards)
			{
				if(this.IsArm(c) && firstArm)
				{
					firstArm = false;
					cardsToPlay.Add(c);
				}else if (this.IsHead(c) && firstHead)
				{
					firstHead = false;
					cardsToPlay.Add(c);
				} else if (this.IsBody(c) && firstBody)
				{
					firstBody = false;
					cardsToPlay.Add(c);
				} else
				{
					cardsToShuffle.Add(c);
				}
			}

			foreach(Card c in cardsToPlay)
			{
				coroutine = base.GameController.PlayCard(base.TurnTakerController, c, true, null, false, null, null, false, null, null, null, false, false, true, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			//Shuffle the remaining cards back into the Villain Deck.
			coroutine = base.GameController.ShuffleCardsIntoLocation(this.DecisionMaker, cardsToShuffle, this.TurnTaker.Deck, false, base.GetCardSource(null));
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

		private bool IsArm(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "arm", false, false);
		}

		private bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head", false, false);
		}

		private bool IsBody(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "body", false, false);
		}

	}
}

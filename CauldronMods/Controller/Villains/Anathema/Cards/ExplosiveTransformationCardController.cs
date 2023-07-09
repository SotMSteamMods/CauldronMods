using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class ExplosiveTransformationCardController : AnathemaCardController
    {
		public ExplosiveTransformationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//Anathema deals each Hero target 1 projectile damage.
			IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => IsHeroTarget(card), 1, DamageType.Projectile);
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
			coroutine = base.GameController.RevealCards(base.TurnTakerController, TurnTaker.Deck, base.H, revealedCards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: base.GetCardSource());
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
				if(base.IsArm(c) && firstArm)
				{
					firstArm = false;
					cardsToPlay.Add(c);
				}else if (base.IsHead(c) && firstHead)
				{
					firstHead = false;
					cardsToPlay.Add(c);
				} else if (base.IsBody(c) && firstBody)
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
				coroutine = base.GameController.PlayCard(base.TurnTakerController, c, true, cardSource: base.GetCardSource());
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
			coroutine = base.GameController.ShuffleCardsIntoLocation(this.DecisionMaker, cardsToShuffle, this.TurnTaker.Deck, cardSource: base.GetCardSource());
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

	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfFireCardController : SpellCardController
	{
        #region Constructors

        public ElementOfFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		#endregion Constructors

		#region Methods
		public override IEnumerator Play()
		{
			IEnumerator coroutine;
			Card characterCard = base.TurnTaker.FindCard("InfernoTiamatCharacter");
			//If {Tiamat}, The Mouth of the Inferno is active, she deals each hero target 2+X fire damage, where X is the number of Element of Fire cards in the villain trash.
			if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
			{
				coroutine = base.GameController.DealDamage(base.DecisionMaker, characterCard, (Card c) => c.IsHero, PlusNumberOfThisCardInTrash(2), DamageType.Fire, cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			//The hero with the most cards in play...
			List<TurnTaker> storedResults = new List<TurnTaker>();
			coroutine = base.FindHeroWithMostCardsInPlay(storedResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (storedResults.Count<TurnTaker>() > 0)
			{
				//...may not play cards until the start of the next villain turn.
				TurnTaker isSpecificTurnTaker = storedResults.First<TurnTaker>();
				CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
				cannotPlayCardsStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = isSpecificTurnTaker;
				cannotPlayCardsStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
				coroutine = base.AddStatusEffect(cannotPlayCardsStatusEffect);
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

        #endregion Methods
    }
}
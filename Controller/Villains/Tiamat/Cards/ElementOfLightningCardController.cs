using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfLightningCardController : SpellCardController
	{
        #region Constructors

        public ElementOfLightningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		#endregion Constructors

		#region Methods
		public override IEnumerator Play()
		{
			IEnumerator coroutine;
			Card characterCard = base.TurnTaker.FindCard("StormTiamatCharacter");
			//If {Tiamat}, The Eye of the Storm is active, she deals each hero target 2+X lightning damage, where X is the number of Element of Lightning cards in the villain trash.
			if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
			{
				coroutine = base.GameController.DealDamage(this.DecisionMaker, characterCard, (Card c) => c.IsHero, PlusNumberOfThisCardInTrash(2), DamageType.Lightning, cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			//The hero with the most cards in hand...
			List<TurnTaker> storedResults = new List<TurnTaker>();
			coroutine = base.FindHeroWithMostCardsInHand(storedResults, 1, 1, null, null, false, false);
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
				//...may not draw cards until the start of the next villain turn.
				TurnTaker isSpecificTurnTaker = storedResults.First<TurnTaker>();
				PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
				preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = new Phase?(Phase.DrawCard);
				preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = isSpecificTurnTaker;
				preventPhaseActionStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
				coroutine = base.AddStatusEffect(preventPhaseActionStatusEffect, true);
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
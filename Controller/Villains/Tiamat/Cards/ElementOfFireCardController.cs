using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfFireCardController : CardController
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
			Card characterCard = base.TurnTaker.GetCardByIdentifier("InfernoTiamatCharacterCard");
			//If {Tiamat}, The Mouth of the Inferno is active, she deals each hero target 2+X fire damage, where X is the number of Element of Fire cards in the villain trash.
			if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
			{ 
				coroutine = base.GameController.DealDamage(this.DecisionMaker, characterCard, (Card c) => c.IsHero && c.IsTarget, PlusNumberOfCardInTrash(2, "ElementOfFire"), DamageType.Fire);
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
			coroutine = base.FindHeroWithMostCardsInPlay(storedResults, 1, 1, null, null, false, null, false);
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
				coroutine = base.AddStatusEffect(cannotPlayCardsStatusEffect, true);
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

        private int PlusNumberOfCardInTrash(int damage, string identifier)
        {
            return damage + (from card in base.TurnTaker.Trash.Cards
                             where card.Identifier == identifier
                             select card).Count<Card>();
        }

        #endregion Methods
    }
}
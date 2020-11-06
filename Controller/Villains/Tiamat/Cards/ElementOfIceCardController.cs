using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Haka;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class ElementOfIceCardController : CardController
    {
        #region Constructors

        public ElementOfIceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		#endregion Constructors

		#region Methods

		public override IEnumerator Play()
		{
			IEnumerator coroutine;
			Card characterCard = base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacterCard");
			//If {Tiamat}, The Jaws of Winter is active, she deals each hero target 2+X cold damage, where X is the number of Element of Ice cards in the villain trash.
			if (characterCard.IsInPlayAndHasGameText && !characterCard.IsFlipped)
			{
				coroutine = base.GameController.DealDamage(this.DecisionMaker, characterCard, (Card c) => c.IsHero && c.IsTarget, PlusNumberOfCardInTrash(2, "ElementOfIce"), DamageType.Cold);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			//The hero with the highest HP...
			IEnumerable<Card> heroes = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsFlipped));
			Card highestHpHero = heroes.FirstOrDefault();
			int highestHP = Convert.ToInt32(heroes.FirstOrDefault().HitPoints);
			if (base.IsHighestHitPointsUnique((Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsFlipped, 1))
			{
				foreach (Card hero in heroes)
				{
					if (hero.HitPoints >= highestHpHero.HitPoints)
					{
						highestHpHero = hero;
					}
				}
            }
            else
			{
				foreach (Card hero in heroes)
				{
					if (hero.HitPoints >= highestHpHero.HitPoints)
					{
						highestHP = Convert.ToInt32(hero.HitPoints);
					}
				}
				List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
				coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.HeroCharacterCard, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsFlipped && c.HitPoints == highestHP), storedResults, false, false, null, true, null);
				highestHpHero = storedResults.FirstOrDefault().SelectedCard;
            }

			//...may not use powers until the start of the next villain turn.
			CannotUsePowersStatusEffect cannotUsePowersStatusEffect = new CannotUsePowersStatusEffect();
			cannotUsePowersStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = highestHpHero.NativeDeck.OwnerTurnTaker;
			cannotUsePowersStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
			coroutine = base.AddStatusEffect(cannotUsePowersStatusEffect, true);
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

		private int PlusNumberOfCardInTrash(int damage, string identifier)
		{
			return damage + (from card in base.TurnTaker.Trash.Cards
							 where card.Identifier == identifier
							 select card).Count<Card>();
		}

		#endregion Methods
	}
}
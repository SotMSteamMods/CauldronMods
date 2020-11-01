using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SotMWorkshop.Controller.Anathema
{
	public class AnathemaCharacterCardController : VillainCharacterCardController
	{
		public AnathemaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//show the number of villain targets in play
			base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => base.IsVillainTarget(c), "villain", true, false, null, null, false), null, null, null, false).Condition = (() => true);
		}

		//number of villain targets in play other than Anathema
		private int NumberOfVillainTargetsInPlay
		{
			get
			{ 
				return base.FindCardsWhere((Card c) => base.IsVillainTarget(c) && c.IsInPlay && c != base.CharacterCard, false, null, false).Count<Card>();
			}
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

		private bool IsArmHeadOrBody(Card c)
		{
			return IsArm(c) || IsHead(c) || IsBody(c);
		}

		public override void AddSideTriggers()
		{
			//on his front side
			if (!base.Card.IsFlipped)
			{
				//At the start of the Villain Turn, if there are no other villain targets in play, flip Anathema's character card.
				this.SideTriggers.Add(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.StartOfTurnResponse), new TriggerType[]
				{
					TriggerType.FlipCard
				}, null, false));

				//If there are 4 other villain targets in play, Anathema is immune to damage.
				base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction d) => d.Target == base.CharacterCard && this.NumberOfVillainTargetsInPlay == 4, false));

				//Whenever a villain card destroys an arm, body, or head, Anathema regains 2HP.
				base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction destroyCard) => destroyCard.WasCardDestroyed && this.IsArmHeadOrBody(destroyCard.CardToDestroy.Card) && base.GameController.IsCardVisibleToCardSource(destroyCard.CardToDestroy.Card, base.GetCardSource(null)) && destroyCard.CardSource != null && base.IsVillain(destroyCard.CardSource.Card), new Func<DestroyCardAction, IEnumerator>(this.GainHpResponse), TriggerType.GainHP, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false));
				
				if (base.IsGameAdvanced)
				{
					//If there are 3 other villain targets in play, Anathema is immune to damage.
					base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction d) => d.Target == base.CharacterCard && this.NumberOfVillainTargetsInPlay == 3, false));
				}
			}
			else
			{
				//At the start of the Villain Turn, play the top card of the villain deck.
				this.SideTriggers.Add(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.FlippedStartOfTurnResponse), TriggerType.PlayCard, null, false));
				
				//At the end of the Villain Turn, if there are any other villain targets in play, flip Anathema's character card.
				this.SideTriggers.Add(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.FlippedEndOfTurnFlippingResponse), TriggerType.FlipCard, null, false));

				//At the end of the Villain Turn, each player discards a card from their hand.
				this.SideTriggers.Add(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.AllHeroesDiscardResponse), TriggerType.DiscardCard, null, false));
				
				if (base.IsGameAdvanced)
				{
					//At the start of the Villain Turn, each player discards a card from their hand.
					this.SideTriggers.Add(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.AllHeroesDiscardResponse), TriggerType.DiscardCard, null, false));
				}
			}
			base.AddDefeatedIfDestroyedTriggers(false);
		}

		private IEnumerator FlippedEndOfTurnFlippingResponse(PhaseChangeAction phaseChange)
		{
			//if there are any other villain targets in play, flip Anathema's character card.
			if (this.NumberOfVillainTargetsInPlay > 0)
			{
				IEnumerator coroutine = base.GameController.FlipCard(this, false, false, null, null, base.GetCardSource(null), true);
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

		private IEnumerator GainHpResponse(DestroyCardAction dca)
		{
			//Anathema regains 2HP
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(2), null, null);
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

		private IEnumerator AllHeroesDiscardResponse(PhaseChangeAction phaseChange)
		{
			//Each player discards a card from their hand.
			IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(1, new int?(1), null, true, null, false, null, false, base.GetCardSource(null));
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

		private IEnumerator FlippedStartOfTurnResponse(PhaseChangeAction phaseChange)
		{
			//play the top card of the villain deck
			IEnumerator coroutine = base.PlayTheTopCardOfTheVillainDeckResponse(phaseChange);
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

		private IEnumerator StartOfTurnResponse(PhaseChangeAction phaseChange)
		{
			//if there are no other villain targets in play, flip Anathema's character card.
			int numberOfVillainsInPlay = this.NumberOfVillainTargetsInPlay;
			if (numberOfVillainsInPlay == 0)
			{
				IEnumerator coroutine = base.GameController.FlipCard(this, false, false, null, null, base.GetCardSource(null), true);
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

		
	}
}

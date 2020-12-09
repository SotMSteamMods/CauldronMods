using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Anathema
{
	public class AnathemaCharacterCardController : VillainCharacterCardController
	{
		public AnathemaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//show the number of villain targets in play
			base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => base.IsVillainTarget(c), "villain")).Condition = (() => true);
		}

		//number of villain targets in play other than Anathema
		private int NumberOfVillainTargetsInPlay
		{
			get
			{ 
				return base.FindCardsWhere((Card c) => base.IsVillainTarget(c) && c.IsInPlay && c != base.CharacterCard).Count();
			}
		}

		private bool IsArm(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "arm");
		}

		private bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head");
		}

		private bool IsBody(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "body");
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
				this.SideTriggers.Add(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.StartOfTurnResponse), TriggerType.FlipCard));

				//If there are 4 other villain targets in play, Anathema is immune to damage.
				base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction d) => d.Target == base.CharacterCard && this.NumberOfVillainTargetsInPlay == 4));

				//Whenever a villain card destroys an arm, body, or head, Anathema regains 2HP.
				base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction destroyCard) => destroyCard.WasCardDestroyed 
						&& this.IsArmHeadOrBody(destroyCard.CardToDestroy.Card) 
						&& base.GameController.IsCardVisibleToCardSource(destroyCard.CardToDestroy.Card, this.GetCardSource())
						&& destroyCard.WasDestroyedBy(c => IsVillain(c)),
					new Func<DestroyCardAction, IEnumerator>(this.GainHpResponse), TriggerType.GainHP, TriggerTiming.After));
				
				if (base.IsGameAdvanced)
				{
					//If there are 3 other villain targets in play, Anathema is immune to damage.
					base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction d) => d.Target == base.CharacterCard && this.NumberOfVillainTargetsInPlay == 3));
				}
			}
			else
			{
				//At the start of the Villain Turn, play the top card of the villain deck.
				this.SideTriggers.Add(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.FlippedStartOfTurnResponse), TriggerType.PlayCard));
				
				//At the end of the Villain Turn, if there are any other villain targets in play, flip Anathema's character card.
				this.SideTriggers.Add(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.FlippedEndOfTurnFlippingResponse), TriggerType.FlipCard));

				//At the end of the Villain Turn, each player discards a card from their hand.
				this.SideTriggers.Add(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.AllHeroesDiscardResponse), TriggerType.DiscardCard));
				
				if (base.IsGameAdvanced)
				{
					//At the start of the Villain Turn, each player discards a card from their hand.
					this.SideTriggers.Add(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.AllHeroesDiscardResponse), TriggerType.DiscardCard));
				}
			}
			base.AddDefeatedIfDestroyedTriggers();
		}

		private IEnumerator FlippedEndOfTurnFlippingResponse(PhaseChangeAction phaseChange)
		{
			//if there are any other villain targets in play, flip Anathema's character card.
			if (this.NumberOfVillainTargetsInPlay > 0)
			{
				IEnumerator coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
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
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(2));
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
			IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(1, new int?(1),cardSource: base.GetCardSource());
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
			IEnumerator coroutine = base.PlayTheTopCardOfTheVillainDeckWithMessageResponse(phaseChange);
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
				IEnumerator coroutine = base.GameController.FlipCard(this,cardSource: base.GetCardSource());
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

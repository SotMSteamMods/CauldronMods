using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class StSimeonsCatacombsCardController : CharacterCardController
    {
        #region Constructors

        public StSimeonsCatacombsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsUnderCard && c.Location == base.Card.UnderLocation, "under this card"));
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
			base.SetCardProperty("Indestructible", false);
		}

		#endregion Constructors

		#region Methods
		public override void AddTriggers()
		{
			this.AddSideTriggers();
		}

        public override IEnumerator Play()
        {
			//Environment cards cannot be played
			IEnumerator coroutine = this.AddCannotPlayCardsEffect(FindCardController(base.Card));
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


        public override void AddSideTriggers()
		{
			//on  front side
			if (!base.Card.IsFlipped)
			{
				//At the end of the environment turn, put a random room card from beneath this one into play and flip this card
				base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.MoveRandomRoomIntoPlayThenFlip), TriggerType.MoveCard));
			}
			else
			{				
				//Whenever a room card would leave play, instead place it face up beneath this card. Then choose a different room beneath this card and put it into play.
				base.AddTrigger<MoveCardAction>((MoveCardAction mc) => mc.CardToMove.IsRoom && mc.Destination != base.TurnTaker.PlayArea && mc.Destination != base.Card.UnderLocation, new Func<MoveCardAction, IEnumerator>(this.ChangeRoomPostLeaveResponse), TriggerType.MoveCard, TriggerTiming.Before);
				//If you change rooms this way three times in a turn, room cards become indestructible until the end of the turn.
				base.AddTrigger<GameAction>((GameAction ga) => NumberOfRoomsEnteredPlayThisTurn() >= 3  && !base.IsPropertyTrue("Indestructible", null), new Func<GameAction, IEnumerator>(this.SetRoomsIndestructibleResponse), TriggerType.CreateStatusEffect, TriggerTiming.Before);
				//At the end of the environment turn, if no room cards have entered play this turn, you may destroy a room card.
				base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.FreeDestroyRoomResponse), TriggerType.DestroyCard, (PhaseChangeAction pca) => NumberOfRoomsEnteredPlayThisTurn() == 0);
			}
		}

		private IEnumerator MoveRandomRoomIntoPlayThenFlip(PhaseChangeAction pca)
		{
			string message = base.Card.Title + " puts a random room card from beneath this one into play";

			IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//puts a random room card from beneath this one into play
			IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.Card.UnderLocation, false, true, false, new LinqCardCriteria((Card c) => this.IsDefinitionRoom(c), "room"), new int?(1), shuffleBeforehand: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}

			//flips this card
			IEnumerator coroutine3 = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}

			yield break;
		}

		private IEnumerator ChangeRoomPostLeaveResponse(MoveCardAction mc)
		{
			//Whenever a room card would leave play, instead place it face up beneath this card. 
			//Then choose a different room beneath this card and put it into play.
			IEnumerator cancel = base.CancelAction(mc);
			IEnumerator under = base.GameController.MoveCard(base.TurnTakerController, mc.CardToMove, base.Card.UnderLocation, cardSource: base.GetCardSource());
			IEnumerator shuffle = base.GameController.ShuffleLocation(base.Card.UnderLocation, cardSource: base.GetCardSource());
			//Then choose a different room beneath this card and put it into play.
			IEnumerator play = base.GameController.SelectAndPlayCard(this.DecisionMaker, base.Card.UnderLocation.Cards.Where(c => c != mc.CardToMove), isPutIntoPlay: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(cancel);
				yield return base.GameController.StartCoroutine(under);
				yield return base.GameController.StartCoroutine(shuffle);
				yield return base.GameController.StartCoroutine(play);
			}
			else
			{
				base.GameController.ExhaustCoroutine(cancel);
				base.GameController.ExhaustCoroutine(under);
				base.GameController.ExhaustCoroutine(shuffle);
				base.GameController.ExhaustCoroutine(play);
			}
			yield break;
		}

		private IEnumerator SetRoomsIndestructibleResponse(GameAction ga)
		{
			//room cards become indestructible until the end of the turn
			base.SetCardPropertyToTrueIfRealAction("Indestructible", null);
			MakeIndestructibleStatusEffect makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
			makeIndestructibleStatusEffect.CardsToMakeIndestructible.HasAnyOfTheseKeywords = new List<string>() { "room" };
			makeIndestructibleStatusEffect.ToTurnPhaseExpiryCriteria.Phase = new Phase?(Phase.End);
			IEnumerator coroutine = base.AddStatusEffect(makeIndestructibleStatusEffect, true);

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

		private IEnumerator FreeDestroyRoomResponse(PhaseChangeAction pca)
        {
			//you may destroy a room card.
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker,new LinqCardCriteria((Card c) => c.IsRoom, "room"), true, cardSource: base.GetCardSource());
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

		private IEnumerator AddCannotPlayCardsEffect(CardController cardController)
		{
			//create a "Environment Cannot Play Cards" status effect
			//add it to the SideStatusEffect List
			CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
			cannotPlayCardsStatusEffect.TurnTakerCriteria.IsEnvironment = true;
			IEnumerator coroutine3 = base.GameController.AddStatusEffect(cannotPlayCardsStatusEffect, true, cardController.GetCardSource());
			this.SideStatusEffects.Add(cannotPlayCardsStatusEffect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}

			yield break;
		}

		private IEnumerator AddSideStatusEffect()
		{
			//if on the front side, add the environment can't play cards status effect
			if (!base.Card.IsFlipped)
			{
				IEnumerator coroutine = this.AddCannotPlayCardsEffect(FindCardController(base.Card));

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

		private void RemoveSideEffects()
		{
			//remove all status effects in the SideStatusEffectList
			foreach (StatusEffect effect in this.SideStatusEffects)
			{
				base.GameController.StatusEffectManager.RemoveStatusEffect(effect);
			}
			this.SideStatusEffects.Clear();
		}

		public override IEnumerator AfterFlipCardImmediateResponse()
		{
			//On flip, remove all old side trigger, get new triggers for new side
			this.RemoveSideTriggers();
			this.AddSideTriggers();
			//remove all old side status effects, get new status effects
			this.RemoveSideEffects();
			IEnumerator addStatusEffects = this.AddSideStatusEffect();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(addStatusEffects);
			}
			else
			{
				base.GameController.ExhaustCoroutine(addStatusEffects);

			}
			yield break;
		}

		private int NumberOfRoomsEnteredPlayThisTurn()
        {
			int result = (from e in base.GameController.Game.Journal.CardEntersPlayEntriesThisTurn()
						 where this.IsDefinitionRoom(e.Card)
						  select e).Count();

			return result;
		}

		public override bool AskIfCardIsIndestructible(Card card)
		{
			return card == base.Card || card.Location == base.Card.UnderLocation;
		}

		private bool IsDefinitionRoom(Card card)
		{
			return card != null && card.Definition.Keywords.Contains("room");
		}
		private List<StatusEffect> SideStatusEffects = new List<StatusEffect>();
		  
        #endregion Methods
    }
}
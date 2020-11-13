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
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsUnderCard && c.Location == base.Card.UnderLocation, "under this card", false, true));
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
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
			IEnumerator coroutine = this.AddSideEffects(FindCardController(base.Card));
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
				//remove the environment cards can't be played status effect
				this.RemoveSideEffects();
				//Whenever a room card would leave play, instead place it face up beneath this card. Then choose a different room beneath this card and put it into play.
				base.AddTrigger<MoveCardAction>((MoveCardAction mc) => mc.CardToMove.IsRoom && mc.Destination != base.TurnTaker.PlayArea && mc.Destination != base.Card.UnderLocation , new Func<MoveCardAction, IEnumerator>(this.ChangeRoomPostLeaveResponse), TriggerType.MoveCard, TriggerTiming.Before);
				//If you change rooms this way three times in a turn, room cards become indestructible until the end of the turn.
				//At the end of the environment turn, if no room cards have entered play this turn, you may destroy a room card.
			}
		}

        private IEnumerator ChangeRoomPostLeaveResponse(MoveCardAction mc)
        {
			//Whenever a room card would leave play, instead place it face up beneath this card. 
			//Then choose a different room beneath this card and put it into play.
			IEnumerator cancel = base.CancelAction(mc);
			IEnumerator under = base.GameController.MoveCard(base.TurnTakerController, mc.CardToMove, base.Card.UnderLocation, cardSource: base.GetCardSource());
			
			//Then choose a different room beneath this card and put it into play.
			IEnumerator play = base.GameController.SelectAndPlayCard(this.DecisionMaker, base.Card.UnderLocation.Cards.Where(c => c != mc.CardToMove), isPutIntoPlay: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(cancel);
				yield return base.GameController.StartCoroutine(under);
				yield return base.GameController.StartCoroutine(play);
			}
			else
			{
				base.GameController.ExhaustCoroutine(cancel);
				base.GameController.ExhaustCoroutine(under);
				base.GameController.ExhaustCoroutine(play);
			}
			yield break;
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
			IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.Card.UnderLocation, false, true, false, new LinqCardCriteria((Card c) => true, "room"), new int?(1), shuffleBeforehand: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}

			//flips this card
			IEnumerator coroutine3 = base.GameController.FlipCard(this, cardSource: base.GetCardSource()) ;
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

		

		private IEnumerator AddCannotPlayEffect()
		{
			if (!base.Card.IsFlipped)
			{
				IEnumerator coroutine = this.AddSideEffects(FindCardController(base.Card));

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

		public void RemoveSideEffects()
		{
			foreach (StatusEffect effect in this.SideStatusEffects)
			{
				base.GameController.StatusEffectManager.RemoveStatusEffect(effect);
			}
			this.SideStatusEffects.Clear();
		}

		public IEnumerator AddSideEffects(CardController cardController)
		{
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

		private List<StatusEffect> SideStatusEffects = new List<StatusEffect>();

		public override bool AskIfCardIsIndestructible(Card card)
		{ 
			return card == base.Card || card.Location == base.Card.UnderLocation;
		}

		
		
		public override IEnumerator AfterFlipCardImmediateResponse()
		{
			this.RemoveSideTriggers();
			this.AddSideTriggers();
			IEnumerator cantPlayCards = this.AddCannotPlayEffect();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(cantPlayCards);
			}
			else
			{
				base.GameController.ExhaustCoroutine(cantPlayCards);

			}
			yield break;
		}
  
        #endregion Methods
    }
}
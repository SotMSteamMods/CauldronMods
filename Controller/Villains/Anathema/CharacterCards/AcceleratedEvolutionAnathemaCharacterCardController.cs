using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Anathema
{
	public class AcceleratedEvolutionAnathemaCharacterCardController : VillainCharacterCardController
	{
		public AcceleratedEvolutionAnathemaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			
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

		private bool IsArmOrHead(Card c)
		{
			return IsArm(c) || IsHead(c);
		}

		public override void AddSideTriggers()
		{
			//on his front side
			if (!base.Card.IsFlipped)
			{
				//Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.

				//At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
				//Then if there are {H} or more cards under {Anathema}, flip his villain character card.
				
				if (base.IsGameAdvanced)
				{
					//At the end of the villain turn, {Anathema} regains {H - 2} HP.
				}
			}
			else
			{
				//Arm and head cards are indestructible during the villain turn.

				//When explosive transformation enters play, flip {Anathema}'s character cards.


				if (base.IsGameAdvanced)
				{
					//At the end of the villain turn {Anathema} regains 1 HP for each villain target in play.
				}
			}

			base.AddDefeatedIfDestroyedTriggers();
		}

		public override IEnumerator AfterFlipCardImmediateResponse()
		{
			//Remove and Add existing Side Triggers
			IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//When {Anathema} flips to this side, put all cards from underneath him into play. 
			IEnumerable<Card> cardsToMove = base.CharacterCard.UnderLocation.Cards;
			coroutine = GameController.MoveCards(base.TurnTakerController, cardsToMove, (Card c) => new MoveCardDestination(c.Owner.PlayArea), isPutIntoPlay: true, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Shuffle all copies of explosive transformation from the villain trash into the villain deck.
			IEnumerable<Card> cardsToShuffle = FindCardsWhere((Card c) => c.Identifier == "ExplosiveTransformation" && base.TurnTaker.Trash.HasCard(c));
			coroutine = GameController.ShuffleCardsIntoLocation(DecisionMaker, cardsToShuffle, base.TurnTaker.Deck, cardSource: GetCardSource());
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

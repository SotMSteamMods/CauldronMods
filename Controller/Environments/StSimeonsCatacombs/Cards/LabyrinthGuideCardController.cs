﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class LabyrinthGuideCardController : CardController
    {
        #region Constructors

        public LabyrinthGuideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
			//At the start of a hero's turn, that hero may discard 2 cards to destroy a Room in play.
			base.AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, new Func<PhaseChangeAction, IEnumerator>(this.Discard2ToDestroyRoom), new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DestroyCard
            }, (PhaseChangeAction p) => !p.ToPhase.TurnTaker.ToHero().IsIncapacitatedOrOutOfGame);

			//At the start of the environment turn, if Twisting Passages is not in play, this card deals each hero target 1 psychic damage or it is destroyed.
			base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.StartOfEnvironmentResponse), new TriggerType[]
			{
				TriggerType.DealDamage,
				TriggerType.DestroySelf
			});
		}

        private IEnumerator StartOfEnvironmentResponse(PhaseChangeAction arg)
        {
			//if Twisting Passages is not in play
			if (!this.IsTwistingPassagesInPlay())
			{
				List<bool> storedResults = new List<bool>();
				IEnumerator coroutine = base.MakeUnanimousDecision((HeroTurnTakerController hero) => !hero.IsIncapacitatedOrOutOfGame, SelectionType.DealDamage, storedResults: storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (storedResults.Count<bool>() > 0 && storedResults.First<bool>())
				{
					// this card deals each hero target 1 psychic damage .
					IEnumerator coroutine2 = base.DealDamage(base.Card, (Card c) => c.IsHero && c.IsTarget, 1, DamageType.Psychic);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}

				} else
                {
					//or it is destroyed
					IEnumerator coroutine3 = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine3);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine3);
					}
				}
			}
			yield break;
		}

		private IEnumerator Discard2ToDestroyRoom(PhaseChangeAction phaseChange)
		{
			if (phaseChange.ToPhase.TurnTaker.IsHero)
			{
				//...that hero...
				HeroTurnTakerController heroTurnTakerController = base.GameController.FindHeroTurnTakerController(phaseChange.ToPhase.TurnTaker.ToHero());
				if (heroTurnTakerController.NumberOfCardsInHand >= 2)
				{
					//... may discard 2 cards...
					List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
					IEnumerator coroutine = base.SelectAndDiscardCards(heroTurnTakerController, new int?(2), true, storedResults: storedResults);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
					if (base.DidDiscardCards(storedResults, new int?(2)))
					{
						//... to destroy a Room in play.
						IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsRoom, "room"), false, cardSource: base.GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}
					}
				}
				else
				{
					//send message if not enough cards in hand
					IEnumerator coroutine3 = base.GameController.SendMessageAction(heroTurnTakerController.Name + " does not have enough cards in their hand to discard for " + base.Card.Title + ".", Priority.Low, base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine3);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine3);
					}
				}
			}
			yield break;

		}
		private bool IsTwistingPassagesInPlay()
		{
			return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "TwistingPassages").Count() > 0;
		}
	}
        #endregion Methods
}

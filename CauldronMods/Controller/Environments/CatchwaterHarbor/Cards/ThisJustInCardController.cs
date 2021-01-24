using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class ThisJustInCardController : CatchwaterHarborUtilityCardController
    {
        public ThisJustInCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, destroy this card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

        }

		public override IEnumerator Play()
		{
			// When this card enters play, 1 player may discard 3 cards.

			List<SelectTurnTakerDecision> storedTurnTaker = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.DiscardCard, false, false, storedTurnTaker, heroCriteria: new LinqTurnTakerCriteria(tt => GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidSelectTurnTaker(storedTurnTaker))
			{
				HeroTurnTaker htt = GetSelectedTurnTaker(storedTurnTaker).ToHero();
				HeroTurnTakerController hero = FindHeroTurnTakerController(htt);
				List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
				coroutine = SelectAndDiscardCards(hero, 3, optional: true, storedResults: storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				if (DidDiscardCards(storedResults))
				{
					yield break;
				}

			}

			//If no cards are discarded this way, play the top card of the villain deck.
			coroutine = PlayTheTopCardOfTheVillainDeckWithMessageResponse(null);
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

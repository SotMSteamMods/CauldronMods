using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WishCardController : StarlightCardController
    {
        public WishCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
			//"1 player..." 
			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.RevealCardsFromDeck, optional: false, allowAutoDecide: false, storedResults, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame, "active heroes"));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			TurnTaker hero = GetSelectedTurnTaker(storedResults);
			if (hero == null || !hero.IsHero)
			{
				yield break;
			}
			HeroTurnTakerController heroTTC = FindHeroTurnTakerController(hero.ToHero());

			//"...may..."
			YesNoDecision doesSelectedHeroAgree = new YesNoDecision(GameController, heroTTC, SelectionType.RevealCardsFromDeck, cardSource: GetCardSource());
			IEnumerator coroutine2 = GameController.MakeDecisionEvent(doesSelectedHeroAgree);
			if (UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}

			if(GameController.DidAnswerYes(doesSelectedHeroAgree))
            {
				//"...look at the top 5 cards of their deck, put 1 of them into play, then put the rest on the bottom of their deck in any order."
				List<MoveCardDestination> list = new List<MoveCardDestination>();
				list.Add(new MoveCardDestination(heroTTC.TurnTaker.PlayArea));
				list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
				list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
				list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
				list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
				IEnumerator coroutine3 = RevealCardsFromDeckToMoveToOrderedDestinations(DecisionMaker, base.TurnTaker.Deck, list, fromBottom: false, sendCleanupMessageIfNecessary: true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
			}

			yield break;
        }
    }
}
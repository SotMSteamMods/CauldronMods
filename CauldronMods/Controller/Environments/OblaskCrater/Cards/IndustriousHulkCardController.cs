using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class IndustriousHulkCardController : OblaskCraterUtilityCardController
    {
        /*
         * Whenever a hero draws a card, this card deals them 1 melee damage.
         * When this card is destroyed, each player may put the top card of 
         * their deck into their hand.
        */
        public IndustriousHulkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger((DrawCardAction drawCard) => drawCard.IsSuccessful && drawCard.DidDrawCard && IsHero(drawCard.DrawnCard), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            base.AddWhenDestroyedTrigger((dca) => EachPlayerPutsTopCardInHand(), TriggerType.MoveCard);
        }

		private IEnumerator DealDamageResponse(DrawCardAction drawCard)
		{
			List<Card> storedCharacter = new List<Card>();
			IEnumerator coroutine = FindCharacterCardToTakeDamage(drawCard.HeroTurnTaker, storedCharacter, Card, 1, DamageType.Melee);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card card = storedCharacter.FirstOrDefault();
			if (card != null)
			{
				IEnumerator coroutine2 = DealDamage(Card, card, 1, DamageType.Melee, cardSource: GetCardSource());
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

		private IEnumerator EachPlayerPutsTopCardInHand()
        {
			SelectTurnTakersDecision selectTurnTakersDecision = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.MoveCard, allowAutoDecide: true, cardSource: GetCardSource());
			selectTurnTakersDecision.BattleZone = this.BattleZone;
			IEnumerator coroutine2 = GameController.SelectTurnTakersAndDoAction(selectTurnTakersDecision, (TurnTaker hero) => OptionalMoveCardToHand(hero), cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine2);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
        }

		private IEnumerator OptionalMoveCardToHand(TurnTaker hero)
        {
			IEnumerator coroutine;
			if(!hero.Deck.HasCards)
            {
				coroutine = GameController.SendMessageAction($"{hero.Name} has no cards in their deck.", Priority.Medium, GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
				yield break;
			}

			var heroTTC = FindHeroTurnTakerController(hero.ToHero());
			var yesNo = new YesNoDecision(GameController, heroTTC, SelectionType.Custom, cardSource: GetCardSource());
			coroutine = GameController.MakeDecisionAction(yesNo);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			if(DidPlayerAnswerYes(yesNo))
            {
				coroutine = GameController.MoveCard(heroTTC, hero.Deck.TopCard, heroTTC.HeroTurnTaker.Hand, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
        }

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{

			return new CustomDecisionText("Do you want to put the top card of your deck into your hand?", "Should they put the top card of their deck into their hand?", "Vote for if they should put the top card of their deck into their hand?", "put the top card of their deck into their hand");

		}
	}
}

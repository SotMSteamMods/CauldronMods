using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class AbandonedFactoryCardController : CatchwaterHarborUtilityCardController
    {
        public AbandonedFactoryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever an ongoing or equipment card enters a hero trash pile, the villain character target with the lowest HP regains 2HP.
            AddTrigger<MoveCardAction>((MoveCardAction mca) => mca.CardToMove != null && (IsOngoing(mca.CardToMove) || IsEquipment(mca.CardToMove)) && mca.Destination.IsTrash && mca.Destination.IsHero, CardToTrashResponse, TriggerType.GainHP, TriggerTiming.After);

			//When a Transport card would be destroyed, you may destroy this card instead.
			AddTrigger((DestroyCardAction dca) => !this.IsBeingDestroyed && IsTransport(dca.CardToDestroy.Card) && !GameController.IsCardIndestructible(dca.CardToDestroy.Card), DestroyThisCardInsteadResponse, new TriggerType[]
				{
					TriggerType.CancelAction,
					TriggerType.DestroySelf
				}, TriggerTiming.Before);
		}

		private IEnumerator CardToTrashResponse(MoveCardAction arg)
        {
			//villain character target with the lowest HP regains 2HP
			List<Card> storedResults = new List<Card>();
			GainHPAction gameAction = new GainHPAction(base.GameController, null, 2, null);
			IEnumerator coroutine = base.GameController.FindTargetsWithLowestHitPoints(1, 1, (Card c) => IsVillain(c) && c.IsCharacter && c.IsTarget, storedResults, gameAction, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			foreach (Card item in storedResults)
			{
				coroutine = base.GameController.GainHP(item, 2, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
		}

		public IEnumerator DestroyThisCardInsteadResponse(DestroyCardAction dca)
		{
			// Ask player if they want to destroy this card
			List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
			IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker,
				SelectionType.DestroyCard, base.Card, storedResults: storedResults, cardSource: GetCardSource());

			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (!base.DidPlayerAnswerYes(storedResults))
			{
				yield break;
			}
			//do hp checks in case something causes transport to become a target
			coroutine = dca.CardToDestroy.Card.HitPoints == null || !(dca.CardToDestroy.Card.HitPoints.HasValue) || dca.CardToDestroy.Card.HitPoints.Value > 0 ? CancelAction(dca, isPreventEffect: true) : GameController.SendMessageAction($"{dca.CardToDestroy.Card.Title} has less than 0 HP! We can't save it!", Priority.Medium, GetCardSource(), showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = DestroyThisCardResponse(dca);
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

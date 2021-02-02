using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class SearingGazeCardController : TerminusBaseCardController
    {
        /* 
         * {Terminus} deals 6 cold damage to a target that dealt her damage since your last turn.
         * If a non-character target is destroyed this way, you may remove it and this card from 
         * the game.
         */
        public SearingGazeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			base.SpecialStringMaker.ShowDamageTaken(base.CharacterCard, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "targets", useCardsSuffix: false), null, showTotalAmountOfDamageTaken: false, () => true, base.TurnTaker, null, thisRound: false, thisTurn: false, showDamageDealers: true);
		}
		public override bool DoNotMoveOneShotToTrash
		{
			get
			{
				bool moveToTrash = false;
				if (base.Card.IsOutOfGame)
				{
					moveToTrash = true;
				}

				return moveToTrash;
			}
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine;
			SelectCardDecision selectCardDecision;
			IEnumerable<Card> cardChoices = base.GameController.FindTargetsInPlay((card) => HasTargetDealtDamageToTerminusSinceHerLastTurn(card));

			// Select a non-hero target.
			selectCardDecision = new SelectCardDecision(base.GameController, DecisionMaker, SelectionType.DealDamage, cardChoices, cardSource: base.GetCardSource());
			coroutine = base.GameController.SelectCardAndDoAction(selectCardDecision, ActionWithCardResponse);
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

		private IEnumerator ActionWithCardResponse(SelectCardDecision targetCard)
		{
			IEnumerator coroutine;
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
			List<YesNoCardDecision> yesNoCardDecisions = new List<YesNoCardDecision>();

			if (targetCard != null && targetCard.SelectedCard != null)
			{
				// {Terminus} deals 6 cold damage to a target that dealt her damage since your last turn.
				coroutine = base.DealDamage(base.CharacterCard, targetCard.SelectedCard, 6, DamageType.Cold, storedResults: storedResults, cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				// If a non-character target is destroyed this way, you may remove it and this card from the game.
				if (storedResults != null && storedResults.Count() > 0 && !storedResults.FirstOrDefault().Target.IsCharacter && storedResults.FirstOrDefault().DidDestroyTarget)
                {
					coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.RemoveCardFromGame, targetCard.SelectedCard, storedResults: yesNoCardDecisions, cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}

					if (DidPlayerAnswerYes(yesNoCardDecisions))
                    {
						coroutine = base.GameController.MoveCard(base.TurnTakerController, targetCard.SelectedCard, targetCard.SelectedCard.Owner.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}

						coroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
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
			}
			else
            {
				// TODO: May need to notify that nothing happened because there was no damage.
				coroutine = DoNothing();
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

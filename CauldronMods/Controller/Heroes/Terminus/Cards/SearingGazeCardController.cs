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

			// Select a non-hero target.

			var storedDamage = new List<DealDamageAction>();
			// {Terminus} deals 6 cold damage to a target that dealt her damage since your last turn.
			coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 6, DamageType.Cold, 1, false, 1, storedResultsDamage: storedDamage, additionalCriteria: (Card c) => HasTargetDealtDamageToTerminusSinceHerLastTurn(c), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			List<YesNoCardDecision> yesNoCardDecisions = new List<YesNoCardDecision>();
			var damage = storedDamage.FirstOrDefault();
			// If a non-character target is destroyed this way, you may remove it and this card from the game.
			if (damage != null && damage.DidDestroyTarget && !damage.Target.IsCharacter)
            {
				var targetCard = damage.Target;
				coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.RemoveCardFromGame, targetCard, storedResults: yesNoCardDecisions, cardSource: base.GetCardSource());
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
					coroutine = base.GameController.MoveCard(base.TurnTakerController, targetCard, targetCard.Owner.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
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

			yield break;
		}
	}
}

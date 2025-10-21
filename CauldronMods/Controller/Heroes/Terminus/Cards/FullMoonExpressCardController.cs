using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class FullMoonExpressCardController : TerminusBaseCardController
    {
		/*
		 * Whenever {Terminus} is dealt damage by a non-hero target, she may deal that target 2 melee damage. If Stained Badge is not in play, 
		 * you may redirect damage dealt by non-hero targets to {Terminus}.
		 * At the start of your turn, remove this card from the game.
		 */
		public FullMoonExpressCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			base.SpecialStringMaker.ShowIfSpecificCardIsInPlay("StainedBadge");
        }

        public override void AddTriggers()
        {
			base.AddStartOfTurnTrigger((tt) => tt == base.TurnTaker, PhaseChangeActionResponse, TriggerType.RemoveFromGame);
			base.AddRedirectDamageTrigger(RedirectDamageActionCritera, () => base.CharacterCard, true);
			base.AddTrigger<DealDamageAction>((dda) => dda.DidDealDamage && dda.DamageSource != null && dda.DamageSource.IsTarget && !dda.DamageSource.IsHeroTarget && dda.Target == base.CharacterCard, DealDamageActionResponse, TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken);
		}

		private bool RedirectDamageActionCritera(DealDamageAction dealDamageAction)
        {
			Card stainedBadge = base.TurnTaker.FindCard("StainedBadge");

			return !stainedBadge.IsInPlayAndHasGameText && !dealDamageAction.DamageSource.IsHeroTarget && dealDamageAction.DamageSource.IsTarget && dealDamageAction.Target != base.CharacterCard;
		}

		private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
			IEnumerator coroutine;

			coroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
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

		private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
		{
			IEnumerator coroutine;

			coroutine = base.GameController.DealDamageToTarget(new DamageSource(base.GameController, base.CharacterCard), dealDamageAction.DamageSource.Card, 2, DamageType.Melee, optional: true, cardSource: base.GetCardSource());
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

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Quicksilver
{
    public class MalleableArmorCardController : QuicksilverBaseCardController
	{
        public MalleableArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry ddje) => ddje.SourceCard == this.CharacterCard && ddje.Amount > 0).Any() ? "Quicksilver has dealt damage this turn." : "Quicksilver has not dealt damage this turn.");
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"If {Quicksilver} has not dealt damage this turn, she regains 3HP."

            int regains = GetPowerNumeral(0, 3);

            bool hasDealtDamage = Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry ddje) => ddje.SourceCard == this.CharacterCard && ddje.Amount > 0).Any();
            IEnumerator coroutine;
            if (hasDealtDamage)
            {
                coroutine = GameController.SendMessageAction($"{this.CharacterCard.Title} has dealt damage this turn, and does not regain HP.", Priority.High, GetCardSource());
            }
            else
            {
                coroutine = GameController.GainHP(this.CharacterCard, regains, cardSource: GetCardSource());
            }

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
        public override void AddTriggers()
        {
			//If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP, restore her to 1HP.

			//The default "AddWhenHPDropsToZeroOrBelowRestoreHPTriggers" doesn't let you put a criterion on the restore
			//So I'll have to roll my own.

			AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card == this.CharacterCard && this.CharacterCard.HitPoints <= 0 && DestroyIsDueToDamageFromHighHitPoints(dc),
						(DestroyCardAction dc) => PreventDestroyAndRestoreHitPointsResponse(dc, this.CharacterCard, 1),
						new TriggerType[] { TriggerType.CancelAction },
						TriggerTiming.Before);
        }

		private bool DestroyIsDueToDamageFromHighHitPoints(DestroyCardAction dc)
        {
			if(dc.ActionSource != null && dc.ActionSource is DealDamageAction)
            {
				var damageAction = dc.ActionSource as DealDamageAction;
				return damageAction.TargetHitPointsBeforeBeingDealtDamage > 1;
            }
			return false;
        }

		//copy-pasted from ILSpy, then pared down
		private IEnumerator PreventDestroyAndRestoreHitPointsResponse(GameAction action, Card cardThatGetsRestored, int numberOfHitPoints)
		{
			DestroyCardAction destroyCardAction = null;
			DealDamageAction dealDamageAction = null;
			if (action is DealDamageAction)
			{
				dealDamageAction = action as DealDamageAction;
			}
			if (action is DestroyCardAction)
			{
				destroyCardAction = action as DestroyCardAction;
				if (destroyCardAction.ActionSource is DealDamageAction)
				{
					dealDamageAction = destroyCardAction.ActionSource as DealDamageAction;
				}
			}
			if (numberOfHitPoints > 0)
			{
				if ((destroyCardAction != null && destroyCardAction.CardToDestroy.Card == cardThatGetsRestored) || (dealDamageAction != null && dealDamageAction.Target == cardThatGetsRestored))
				{
					IEnumerator coroutine = CancelAction(action);
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine);
					}
				}
				IEnumerator coroutine2;

				if (cardThatGetsRestored.MaximumHitPoints.HasValue)
				{
					numberOfHitPoints = Math.Min(numberOfHitPoints, cardThatGetsRestored.MaximumHitPoints.Value);
				}
				coroutine2 = GameController.SetHP(cardThatGetsRestored, numberOfHitPoints, GetCardSource());
				
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine2);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine2);
				}
			}
			yield break;
		}
    }
}
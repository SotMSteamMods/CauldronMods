using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Quicksilver
{
    public class MalleableArmorCardController : CardController
    {
        private bool _primedToSaveSelf = false;

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
			//If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP...

			//The default "AddWhenHPDropsToZeroOrBelowRestoreHPTriggers" doesn't let you put a criterion on the restore
			//So I'll have to roll my own.

			AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card == this.CharacterCard && this.CharacterCard.HitPoints <= 0 && HitPointsBeforeMostRecentDamage > 1,
						(DestroyCardAction dc) => PreventDestroyAndRestoreHitPointsResponse(dc, this.CharacterCard, null, 1, false, false, false),
						new TriggerType[] { TriggerType.CancelAction },
						TriggerTiming.Before);

			/*
            base.AddTrigger<DealDamageAction>(delegate (DealDamageAction action)
            {
                if (action.Target == base.CharacterCard && base.CharacterCard.HitPoints > 1)
                {
                    int amount = action.Amount;
                    int? hitPoints = action.Target.HitPoints;
                    return amount >= hitPoints.GetValueOrDefault() & hitPoints != null;
                }
                return false;
            }, new Func<DealDamageAction, IEnumerator>(this.DealDamageResponse), new TriggerType[]
            {
                TriggerType.WouldBeDealtDamage,
                TriggerType.GainHP
            }, TriggerTiming.Before);
			*/
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            //I think this has the possibility to interact incorrectly with some esoteric effects.
            //Not sure of a better way to do it, though.
            //...restore her to 1HP.
            base.CharacterCard.SetHitPoints(action.Amount + 1);
            yield break;
        }

		//copy-pasted from ILSpy
		private IEnumerator PreventDestroyAndRestoreHitPointsResponse(GameAction action, Card cardThatGetsRestored, Func<GameAction, IEnumerator> runBeforeRestore, int numberOfHitPoints, bool destroyAfterwards, bool addToHPInsteadOfSet, bool onlyOncePerGame)
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
				if (onlyOncePerGame)
				{
					SetCardPropertyToTrueIfRealAction("OnlyRestoredHitPointsOncePerGame");
				}
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
				if (runBeforeRestore != null)
				{
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(runBeforeRestore(action));
					}
					else
					{
						GameController.ExhaustCoroutine(runBeforeRestore(action));
					}
				}
				IEnumerator coroutine2;
				if (addToHPInsteadOfSet)
				{
					coroutine2 = GameController.GainHP(cardThatGetsRestored, numberOfHitPoints, null, null, GetCardSource());
				}
				else
				{
					if (cardThatGetsRestored.MaximumHitPoints.HasValue)
					{
						numberOfHitPoints = Math.Min(numberOfHitPoints, cardThatGetsRestored.MaximumHitPoints.Value);
					}
					coroutine2 = GameController.SetHP(cardThatGetsRestored, numberOfHitPoints, GetCardSource());
				}
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine2);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (destroyAfterwards)
			{
				IEnumerator coroutine3 = GameController.DestroyCard(DecisionMaker, Card, optional: false, null, null, null, null, null, null, null, null, GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine3);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine3);
				}
			}
		}

		private int HitPointsBeforeMostRecentDamage
        {
            get
            {
                DealDamageJournalEntry lastDamage = Journal.MostRecentDealDamageEntry((DealDamageJournalEntry dc) => dc.TargetCard == base.CharacterCard);
                if (lastDamage != null && lastDamage.TargetCardsHitPointsAfterDamage != null)
                {
                    return lastDamage.Amount + (int)lastDamage.TargetCardsHitPointsAfterDamage;
                }
                return 0;
            }
        }
    }
}
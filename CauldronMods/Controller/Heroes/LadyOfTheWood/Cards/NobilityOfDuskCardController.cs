using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
    public class NobilityOfDuskCardController : CardController
    {
        public NobilityOfDuskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} can increase damage this turn.");
            ss.Condition = IsBuffAvailable;

            this.RunModifyDamageAmountSimulationForThisCard = false;
        }

        public override void AddTriggers()
        {
            //Once per turn when LadyOfTheWood would deal damage you may increase that damage by 2
            Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => IsBuffAvailable() && dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard);
            base.AddTrigger<DealDamageAction>(criteria, this.IncreaseDamageDecision, TriggerType.ModifyDamageAmount, TriggerTiming.Before, isActionOptional: true);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("BuffUsed"), TriggerType.Hidden);
        }

        private bool IsBuffAvailable()
        {
            return Game.Journal.CardPropertiesEntriesThisTurn(Card).Any(j => j.Key == "BuffUsed") != true;
        }

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (IsBuffAvailable())
                {
                    return false;
                }
                //if we've used the trigger this turn then we can use the default
                return base.AllowFastCoroutinesDuringPretend;
            }
        }

        private IEnumerator IncreaseDamageDecision(DealDamageAction dd)
        {
            //Offer the player a yes/no decision if they want to increase that damage by 2
            //This currently doesn't have any text on the decision other than yes/no, room for improvement

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.IncreaseNextDamage, base.Card,
                                        action: dd,
                                        storedResults: storedResults,
                                        cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidPlayerAnswerYes(storedResults))
            {
                //if player said yes, set BuffUsed to true and Increase Damage
                base.SetCardPropertyToTrueIfRealAction("BuffUsed");
                coroutine = base.GameController.IncreaseDamage(dd, 2, cardSource: base.GetCardSource());
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

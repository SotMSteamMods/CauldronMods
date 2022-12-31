using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public abstract class GlyphCardController : TheStrangerBaseCardController
    {
        protected GlyphCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} can prevent damage this turn.");
            ss.Condition = IsPreventionAvailable;
        }

        private bool IsPreventionAvailable()
        {
            return Game.ActiveTurnTaker == this.TurnTaker &&  Game.Journal.CardPropertiesEntriesThisTurn(Card).Any(j => j.Key == "PreventionUsed") != true;
        }

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (IsPreventionAvailable())
                {
                    return false;
                }
                //if it's not Stranger turn or we've used the triggers then we can use the default
                return base.AllowFastCoroutinesDuringPretend;
            }
        }

        public override void AddTriggers()
        {
            // Once during your turn when The Stranger would deal himself damage, prevent that damage.
            base.AddTrigger<DealDamageAction>(dda => IsPreventionAvailable() && dda.DamageSource.IsSameCard(base.CharacterCard) && dda.Target == base.CharacterCard, this.DamagePreventionResponse, TriggerType.CancelAction, TriggerTiming.Before, isActionOptional: true);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("PreventionUsed"), TriggerType.Hidden);
        }

        private IEnumerator DamagePreventionResponse(DealDamageAction dd)
        {
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            List<Card> list = new List<Card>();
            list.Add(base.Card);
            IEnumerator coroutine2 = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.PreventDamage, base.Card, dd, storedResults, list, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            YesNoCardDecision yesNoCardDecision = storedResults.FirstOrDefault();
            if (yesNoCardDecision.Answer != null && yesNoCardDecision.Answer.Value)
            {
                
                base.SetCardPropertyToTrueIfRealAction("PreventionUsed");
                coroutine2 = base.CancelAction(dd, true, true, null, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            yield break;
        }
    }
}
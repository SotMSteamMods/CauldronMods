using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class GlyphCardController : CardController
    {
        #region Constructors

        public GlyphCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SetCardProperty(base.GeneratePerTargetKey("PreventionUsed", base.CharacterCard, null), false);

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            // Once during your turn when The Stranger would deal himself damage, prevent that damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => dealDamage.DamageSource.IsSameCard(base.CharacterCard) && dealDamage.Target == base.CharacterCard && !base.CharacterCardController.IsPropertyTrue("PreventionUsed", null), new Func<DealDamageAction, IEnumerator>(this.PreventDamageDecision), TriggerType.CancelAction, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);

            //Reset the PreventionUsed property to false at the start of the turn
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt.CharacterCard == base.CharacterCard, new Func<PhaseChangeAction, IEnumerator>(this.ResetPreventionUsed), TriggerType.Other, null, false);
        }

        private IEnumerator PreventDamageDecision(DealDamageAction dd)
        {
			//Offer the player a yes/no decision if they want to prevent the damage
			//This currently doesn't have any text on the decision other than yes/no, room for improvement
			List<Card> list = new List<Card>();
			list.Add(base.Card);
			YesNoDecision yesNo = new YesNoDecision(base.GameController, base.HeroTurnTakerController, SelectionType.PreventDamage, false, dd, list, base.GetCardSource(null));
			IEnumerator coroutine = base.GameController.MakeDecisionAction(yesNo, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//if player said yes, set BuffUsed to true and prevent the damage
			if (yesNo.Answer != null && yesNo.Answer.Value)
			{
				base.CharacterCardController.SetCardPropertyToTrueIfRealAction("PreventionUsed", null);
				IEnumerator coroutine2 = base.CancelAction(dd, true, true, null, true);
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

        private IEnumerator ResetPreventionUsed(PhaseChangeAction pa)
        {
            base.CharacterCardController.SetCardProperty("PreventionUsed", false);
            yield break;
        }
        #endregion Methods
    }
}
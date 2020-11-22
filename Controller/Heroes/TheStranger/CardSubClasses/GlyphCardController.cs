using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class GlyphCardController : CardController
    {
        #region Constructors

        public GlyphCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SetCardProperty("PreventionUsed", false);
			this.AllowFastCoroutinesDuringPretend = false;
		}

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            // Once during your turn when The Stranger would deal himself damage, prevent that damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => dealDamage.DamageSource.IsSameCard(base.CharacterCard) && dealDamage.Target == base.CharacterCard && !base.IsPropertyTrue("PreventionUsed"), this.DamagePreventionResponse, TriggerType.CancelAction, TriggerTiming.Before);

            //Reset the PreventionUsed property to false at the start of the turn
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt.CharacterCard == base.CharacterCard, new Func<PhaseChangeAction, IEnumerator>(this.ResetPreventionUsed), TriggerType.Other, null, false);
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
			YesNoCardDecision yesNoCardDecision = storedResults.FirstOrDefault<YesNoCardDecision>();
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

		private IEnumerator ResetPreventionUsed(PhaseChangeAction pa)
        {
            base.CharacterCardController.SetCardProperty("PreventionUsed", false);
            yield break;
        }

        protected bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }

        protected bool IsGlyph(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "glyph", false, false);
        }
        #endregion Methods
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class FutureFocusCardController : DriftFocusUtilityCardController
    {
        public FutureFocusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, return all other focus cards to your hand.
        /**Added to FocusUtilityCardController**/


        //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftRRR}. If you shifted {DriftRRR} this way, {Drift} deals 1 target 3 radiant damage.
        /**Trigger added to FocusUtilityCardController**/

        public override IEnumerator ShiftResponse(DealDamageAction action)
        {
            List<YesNoCardDecision> decision = new List<YesNoCardDecision>();
            //...you may shift {DriftRRR}. 
            customMode = CustomMode.AskToShift;
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.Custom, this.Card, action, decision, new Card[] { base.GetShiftTrack() }, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (decision.FirstOrDefault().Answer ?? false)
            {
                var shiftCounter = new List<bool>();
                coroutine = base.ShiftRRR(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //If you shifted {DriftRRR} this way, {Drift} deals 1 target 3 radiant damage.
                if (shiftCounter.Count() >= 3)
                {
                    coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), 3, DamageType.Radiant, 1, false, 1, cardSource: base.GetCardSource());
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (customMode == CustomMode.AskToShift)
            {
                return new CustomDecisionText("Do you want to shift {{ShiftRRR}}?", "Should they shift {{ShiftRRR}}?", "Vote for if they should shift {{ShiftRRR}}", "shifting {{ShiftRRR}}");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}

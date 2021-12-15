using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class PastFocusCardController : DriftFocusUtilityCardController
    {
        public PastFocusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, return all other focus cards to your hand.
        /**Added to FocusUtilityCardController**/

        //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftLLL}. If you shifted {DriftLLL} this way, you may play a card.
        /**Trigger added to FocusUtilityCardController**/

        public override IEnumerator ShiftResponse(DealDamageAction action)
        {
            List<YesNoCardDecision> decision = new List<YesNoCardDecision>();
            //...you may shift {DriftLLL}. 
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
                coroutine = base.ShiftLLL(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //If you shifted {DriftLLL} this way, you may play a card.
                if (shiftCounter.Count() >= 3)
                {
                    coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: GetCardSource());
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
                string ShiftLLL = "{{ShiftLLL}}";
                return new CustomDecisionText($"Do you want to shift {ShiftLLL}?", $"Should they shift {ShiftLLL}?", $"Vote for if they should shift {ShiftLLL}", $"shifting {ShiftLLL}");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}

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
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.MakeDecision, this.Card, action, decision, new Card[] { base.GetShiftTrack() }, base.GetCardSource());
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
                coroutine = base.ShiftLLL();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //If you shifted {DriftLLL} this way, you may play a card.
                if (base.TotalShifts >= 3)
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
    }
}

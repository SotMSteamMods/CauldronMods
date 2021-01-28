using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DriftStepCardController : DriftUtilityCardController
    {
        public DriftStepCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Shift {DriftL} or {DriftR}.
            //If you shifted {DriftL} this way, {Drift} regains 1 HP. If you shifted {DriftR} this way, {Drift} deals 1 target 1 radiant damage.
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Shift Left", SelectionType.RemoveTokens, () => this.ShiftResponse(0)),
                    new Function(base.HeroTurnTakerController, "Shift Right", SelectionType.AddTokens, () => this.ShiftResponse(1))
            });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Draw a card. 
            coroutine = base.DrawCard();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may play a card.
            coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: base.GetCardSource());
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

        private IEnumerator ShiftResponse(int response)
        {
            //Shift {DriftL} or {DriftR}.
            if (response == 0)
            {
                //Shift {DriftL}
                IEnumerator coroutine = base.ShiftL();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //If you shifted {DriftL} this way, {Drift} regains 1 HP.
                if (base.TotalShifts >= 1)
                {
                    coroutine = base.GameController.GainHP(base.GetActiveCharacterCard(), 1);
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
            else
            {
                //Shift {DriftR}
                IEnumerator coroutine = base.ShiftR();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //If you shifted {DriftR} this way, {Drift} deals 1 target 1 radiant damage.
                if (base.TotalShifts >= 1)
                {
                    coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), 1, DamageType.Radiant, 1, false, 1, cardSource: base.GetCardSource());
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

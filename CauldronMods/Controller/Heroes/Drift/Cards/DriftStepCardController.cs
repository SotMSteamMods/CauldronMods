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
                    new Function(base.HeroTurnTakerController, "Drift Left", SelectionType.PlayCard, () => this.ShiftResponse(0)),
                    new Function(base.HeroTurnTakerController, "Drift Right", SelectionType.PlayCard, () => this.ShiftResponse(1))
            });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Draw a card. You may play a card.
            yield break;
        }

        private IEnumerator ShiftResponse(int response)
        {
            //Shift {DriftL} or {DriftR}.
            if (response == 0)
            {
                bool canShift = base.CurrentShiftPosition() > 1;
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
                if (canShift)
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

                bool canShift = base.CurrentShiftPosition() < 4;
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
                if (canShift)
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

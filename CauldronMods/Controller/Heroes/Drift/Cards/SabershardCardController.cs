using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class SabershardCardController : DriftUtilityCardController
    {
        public SabershardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {

            IEnumerator coroutine;
            //{DriftPast} 
            if (base.IsTimeMatching(Past))
            {
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

                //Shift {DriftR}.
                coroutine = base.ShiftR();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{DriftFuture} 
            if (base.IsTimeMatching(Future))
            {
                int targetNumeral = base.GetPowerNumeral(0, 1);
                int damageNumeral = base.GetPowerNumeral(1, 2);

                //{Drift} 1 target 2 radiant damage.
                coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), damageNumeral, DamageType.Radiant, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftL}.
                coroutine = base.ShiftL();
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

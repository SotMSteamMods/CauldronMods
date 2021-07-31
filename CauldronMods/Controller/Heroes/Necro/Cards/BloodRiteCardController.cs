using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Necro
{
    public class BloodRiteCardController : NecroCardController
    {
        public BloodRiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        public override void AddTriggers()
        {
            //When an Undead target is destroyed, all non-undead hero targets regain 2 HP.
            AddUndeadDestroyedTrigger(GainHPResponse, TriggerType.GainHP);
        }

        private IEnumerator GainHPResponse(DestroyCardAction dca)
        {
            //all non-undead hero targets regain 2 HP.
            int powerNumeral = base.GetPowerNumeral(0, 2);
            IEnumerator coroutine = base.GameController.GainHP(base.HeroTurnTakerController, c => IsHeroTargetConsidering1929(c) && !this.IsUndead(c), powerNumeral, cardSource: base.GetCardSource());
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
    }
}

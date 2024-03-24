using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class TranslocationAcceleratorCardController : CardController
    {
        public TranslocationAcceleratorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<UsePowerAction>(upa => upa.HeroUsingPower == this.HeroTurnTakerController && upa.IsSuccessful, UsePowerResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator UsePowerResponse(UsePowerAction action)
        {
            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Energy, 1, false, 0,
                                additionalCriteria: c => !IsHeroTarget(c) && c.IsTarget && c.IsInPlayAndHasGameText,
                                cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
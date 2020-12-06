using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class SwarmingFrequencyCardController : CardController
    {
        public SwarmingFrequencyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            //If there is at least 1 environment target in play, redirect all damage dealt by villain targets to the environment target with the lowest HP.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => base.FindCardsWhere((Card c) => c.IsEnvironmentTarget && c.IsInPlayAndHasGameText).Any() && action.DamageSource.IsVillainTarget, this.RedirectDamageResponse, new TriggerType[] { TriggerType.RedirectDamage }, TriggerTiming.Before);
            //At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction action)
        {
            List<Card> list = new List<Card>();
            //...redirect all damage dealt by villain targets to the environment target with the lowest HP.
            IEnumerator coroutine = base.RedirectDamage(action, TargetType.LowestHP, (Card c) => c.IsEnvironmentTarget && c.IsInPlayAndHasGameText);
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
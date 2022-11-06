using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class HurledObstructionCardController : CardController
    {
        public HurledObstructionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} deals up to 3 targets 1 projectile damage each.",
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), 1, DamageType.Projectile, 3, false, 0, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt by villain targets by 1.",
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsTarget && IsVillainTarget(dd.DamageSource.Card), dd => 1);

            //"At the start of your turn, destroy this card."
            AddStartOfTurnTrigger(tt => tt == this.TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}
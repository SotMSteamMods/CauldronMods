using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron
{
    public class LivingReactorCardController : CardController
    {
        public LivingReactorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever {Gray} deals damage to a hero target, either increase that damage by 1 or that player must discard a card.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target.IsHero && action.DamageSource.Card == base.CharacterCard, IncreaseOrDiscardResponse, new TriggerType[] { TriggerType.IncreaseDamage, TriggerType.DiscardCard }, TriggerTiming.Before);
        }

        private IEnumerator IncreaseOrDiscardResponse(DealDamageAction action)
        {
            HeroTurnTakerController damageSource = base.FindHeroTurnTakerController(action.DamageSource.Card.Owner.ToHero();
            //either increase that damage by 1 or that player must discard a card.
            IEnumerator coroutine = base.SelectAndPerformFunction(damageSource, new Function[]
            {
                //increase that damage by 1
                new Function(this.DecisionMaker, "increase this damage by 1", SelectionType.IncreaseDamage, () => base.GameController.IncreaseDamage(action, (DealDamageAction dda) => 1, base.GetCardSource())),
                //that player must discard a card
                new Function(damageSource, "discard a card", SelectionType.DiscardCard, () => base.SelectAndDiscardCards(damageSource, new int?(1)), damageSource.HasCardsInHand)
            });
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
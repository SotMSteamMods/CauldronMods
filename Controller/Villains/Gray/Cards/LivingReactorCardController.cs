using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Gray
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
            HeroTurnTakerController target = base.FindHeroTurnTakerController(action.Target.Owner.ToHero());
            //either increase that damage by 1 or that player must discard a card.
            IEnumerator coroutine2 = base.SelectAndPerformFunction(target, new Function[]
            {
                //increase that damage by 1
                new Function(this.DecisionMaker, "increase this damage by 1", SelectionType.IncreaseDamage, () => base.GameController.IncreaseDamage(action, 1, cardSource: base.GetCardSource())),
                //that player must discard a card
                new Function(target, "discard a card", SelectionType.DiscardCard, () => base.GameController.SelectAndDiscardCards(target, new int?(1), false, new int?(1), cardSource: base.GetCardSource()), target.HasCardsInHand)
            });
            List<Function> functions = new List<Function> {
                new Function(target, "increase this damage by 1", SelectionType.IncreaseDamage, () => base.GameController.IncreaseDamage(action, 1, cardSource: base.GetCardSource())),
               new Function(target, "discard a card", SelectionType.DiscardCard, () => base.GameController.SelectAndDiscardCards(target, new int?(1), false, new int?(1), cardSource: base.GetCardSource()), target.HasCardsInHand)
            };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, target, functions, false, cardSource: base.GetCardSource(null));
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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
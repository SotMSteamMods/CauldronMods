using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
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
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => IsHeroTarget(action.Target) && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard && !action.IsPretend && action.IsSuccessful, IncreaseOrDiscardResponse, new TriggerType[] { TriggerType.IncreaseDamage, TriggerType.DiscardCard }, TriggerTiming.Before);
        }

        private IEnumerator IncreaseOrDiscardResponse(DealDamageAction action)
        {
            HeroTurnTakerController target = base.FindHeroTurnTakerController(action.Target.Owner.ToHero());
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = base.GameController.SelectAndDiscardCard(base.FindHeroTurnTakerController(action.Target.Owner.ToHero()),optional: true, storedResults: storedResults, dealDamageInfo: action.ToEnumerable(), cardSource: GetCardSource()) ;
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(storedResults, 0))
            {
                coroutine = base.GameController.IncreaseDamage(action, 1, cardSource: base.GetCardSource());
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

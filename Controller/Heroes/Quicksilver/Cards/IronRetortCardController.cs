using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class IronRetortCardController : CardController
    {
        public IronRetortCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, draw a card...
            IEnumerator coroutine = base.DrawCard();
            //...and {Quicksilver} regains 2HP.
            IEnumerator coroutine2 = base.GameController.GainHP(base.CharacterCard, 2, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //When {Quicksilver} is dealt damage, you may destroy this card...
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.CharacterCard && !action.IsPretend && action.Amount > 0 && !base.IsBeingDestroyed, new Func<DealDamageAction, IEnumerator>(this.DestroySelfResponse), new TriggerType[]
            {
                TriggerType.WouldBeDealtDamage,
                TriggerType.DestroySelf
            }, TriggerTiming.Before);
            //... If you do, you may play a card.
            base.AddWhenDestroyedTrigger((DestroyCardAction action) => base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true), TriggerType.DealDamage);
        }
        private IEnumerator DestroySelfResponse(DealDamageAction action)
        {
            IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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
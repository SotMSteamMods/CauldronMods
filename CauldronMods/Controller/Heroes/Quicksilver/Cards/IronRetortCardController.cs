using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class IronRetortCardController : QuicksilverBaseCardController
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
            //When {Quicksilver} is dealt damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.CharacterCard && !dd.IsPretend && dd.DidDealDamage && dd.Amount > 0 && !base.IsBeingDestroyed, DestroySelfResponse, new TriggerType[]
             {
                TriggerType.DestroySelf
             }, TriggerTiming.After, new ActionDescription[] { ActionDescription.DamageTaken });
        }
        private IEnumerator DestroySelfResponse(DealDamageAction action)
        {
            //... you may destroy this card..
            IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card,
                //... If you do, you may play a card.
                postDestroyAction: () => base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true),
                optional: true,
                cardSource: base.GetCardSource());
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
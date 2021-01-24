using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class HarborCraneCardController : CatchwaterHarborUtilityCardController
    {
        public HarborCraneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever this card is dealt damage by a target, move it next to that target.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == Card && dd.DamageSource.Card.IsTarget && !IsThisCardNextToCard(dd.DamageSource) && dd.DidDealDamage, NextToResponse, TriggerType.MoveCard, TriggerTiming.After);

            //Increase damage dealt by the target next to this card by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && GetCardThisCardIsNextTo() != null && dd.DamageSource.Card == GetCardThisCardIsNextTo(), 1);

            //When this card is destroyed, it deals the target next to it 5 melee damage
            AddWhenDestroyedTrigger(DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, additionalCriteria: (DestroyCardAction dca) => GetCardThisCardIsNextTo() != null);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine = DealDamage(Card, GetCardThisCardIsNextTo(), 5, DamageType.Melee, cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator NextToResponse(DealDamageAction dd)
        {
            //move it next to that target that dealt damage
            Card card = dd.DamageSource.Card;
            IEnumerator coroutine = GameController.MoveCard(base.TurnTakerController, base.Card, card.NextToLocation,
                playCardIfMovingToPlayArea: false,
                    actionSource: dd,
                    cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

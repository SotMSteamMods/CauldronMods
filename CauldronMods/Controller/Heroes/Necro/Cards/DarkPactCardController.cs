using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Necro
{
    public class DarkPactCardController : NecroCardController
    {
        public DarkPactCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            return FindAndUpdateUndead();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Put an undead card from hand into play.
            IEnumerator coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false,
                cardCriteria: new LinqCardCriteria((Card c) => this.IsUndead(c), "undead"),
                isPutIntoPlay: true,
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

        public override void AddTriggers()
        {
            //Whenever an undead target is destroyed, draw a card.
            AddUndeadDestroyedTrigger(DrawCardResponse, TriggerType.DrawCard);
            // When the ritual leaves play, update undead HPs
            AddWhenDestroyedTrigger(RitualOnDestroyResponse, new TriggerType[] { TriggerType.PlayCard });
        }

        private IEnumerator DrawCardResponse(DestroyCardAction dca)
        {
            //draw a card
            IEnumerator coroutine = base.GameController.DrawCard(base.HeroTurnTaker);
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

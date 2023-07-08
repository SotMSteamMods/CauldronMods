using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class ThievingCardController : NatureCardController
    {
        public ThievingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever the target next to this card deals damage to a hero, that hero must discard a card.
            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && GetCardThisCardIsBelow() != null && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == GetCardThisCardIsBelow() && dd.Target != null && IsHeroCharacterCard(dd.Target), DiscardCardResponse, TriggerType.DiscardCard, TriggerTiming.After);
            base.AddTriggers();
        }

        private IEnumerator DiscardCardResponse(DealDamageAction dd)
        {
            HeroTurnTakerController heroTurnTakerController = base.GameController.FindHeroTurnTakerController(dd.Target.Owner.ToHero());
            if (heroTurnTakerController.NumberOfCardsInHand < 1)
            {
                yield break;
            }
            IEnumerator coroutine = SelectAndDiscardCards(heroTurnTakerController, 1, gameAction: dd);
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

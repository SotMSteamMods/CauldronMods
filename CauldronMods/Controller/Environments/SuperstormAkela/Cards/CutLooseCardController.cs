using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class CutLooseCardController : SuperstormAkelaCardController
    {

        public CutLooseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase all damage dealt by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => true, 1);

            //When a hero destroys a target, this card deals that hero {H} projectile damage and is destroyed
            AddTrigger<DestroyCardAction>((DestroyCardAction destroy) => destroy.CardSource != null && IsHeroCharacterCard(destroy.CardSource.Card) && destroy.CardToDestroy != null && destroy.CardToDestroy.Card.IsTarget && destroy.WasCardDestroyed,
                DealDamageAndDestroyThisCardResponse, new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.DestroySelf
                }, TriggerTiming.After);
        }

        private IEnumerator DealDamageAndDestroyThisCardResponse(DestroyCardAction destroy)
        {
            //this card deals that hero { H} projectile damage 
            Card hero = destroy.CardSource.Card;
            IEnumerator coroutine = DealDamage(base.Card, hero, Game.H, DamageType.Projectile, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //and is destroyed
            coroutine = DestroyThisCardResponse(destroy);
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
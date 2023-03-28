using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class SmoothCriminalCardController : CatchwaterHarborUtilityCardController
    {
        public SmoothCriminalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsTransport(c), "transport"));
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to Gangsters by 1.
            AddReduceDamageTrigger((Card c) => IsGangster(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);

            //At the end of the environment turn, this card deals each hero target X projectile damage, where X is 1 plus the number of Transports in play.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //this card deals each hero target X projectile damage, where X is 1 plus the number of Transports in play.
            IEnumerator coroutine = DealDamage(Card, (Card c) => IsHero(c), (Card c) => 1 + GetNumberOfTransportsInPlay(), DamageType.Projectile);
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

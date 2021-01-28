using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class FrightenedOnlookersCardController : CatchwaterHarborUtilityCardController
    {
        public FrightenedOnlookersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever a target is dealt 4 or more damage from a single source, this card deals itself 1 projectile damage.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Amount >= 4, DealSelfDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the start of the environment turn, 1 player may play a card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pca) => SelectHeroToPlayCard(DecisionMaker), TriggerType.PlayCard);
        }

        private IEnumerator DealSelfDamageResponse(DealDamageAction arg)
        {
            //this card deals itself 1 projectile damage.
            IEnumerator coroutine = DealDamage(Card, Card, 1, DamageType.Projectile, cardSource: GetCardSource());
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

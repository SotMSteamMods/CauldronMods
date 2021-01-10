using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class GulCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public GulCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the lowest HP 3 infernal damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, 3, DamageType.Infernal);
            //Whenever this card destroys a target, it deals each target other than itself 1 cold damage.
            AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card.IsTarget && dca.CardSource != null && dca.CardSource.Card == Card, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction arg)
        {
            IEnumerator coroutine = DealDamage(Card, (Card c) => c != Card && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, DamageType.Cold);
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

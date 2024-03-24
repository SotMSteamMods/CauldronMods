using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class ReturnPulseCardController : CardController
    {
        public ReturnPulseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> targets = new List<DealDamageAction>();
            //{Cricket} deals up to 3 non-hero targets 1 sonic damage each.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Sonic, 3, false, 0, additionalCriteria: (Card c) => !IsHeroTarget(c), storedResultsDamage: targets, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //For each target dealt damage this way, draw a card.
            coroutine = base.DrawCards(base.HeroTurnTakerController, targets.Where((DealDamageAction action) => action.DidDealDamage).Count());
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
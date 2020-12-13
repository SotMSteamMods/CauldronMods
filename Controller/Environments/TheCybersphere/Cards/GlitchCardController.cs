using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class GlitchCardController : TheCybersphereCardController
    {

        public GlitchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 2);
        }


        public override void AddTriggers()
        {
            //When this card is destroyed, it deals the non-environment target with the second highest HP 5 lightning damage.
            AddWhenDestroyedTrigger(DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction dca)
        {
            //it deals the non-environment target with the second highest HP 5 lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 2, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), (Card c) => new int?(5), DamageType.Lightning);
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
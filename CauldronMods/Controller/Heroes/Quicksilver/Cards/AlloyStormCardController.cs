using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class AlloyStormCardController : QuicksilverBaseCardController
    {
        public AlloyStormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{Quicksilver} deals each non-hero target 1 projectile damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => !IsHeroTarget(c), 1, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //You may play a Finisher, or {Quicksilver} may deal herself 2 melee damage and play a Combo.
            coroutine = this.ComboOrFinisherResponse();
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
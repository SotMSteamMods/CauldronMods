using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Necro
{
    public class HellfireCardController : NecroCardController
    {
        public HellfireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //When an Undead target is destroyed, Necro deals 1 non-hero target 3 infernal damage.
            AddUndeadDestroyedTrigger(DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction dca)
        {
            //Necro deals 1 non - hero target 3 infernal damage.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Infernal, 1, false, 1,
                additionalCriteria: c => !IsHeroTargetConsidering1929(c),
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
    }
}

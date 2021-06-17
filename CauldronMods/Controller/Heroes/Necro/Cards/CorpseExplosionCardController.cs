using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Necro
{
    public class CorpseExplosionCardController : NecroCardController
    {
        public CorpseExplosionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //When an Undead target is destroyed, Necro deals 2 toxic damage to all villain targets.
            AddUndeadDestroyedTrigger(DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction dca)
        {
            //Necro deals 2 toxic damage to all villain targets.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => IsVillianTargetConsidering1929(card), 2, DamageType.Toxic);
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

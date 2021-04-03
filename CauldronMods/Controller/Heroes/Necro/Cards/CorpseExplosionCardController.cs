using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Necro
{
    public class CorpseExplosionCardController : NecroCardController
    {
        public CorpseExplosionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            return FindAndUpdateUndead();
        }

        public override void AddTriggers()
        {
            //When an Undead target is destroyed, Necro deals 2 toxic damage to all villain targets.
            AddUndeadDestroyedTrigger(DealDamageResponse, TriggerType.DealDamage);
            // When the ritual leaves play, update undead HPs
            AddWhenDestroyedTrigger(RitualOnDestroyResponse, new TriggerType[] { TriggerType.PlayCard });
        }

        private IEnumerator DealDamageResponse(DestroyCardAction dca)
        {
            //Necro deals 2 toxic damage to all villain targets.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => IsVillianConsidering1929(card) && card.IsTarget, 2, DamageType.Toxic);
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

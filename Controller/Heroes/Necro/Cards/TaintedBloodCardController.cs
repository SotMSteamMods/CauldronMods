using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Necro
{
    public class TaintedBloodCardController : NecroCardController
    {
        public TaintedBloodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        public override void AddTriggers()
        {
            //At the end of your draw phase, Necro deals the undead target with the lowest HP 2 irreducible toxic damage.
            base.AddPhaseChangeTrigger(tt => tt == base.HeroTurnTaker, p => p == Phase.End, _ => true, DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.Before);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //Necro deals the undead target with the lowest HP 2 irreducible toxic damage.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, c => this.IsUndead(c), _ => 2, DamageType.Toxic, isIrreducible: true);
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

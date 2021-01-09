using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class PercussiveWaveCardController : ScreaMachineUtilityCardController
    {
        public PercussiveWaveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.RickyG, ScreaMachineBandmate.Value.Slice };

        public override IEnumerator Play()
        {
            var coroutine = base.ActivateBandAbilities(AbilityIcons);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddReduceDamageTrigger(dda => IsVillainTarget(dda.Target), dda => dda.Amount >= 4 ? 2 : dda.Amount == 3 ? 1 : 0);
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}

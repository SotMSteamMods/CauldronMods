using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class TectonicBeatCardController : ScreaMachineBandCardController
    {
        public TectonicBeatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.RickyG)
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {
            var effect = new ReduceDamageStatusEffect(1);
            effect.TargetCriteria.IsSpecificCard = GetBandmate();
            effect.CardSource = Card;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.UntilTargetLeavesPlay(GetBandmate());

            var coroutine = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}

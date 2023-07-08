using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class ShredZoneCardController : ScreaMachineBandCardController
    {
        public ShredZoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Slice)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(1, 2);
        }

        protected override IEnumerator ActivateBandAbility()
        {
            var coroutine = DealDamageToLowestHP(GetBandmate(), 1, c => IsHero(c), c => 1, DamageType.Sonic, isIrreducible: true,
                                numberOfTargets: 2);
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

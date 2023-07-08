using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class IrresistibleVoiceCardController : ScreaMachineBandCardController
    {
        public IrresistibleVoiceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Valentine)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        protected override IEnumerator ActivateBandAbility()
        {
            var coroutine = DealDamageToHighestHP(null, 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => 2, DamageType.Melee,
                                damageSourceInfo: new TargetInfo(HighestLowestHP.LowestHP, 1, 1, new LinqCardCriteria(c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, "hero target with the lowest")));
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

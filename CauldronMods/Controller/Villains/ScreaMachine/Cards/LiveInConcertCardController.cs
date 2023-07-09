using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class LiveInConcertCardController : ScreaMachineUtilityCardController
    {
        public LiveInConcertCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG };

        public override IEnumerator Play()
        {
            var coroutine = DealDamageToHighestHP(null, 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => H, DamageType.Melee,
                                damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInPlayAndNotUnderCard)));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = ActivateBandAbilities(AbilityIcons);
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

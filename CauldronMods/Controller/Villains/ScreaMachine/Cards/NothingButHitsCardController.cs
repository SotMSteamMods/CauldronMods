using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class NothingButHitsCardController : ScreaMachineUtilityCardController
    {
        public NothingButHitsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG };

        public override IEnumerator Play()
        {
            var coroutine = GameController.SelectCardsAndDoAction(null, new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInPlayAndNotUnderCard, "villain target", false), SelectionType.CardToDealDamage,
                            actionWithCard: source => base.DealDamageToHighestHP(source, 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => 2, DamageType.Projectile),
                            allowAutoDecide: true,
                            cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.ActivateBandAbilities(AbilityIcons);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class ValentineCharacterCardController : ScreaMachineBandCharacterCardController
    {
        public ValentineCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Valentine)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(1, 2).Condition = () => !Card.IsFlipped;
            SpecialStringMaker.ShowHeroTargetWithHighestHP(1, 1).Condition = () => Card.IsFlipped;
        }

        protected override string AbilityDescription => $"{Card.Title} deals the 2 hero targets with the highest HP *{H - 2}* psychic damage.";

        protected override string UltimateFormMessage => "The world's a stage, and I want the brightest spot."; //David Lee Roth

        protected override IEnumerator ActivateBandAbility()
        {
            var coroutine = DealDamageToHighestHP(Card, 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => H - 2, DamageType.Psychic,
                            numberOfTargets: () => 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected override void AddFlippedSideTriggers()
        {
            AddSideTrigger(AddIncreaseDamageTrigger(dda => dda.DamageSource != null && dda.DamageSource.Card != null && IsVillainTarget(dda.DamageSource.Card), 1));
            AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => UltimateEndOfTurn(), TriggerType.DealDamage));
        }

        private IEnumerator UltimateEndOfTurn()
        {
            var damageInfo = new List<DealDamageAction>()
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), null, H - 1, DamageType.Sonic),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), null, H - 2, DamageType.Psychic)
            };

            var coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(damageInfo, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, HighestLowestHP.HighestHP);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class BloodlaceCharacterCardController : ScreaMachineBandCharacterCardController
    {
        public BloodlaceCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Bloodlace)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP().Condition = () => Card.IsFlipped;
            SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => Card.IsFlipped;
        }

        protected override string AbilityDescription => $"Each other villain target regains *{H - 2}* HP.";

        protected override string UltimateFormMessage => "The knock-out punch is always the one you never see coming."; //Aimee Mann

        protected override IEnumerator ActivateBandAbility()
        {
            var coroutine = GameController.GainHP(DecisionMaker, c => c.IsInPlayAndNotUnderCard && IsVillainTarget(c) && c != Card, H - 2, cardSource: GetCardSource());
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
            AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => UltimateEndOfTurn(), new[] { TriggerType.DealDamage, TriggerType.GainHP }));
        }

        private IEnumerator UltimateEndOfTurn()
        {
            List<DealDamageAction> results = new List<DealDamageAction>();
            var coroutine = base.DealDamageToHighestHP(null, 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => H, DamageType.Melee,
                            storedResults: results,
                            numberOfTargets: () => 1,
                            damageSourceInfo: new TargetInfo(HighestLowestHP.LowestHP, 1, 1, new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInPlay)),
                            selectTargetEvenIfCannotDealDamage: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var action = results.FirstOrDefault();
            if (action != null)
            {
                coroutine = GameController.GainHP(action.DamageSource.Card, H, cardSource: GetCardSource());
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
}

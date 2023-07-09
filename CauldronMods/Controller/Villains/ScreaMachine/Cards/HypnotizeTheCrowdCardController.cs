using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class HypnotizeTheCrowdCardController : ScreaMachineBandCardController
    {
        public HypnotizeTheCrowdCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Valentine)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddImmuneToDamageTrigger(dda => dda.DamageSource.IsEnvironmentSource  && dda.Target == GetBandmate());
        }

        protected override IEnumerator ActivateBandAbility()
        {
            List<SelectTargetDecision> result = new List<SelectTargetDecision>();
            var env = GameController.FindCardsWhere(new LinqCardCriteria(c => c.IsEnvironment && c.IsInPlayAndHasGameText, "environment"), visibleToCard: GetCardSource());
            var coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, env, result,
                                allowAutoDecide: true,
                                additionalCriteria: c => c.IsEnvironment,
                                damageAmount: c => 3,
                                damageType: DamageType.Melee,
                                selectionType: SelectionType.CardToDealDamage,
                                cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (result.Any() && result.First() != null && result.First().SelectedCard != null)
            {
                var card = result.First().SelectedCard;

                coroutine = DealDamageToHighestHP(card, 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => 3, DamageType.Melee,
                                numberOfTargets: () => 1);
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

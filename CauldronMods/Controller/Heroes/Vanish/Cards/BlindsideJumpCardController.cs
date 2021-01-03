using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class BlindsideJumpCardController : CardController
    {
        public BlindsideJumpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 1);

            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), damages, DamageType.Energy, targets, false, targets,
                                addStatusEffect: StatusEffectResponse,
                                cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator StatusEffectResponse(DealDamageAction action)
        {
            int increases = GetPowerNumeral(2, 1);

            var effect = new IncreaseDamageStatusEffect(increases);
            effect.TargetCriteria.IsSpecificCard = action.Target;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.UntilCardLeavesPlay(action.Target);
            effect.CardSource = Card;
            effect.Identifier = Card.Title;

            var coroutine = base.AddStatusEffect(effect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
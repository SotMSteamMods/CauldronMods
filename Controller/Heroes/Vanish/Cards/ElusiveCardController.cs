using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class ElusiveCardController : CardController
    {
        public ElusiveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int reduces = GetPowerNumeral(0, 1);
            var effect = new ReduceDamageStatusEffect(reduces);
            effect.CardSource = Card;
            effect.Identifier = Card.Title;
            effect.NumberOfUses = 1;
            effect.TargetCriteria.IsSpecificCard = CharacterCard;

            var coroutine = base.AddStatusEffect(effect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.DrawACardOrPlayACard(DecisionMaker, true);
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
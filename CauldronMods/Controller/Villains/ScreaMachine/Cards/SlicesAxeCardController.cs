using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class SlicesAxeCardController : ScreaMachineBandCardController
    {
        public SlicesAxeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Slice)
        {
            SpecialStringMaker.ShowHeroWithMostCards(true);
        }

        protected override IEnumerator ActivateBandAbility()
        {
            List<TurnTaker> result = new List<TurnTaker>();
            var coroutine = base.FindHeroWithMostCardsInHand(result);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!result.Any() || result.First() is null)
                yield break;

            List<Card> targets = new List<Card>();
            coroutine = base.FindCharacterCardToTakeDamage(result.First(), targets, GetBandmate(), H - 1, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!targets.Any() || targets.First() is null)
                yield break;

            coroutine = DealDamage(GetBandmate(), targets.First(), H - 1, DamageType.Melee, cardSource: GetCardSource());
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

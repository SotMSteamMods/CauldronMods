using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class UpToElevenCardController : ScreaMachineUtilityCardController
    {
        public UpToElevenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.RickyG };

        public override IEnumerator Play()
        {
            List<Card> lowest = new List<Card>();
            var coroutine = GameController.FindTargetWithLowestHitPoints(1, c => IsVillainTarget(c) && c.IsInPlayAndNotUnderCard, lowest, evenIfCannotDealDamage: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (lowest.Any())
            {
                coroutine = GameController.GainHP(lowest.First(), H - 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            coroutine = DealDamage(ValentineCharacter, (Card c) => !IsVillainTarget(c) && c.IsTarget && c.IsInPlayAndHasGameText, 1, DamageType.Sonic);
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

            if (TheSetListCardController is null)
                yield break;

            coroutine = TheSetListCardController.RevealTopCardOfTheSetList();
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

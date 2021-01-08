using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class CantStopTheMusicCardController : ScreaMachineBandCardController
    {
        public CantStopTheMusicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Bloodlace)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
        }

        protected override IEnumerator ActivateBandAbility()
        {
            List<Card> lowest = new List<Card>();
            var coroutine = GameController.FindTargetWithLowestHitPoints(1, c => IsVillainTarget(c) && c.IsInPlayAndNotUnderCard, lowest, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (lowest.Any() && lowest.First() != null)
            {
                coroutine = GameController.GainHP(lowest.First(), 2, cardSource: GetCardSource());
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

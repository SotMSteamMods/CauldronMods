using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class RiftbladeStrikesCardController : OutlanderUtilityCardController
    {
        public RiftbladeStrikesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 2);
        }

        public override IEnumerator Play()
        {
            //{Outlander} deals the non-villain target with the second highest HP 2 fire damage.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 2, (Card c) => !IsVillainTarget(c), (Card c) => 2, DamageType.Fire);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //{Outlander} deals the non-villain target with the highest HP 4 melee damage.
            coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => !IsVillainTarget(c), (Card c) => 4, DamageType.Melee);
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

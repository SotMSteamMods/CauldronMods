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

        }

        public override IEnumerator Play()
        {
            //{Outlander} deals the non-villain target with the second highest HP 2 fire damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 2, (Card c) => !base.IsVillain(c) && c.IsTarget, (Card c) => 2, DamageType.Fire);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //{Outlander} deals the non-villain target with the highest HP 4 melee damage.
            coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => !base.IsVillain(c) && c.IsTarget, (Card c) => 4, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

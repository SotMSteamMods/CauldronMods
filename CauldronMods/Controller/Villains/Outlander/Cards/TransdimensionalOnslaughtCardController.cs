using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class TransdimensionalOnslaughtCardController : OutlanderUtilityCardController
    {
        public TransdimensionalOnslaughtCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Outlander} deals each non-villain target X irreducible psychic damage, where X is the number of Trace cards in play.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => !base.IsVillain(c) && c.IsTarget, (Card c) => base.FindCardsWhere(new LinqCardCriteria((Card card) => base.IsTrace(card) && c.IsInPlayAndHasGameText)).Count(), DamageType.Psychic);
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

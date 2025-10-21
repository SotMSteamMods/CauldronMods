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
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsTrace(c), "trace"));
        }

        public override IEnumerator Play()
        {
            //{Outlander} deals each non-villain target X irreducible psychic damage, where X is the number of Trace cards in play.
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsVillainTarget(c), (Card c) => FindCardsWhere((Card card) => IsTrace(card) && card.IsInPlayAndNotUnderCard).Count(), DamageType.Psychic, isIrreducible: true);
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

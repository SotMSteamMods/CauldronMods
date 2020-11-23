using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class NaniteCorruptionCardController : CardController
    {
        public NaniteCorruptionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until {H - 1} targets are revealed. Put those cards into play and discard the rest.
            IEnumerator coroutine = base.RevealCards_PutSomeIntoPlay_DiscardRemaining(base.TurnTakerController, base.TurnTaker.Deck, Game.H - 1, new LinqCardCriteria((Card c) => c.IsTarget), revealUntilNumberOfMatchingCards: Game.H - 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //{SwarmEater} deals the {H} targets other than itself with the lowest HP 2 projectile damage each.
            coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c != base.CharacterCard, (Card c) => 2, DamageType.Melee, numberOfTargets: Game.H);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
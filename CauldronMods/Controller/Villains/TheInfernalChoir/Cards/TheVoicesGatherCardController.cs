using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class TheVoicesGatherCardController : TheInfernalChoirUtilityCardController
    {
        public TheVoicesGatherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => c.IsTarget, "", useCardsSuffix: false, singular: "target", plural: "targets"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(VagrantHeartSoulRevealedIdentifier);
        }
        /*
         * "Reveal cards from the top of the villain deck until a target is revealed, put it into play, and discard the other revealed cards.",
		 * "That target deals the hero target with the highest HP {H - 1} psychic damage. If Vagrant Heart: Soul Revealed is in play, each player draws a card."
         */
        public override IEnumerator Play()
        {
            List<Card> playedCards = new List<Card>();
            var coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, null, new LinqCardCriteria(c => c.IsTarget, "target"), playedCards: playedCards, revealUntilNumberOfMatchingCards: 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card target = null;
            if(playedCards != null && playedCards.Any())
            {
                target = playedCards.First();
            }

            if (target != null)
            {
                coroutine = base.DealDamageToHighestHP(target, 1, c => IsHeroTarget(c), c => H - 1, DamageType.Psychic, numberOfTargets: () => 1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            var h1 = TurnTaker.FindCard(VagrantHeartHiddenHeartIdentifier, false);
            var h2 = TurnTaker.FindCard(VagrantHeartSoulRevealedIdentifier, false);

            if (IsVagrantHeartSoulRevealedInPlay())
            {
                coroutine = EachPlayerDrawsACard();
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
}

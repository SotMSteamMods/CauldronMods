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
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => c.IsTarget, "", false, false, "target", "targets"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(VagrantHeartSoulRevealedIdentifier);
        }
        /*
         * "Reveal cards from the top of the villain deck until a target is revealed, put it into play, and discard the other revealed cards.",
		 * "That target deals the hero target with the highest HP {H - 1} psychic damage. If Vagrant Heart: Soul Revealed is in play, each player draws a card."
         */
        public override IEnumerator Play()
        {
            List<RevealCardsAction> results = new List<RevealCardsAction>();
            var coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, c => c.IsTarget, 1, results, RevealedCardDisplay.ShowMatchingCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card target = null;
            var result = results.FirstOrDefault();
            if (result != null && result.FoundMatchingCards)
            {
                List<PlayCardAction> playResult = new List<PlayCardAction>();
                coroutine = GameController.PlayCard(TurnTakerController, result.MatchingCards.First(), true, storedResults: playResult, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayCards(playResult, 1))
                {
                    target = playResult.First().CardToPlay;
                }
            }

            coroutine = GameController.MoveCards(TurnTakerController, result.NonMatchingCards, TurnTaker.Trash, isDiscard: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (target != null)
            {
                coroutine = base.DealDamageToHighestHP(target, 1, c => c.IsHero, c => H - 1, DamageType.Psychic, numberOfTargets: () => 1);
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

            //var a = IsVagrantHeartSoulRevealedInPlay();
            //var b = IsVagrantHeartHiddenHeartInPlay();
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

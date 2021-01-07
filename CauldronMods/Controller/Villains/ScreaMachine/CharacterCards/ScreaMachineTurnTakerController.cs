using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.ScreaMachine
{
    public class ScreaMachineTurnTakerController : TurnTakerController
    {
        public ScreaMachineTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            var setList = GameController.FindCardsWhere(c => c.Identifier == "TheSetList", false).First();
            var setCC = FindCardController(setList);
            var bandCards = GameController.FindCardsWhere(new LinqCardCriteria(c => IsBandCard(c))).ToList().Shuffle(base.GameController.Game.RNG);


            IEnumerator coroutine = GameController.BulkMoveCards(this, bandCards, setList.UnderLocation, performBeforeDestroyActions: false, cardSource: setCC.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.FlipCards(bandCards.Select(c => FindCardController(c)), setCC.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (GameController.Game.IsAdvanced)
            {
                /*
                 * At the start of the game, reveal cards from beneath this one until {H - 2} cards
                 * with different keywords are revealed...
                 */
                List<Card> played = new List<Card>();
                HashSet<string> foundKeywords = new HashSet<string>();
                foreach (var card in setList.UnderLocation.Cards)
                {
                    var keywords = GameController.GetAllKeywords(card, true, true);
                    if (!keywords.Any(k => foundKeywords.Contains(k)))
                    {
                        keywords.ForEach(k => foundKeywords.Add(k));
                        played.Add(card);
                    }

                    if (played.Count >= (H - 2))
                        break;
                }

                // ...Play those cards. 
                foreach (var card in played)
                {
                    coroutine = GameController.FlipCard(FindCardController(card), cardSource: setCC.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.PlayCard(this, card, evenIfAlreadyInPlay: true, reassignPlayIndex: true, cardSource: setCC.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                // ...Shuffle the rest and replace them face down beneath this card.
                coroutine = GameController.ShuffleLocation(setList.UnderLocation, cardSource: setCC.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            coroutine = GameController.ShuffleLocation(TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

        }

        protected bool IsBandCard(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.GetAllKeywords(c, evenIfUnderCard, evenIfFacedown).Any(str => ScreaMachineBandmate.Keywords.Contains(str));
        }
    }
}
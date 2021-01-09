using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;

namespace Cauldron.TheMistressOfFate
{
    public class TheMistressOfFateTurnTakerController : TurnTakerController
    {
        private Location _dayDeck;
        private Location dayDeck
        {
            get
            {
                if(_dayDeck == null)
                {
                    _dayDeck = TurnTaker.FindSubDeck("DayDeck");
                }
                return _dayDeck;
            }
        }
        public TheMistressOfFateTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //"At the start of the game, put {TheMistressOfFate}'s villain character cards into play, “Endless Cycle“ side up.",
            //"Put the Timeline card in the center of the play area. 
            IEnumerator coroutine;
            coroutine = GameController.MoveCard(this, TurnTaker.GetCardByIdentifier("TheTimeline"), TurnTaker.PlayArea);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            
            //Shuffle the day cards face down, and place them in a row to the right of the Timeline."
            var days = TurnTaker.GetCardsAtLocation(dayDeck).Where((Card c) => c.IsRealCard).ToList();
            foreach (Card day in days)
            {
                coroutine = GameController.FlipCard(FindCardController(day));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.MoveCard(this, day, TurnTaker.PlayArea, playCardIfMovingToPlayArea: false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //set up incapacitated hero storage
            foreach(HeroTurnTaker hero in GameController.AllHeroes)
            {
                var hand = TurnTaker.GetCardsAtLocation(dayDeck).Where((Card c) => c.Identifier == "HandStorage").FirstOrDefault();
                var deck = TurnTaker.GetCardsAtLocation(dayDeck).Where((Card c) => c.Identifier == "DeckStorage").FirstOrDefault();
                var trash = TurnTaker.GetCardsAtLocation(dayDeck).Where((Card c) => c.Identifier == "TrashStorage").FirstOrDefault();

                coroutine = GameController.BulkMoveCards(this, new Card[] { hand, deck, trash }, hero.OffToTheSide, responsibleTurnTaker: TurnTaker, cardSource: CharacterCardController.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
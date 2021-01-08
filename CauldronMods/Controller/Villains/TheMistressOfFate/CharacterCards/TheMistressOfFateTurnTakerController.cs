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
            IEnumerator coroutine;
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
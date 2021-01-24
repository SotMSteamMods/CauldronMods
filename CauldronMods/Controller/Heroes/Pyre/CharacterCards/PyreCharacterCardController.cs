using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class PyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public PyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int numPlayers = GetPowerNumeral(0, 1);
            //"1 player draws a card. {PyreIrradiate} that card until it leaves their hand. 
            var storedDraw = new List<DrawCardAction>();
            var selectHero = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria(tt => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && GameController.CanDrawCards(FindHeroTurnTakerController(tt.ToHero()), GetCardSource())), SelectionType.DrawCard, numberOfTurnTakers: numPlayers, numberOfCards: 1, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectHero, DrawAndIrradiateDrawnCard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle a cascade card from your trash into your deck."

            yield break;
        }

        private IEnumerator DrawAndIrradiateDrawnCard(TurnTaker tt)
        {
            var drawStorage = new List<DrawCardAction>();
            IEnumerator coroutine = DrawCard(tt.ToHero(), cardsDrawn: drawStorage);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidDrawCards(drawStorage))
            {
                coroutine = IrradiateCard(drawStorage.FirstOrDefault().DrawnCard);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card to draw a card, play a card, and use a power now.",
                        break;
                    }
                case 1:
                    {
                        //"One hero target deals each target 1 energy damage.",
                        break;
                    }
                case 2:
                    {
                        //"One player may discard 2 cards, then draw 2 cards."
                        break;
                    }
            }
            yield break;
        }
    }
}

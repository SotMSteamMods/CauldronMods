using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class HarbingerQuicksilverCharacterCardController : HeroCharacterCardController
    {
        public HarbingerQuicksilverCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now.",
                        break;
                    }
                case 1:
                    {
                        //"Destroy a target with exactly 3HP.",
                        break;
                    }
                case 2:
                    {
                        //"One player may discard a one-shot. If they do, 2 players may each draw a card now."
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Discard the top 2 cards of your deck, or put 1 card fomr your trash into your hand."
            int numDiscard = GetPowerNumeral(0, 2);
            int numReturn = GetPowerNumeral(0, 1);

            var functions = new List<Function>
            {
                new Function(DecisionMaker, $"Discard the top {numDiscard} cards of your deck", SelectionType.DiscardFromDeck, () => DiscardCardsFromTopOfDeck(DecisionMaker, numDiscard), forcedActionMessage: $"There are no cards in {DecisionMaker.TurnTaker.Trash.GetFriendlyName()} to return."),
                new Function(DecisionMaker, $"Return {numReturn} card from your trash to your hand", SelectionType.MoveCardToHandFromTrash, () => ReturnCardsFromTrash(numReturn), DecisionMaker.TurnTaker.Trash.HasCards)
            };
            var functionDecision = new SelectFunctionDecision(GameController, DecisionMaker, functions, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(functionDecision);
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

        private IEnumerator ReturnCardsFromTrash(int numCards)
        {
            IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, DecisionMaker.TurnTaker.Trash, numCards, numCards, new LinqCardCriteria(),
                                            new MoveCardDestination[] { new MoveCardDestination(DecisionMaker.HeroTurnTaker.Hand) },
                                            selectionType: SelectionType.MoveCardToHandFromTrash,
                                            cardSource: GetCardSource());
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
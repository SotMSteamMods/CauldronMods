using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cricket
{
    public class VantagePointCardController : CardController
    {
        public VantagePointCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Each player may put a card other than Vantage Point from their trash into their hand. Destroy this card.
            IEnumerator coroutine = base.EachPlayerMovesCards(1, 1, SelectionType.MoveCardToHandFromTrash, new LinqCardCriteria((Card c) => c.Identifier != base.Card.Identifier), (HeroTurnTaker htt) => htt.Trash, (HeroTurnTaker htt) => new MoveCardDestination(htt.Deck).ToEnumerable<MoveCardDestination>());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Destroy this card.
            coroutine = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, cardSource: base.GetCardSource());
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
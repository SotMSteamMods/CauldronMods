using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class MakeshiftShelterCardController : NorthsparCardController
    {

        public MakeshiftShelterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} is in {Card.Location.GetFriendlyName()}.").Condition = () => !Card.IsInPlay;
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria(c => c.Identifier == TakAhabIdentifier, "Tak Ahab", useCardsSuffix: false));
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, you may shuffle a card from the environment trash into the deck. if Tak Ahab is in the environment trash, shuffle him into the deck.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.ShuffleCardsResponse, TriggerType.ShuffleCardIntoDeck);
        }

        private IEnumerator ShuffleCardsResponse(PhaseChangeAction pca)
        {
            //you may shuffle a card from the environment trash into the deck.
            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.Location == base.TurnTaker.Trash && GameController.IsLocationVisibleToSource(base.TurnTaker.Trash, GetCardSource()));
            Func<Card, IEnumerator> actionWithCard = (Card c) => base.GameController.ShuffleCardIntoLocation(base.DecisionMaker, c, base.TurnTaker.Deck, false, cardSource: base.GetCardSource());
            IEnumerator coroutine = base.GameController.SelectCardsAndDoAction(base.DecisionMaker, cardCriteria, SelectionType.ShuffleCardFromTrashIntoDeck, actionWithCard, new int?(1), true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //if Tak Ahab is in the environment trash, shuffle him into the deck.
            if (base.TurnTaker.Trash.HasCard(FindCard(TakAhabIdentifier)))
            {
                cardCriteria = new LinqCardCriteria((Card c) => c.Location == base.TurnTaker.Trash && c.Identifier == TakAhabIdentifier);
                actionWithCard = (Card c) => base.GameController.ShuffleCardIntoLocation(base.DecisionMaker, c, base.TurnTaker.Deck, false, cardSource: base.GetCardSource());
                coroutine = base.GameController.SelectCardsAndDoAction(base.DecisionMaker, cardCriteria, SelectionType.ShuffleCardFromTrashIntoDeck, actionWithCard, new int?(1), cardSource: base.GetCardSource());
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

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card and Supply Depot are indestructible.
            return card == base.Card || card.Identifier == "SupplyDepot";
        }
    }
}
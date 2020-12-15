using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Menagerie
{
    public class EnclosureCardController : MenagerieCardController
    {
        public EnclosureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override void AddTriggers()
        {
            //Front: Cards beneath villain cards are not considered in play. When an enclosure leaves play, put it under [Menagerie], discarding all cards beneath it. Put any discarded targets into play.
            //Back: Cards beneath enclosures are not considered in play. When an enclosure leaves play, discard all cards beneath it.
            base.AddBeforeLeavesPlayAction(this.HandleEnclosureCardsResponse, TriggerType.MoveCard);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            if (base.CharacterCard.IsFlipped)
            {
                //Back: When an enclosure enters play, move it next to the active hero with the fewest enclosures in their play area. Heroes with enclosures in their play area may not damage cards in other play areas.
                List<TurnTaker> heroes = new List<TurnTaker>();
                IEnumerator coroutine = base.FindHeroWithFewestCardsInPlay(heroes, cardCriteria: new LinqCardCriteria((Card c) => base.IsEnclosure(c)));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && heroes.Contains(c.Owner)), storedResults, isPutIntoPlay, decisionSources);
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
            base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources);
            yield break;
        }

        private IEnumerator HandleEnclosureCardsResponse(GameAction gameAction)
        {
            if (!base.CharacterCard.IsFlipped)
            {
                //Front: When an enclosure leaves play, put it under [Menagerie]...
                if (gameAction is DestroyCardAction)
                {
                    (gameAction as DestroyCardAction).SetPostDestroyDestination(base.CharacterCard.UnderLocation, cardSource: base.GetCardSource());
                }
                else if (gameAction is MoveCardAction && (gameAction as MoveCardAction).Origin.IsInPlay)
                {
                    (gameAction as MoveCardAction).SetDestination(base.CharacterCard.UnderLocation);
                }
            }

            //...discarding all cards beneath it. 
            while (base.CharacterCard.UnderLocation.HasCards)
            {
                Card topCard = base.CharacterCard.UnderLocation.TopCard;
                Location destination = topCard.Owner.Trash;
                bool isPutIntoPlay = false;
                //Front: Put any discarded targets into play.
                if (topCard.IsTarget && !base.CharacterCard.IsFlipped)
                {
                    destination = topCard.Owner.PlayArea;
                    isPutIntoPlay = true;
                }
                IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, topCard, destination, isPutIntoPlay: isPutIntoPlay, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public IEnumerator EncloseTopCardResponse()
        {
            //When this card enters play, place the top card of the villain deck beneath it face down.
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, base.Card.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource());
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
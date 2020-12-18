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
            }
            if (storedResults != null && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(this.TurnTaker)))
            {
                storedResults.Add(new MoveCardDestination(this.TurnTaker.PlayArea));
            }
            yield break;
        }

        private IEnumerator HandleEnclosureCardsResponse(GameAction gameAction)
        {
            //...discarding all cards beneath it. 
            while (base.Card.UnderLocation.HasCards)
            {
                IEnumerator coroutine;
                Card topCard = base.Card.UnderLocation.TopCard;
                if (topCard.IsFlipped)
                {
                    coroutine = base.GameController.FlipCard(base.FindCardController(topCard), cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                Location destination = topCard.Owner.Trash;
                bool isPutIntoPlay = false;
                //Front: Put any discarded targets into play.
                if (topCard.MaximumHitPoints.HasValue && !base.CharacterCard.IsFlipped)
                {
                    destination = topCard.Owner.PlayArea;
                    isPutIntoPlay = true;
                }
                coroutine = base.GameController.MoveCard(base.TurnTakerController, topCard, destination, isPutIntoPlay: isPutIntoPlay, cardSource: base.GetCardSource());
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
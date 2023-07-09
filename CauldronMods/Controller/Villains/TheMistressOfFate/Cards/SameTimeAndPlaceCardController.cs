using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class SameTimeAndPlaceCardController : TheMistressOfFateUtilityCardController
    {
        public SameTimeAndPlaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheMistressOfFate} deals each hero 10 melee damage.",
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) =>  IsHeroCharacterCard(c), 10, DamageType.Melee);
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

        public override void AddTriggers()
        {
            //"If there is no card beneath this one and an environment card leaves play, put that card beneath this one. 
            AddTrigger((MoveCardAction mc) => mc.CanChangeDestination && mc.Origin.IsInPlay && !mc.Destination.IsInPlay && mc.CardToMove.IsEnvironment && this.Card.UnderLocation.IsEmpty, StoreCardUnderResponse, TriggerType.MoveCard, TriggerTiming.Before);
            //When a Day card flips face down, put any cards beneath this one on top of the environment deck."
            AddTrigger((FlipCardAction fc) => IsDay(fc.CardToFlip.Card) && fc.ToFaceDown, ReturnStoredCardsResponse, TriggerType.MoveCard, TriggerTiming.After);
        }

        private IEnumerator StoreCardUnderResponse(MoveCardAction mc)
        {
            if(mc.CanChangeDestination && this.Card.UnderLocation.IsEmpty)
            {
                mc.SetDestination(this.Card.UnderLocation);
            }
            IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} stores {mc.CardToMove.Title}...", Priority.Medium, GetCardSource());
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

        private IEnumerator ReturnStoredCardsResponse(FlipCardAction fc)
        {
            if(this.Card.UnderLocation.HasCards)
            {
                IEnumerator message = GameController.SendMessageAction($"{Card.Title} puts the cards underneath it on top of the environment deck.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
            }
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(DecisionMaker,
                                                    new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation),
                                                    SelectionType.MoveCardOnDeck,
                                                    (Card c) => GameController.MoveCard(DecisionMaker, c, FindEnvironment().TurnTaker.Deck, cardSource: GetCardSource()),
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

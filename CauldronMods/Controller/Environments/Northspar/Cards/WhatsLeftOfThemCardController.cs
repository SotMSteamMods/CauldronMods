using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class WhatsLeftOfThemCardController : NorthsparCardController
    {

        public WhatsLeftOfThemCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => IsWaypoint(c), "waypoint"));
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsWaypoint(c), "waypoint"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 1 psychic damage and is destroyed.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndDestroyResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction pca)
        {
            //this card deals each hero target 1 psychic damage...
            IEnumerator coroutine = base.DealDamage(base.Card, (Card c) => IsHeroTarget(c), 1, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // ...and is destroyed.
            coroutine = base.DestroyThisCardResponse(pca);
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

        public override IEnumerator Play()
        {
            //When this card enters play, search the environment deck and trash for a First, Second, or Third Waypoint card and put it into play, then shuffle the deck.
            IEnumerator coroutine = SearchForWaypoints();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }


    }
}
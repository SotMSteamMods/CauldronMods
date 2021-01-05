using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class RagingBlizzardCardController : NorthsparCardController
    {

        public RagingBlizzardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => IsFrozen(c), "frozen"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, discard the top 4 cards of the environment deck. This card deals each non-environment target X cold damage, where X = the number of Frozen cards discarded this way.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DiscardAndDealDamageReponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator DiscardAndDealDamageReponse(PhaseChangeAction pca)
        {
            //discard the top 4 cards of the environment deck
            //this should force a reshuffle if less than 4 cards
            List<MoveCardAction> storedResults = new List<MoveCardAction>();
            IEnumerator coroutine = DiscardCardsFromTopOfDeck(FindEnvironment(), 4, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidMoveCard(storedResults))
            {
                int X = storedResults.Count((MoveCardAction mc) => IsFrozen(mc.CardToMove));
                if (X == 0)
                {
                    coroutine = base.GameController.SendMessageAction(base.Card.Title + " did not discard any Frozen cards, so no damage is dealt", Priority.Medium, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    //This card deals each non-environment target X cold damage, where X = the number of Frozen cards discarded this way
                    coroutine = base.DealDamage(base.Card, (Card c) => c.IsNonEnvironmentTarget, X, DamageType.Cold);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    //If 4 Frozen cards were discarded this way, destroy this card.
                    if (X == 4)
                    {
                        coroutine = base.DestroyThisCardResponse(pca);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                }
            }

            yield break;
        }
    }
}
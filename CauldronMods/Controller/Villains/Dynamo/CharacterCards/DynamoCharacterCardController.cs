using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class DynamoCharacterCardController : VillainCharacterCardController
    {
        public DynamoCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddSideTriggers()
        {
            if (!base.CharacterCard.IsFlipped)
            {
                //Front:
                //At the start of the villain turn, discard the top card of the villain deck and {Dynamo} deals the hero target with the highest HP {H} energy damage.
                base.AddSideTrigger(base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.StartOfTurnResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DealDamage }));

                //At the end of the villain turn, if there are at least 6 cards in the villain trash, flip {Dynamo}'s villain character card.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.TurnTaker.Trash.Cards.Count() >= 6, base.FlipThisCharacterCardResponse, TriggerType.FlipCard));

                if (base.Game.IsAdvanced)
                {
                    //Front-Advanced:
                    //Whenever a villain target enters play, discard the top card of the villain deck.
                    base.AddSideTrigger(base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => base.IsVillainTarget(action.CardEnteringPlay), (CardEntersPlayAction action) => base.GameController.DiscardTopCards(base.DecisionMaker, base.TurnTaker.Deck, 1, cardSource: base.GetCardSource()), TriggerType.DiscardCard, TriggerTiming.After));
                }
            }
            else if (base.Game.IsAdvanced)
            {
                //Back-Advanced:
                //Increase damage dealt by villain targets by 1.
                base.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction action) => base.IsVillain(action.DamageSource.Card) && action.DamageSource.Card.IsTarget, 1));
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.CharacterCard.IsFlipped)
            {
                //Back:
                //When Dynamo flips to this side, play the top 2 cards of the villain deck. 
                coroutine = base.GameController.PlayTopCard(base.DecisionMaker, base.TurnTakerController, numberOfCards: 2, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Then, shuffle the villain trash...
                coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Trash, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //...put it on the bottom of the villain deck...
                coroutine = base.GameController.BulkMoveCards(base.TurnTakerController, base.TurnTaker.Trash.Cards, base.TurnTaker.Deck, true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...and flip {Dynamo}'s villain character cards.
                coroutine = base.GameController.FlipCard(this, cardSource: base.GetCardSource());
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

        private IEnumerator StartOfTurnResponse(PhaseChangeAction action)
        {
            //...discard the top card of the villain deck...
            IEnumerator coroutine = base.GameController.DiscardTopCards(base.DecisionMaker, base.TurnTaker.Deck, 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and {Dynamo} deals the hero target with the highest HP {H} energy damage.
            coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => base.Game.H, DamageType.Energy);
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

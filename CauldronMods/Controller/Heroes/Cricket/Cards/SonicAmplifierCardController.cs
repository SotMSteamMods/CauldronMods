using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class SonicAmplifierCardController : CardController
    {
        public SonicAmplifierCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsUnderCard(Card);
        }

        public override void AddTriggers()
        {

            //Cards beneath this are not considered to be in play.
            Card.UnderLocation.OverrideIsInPlay = false;

            //Whenever {Cricket} deals sonic damage to a target, you may put the top card of your deck beneath this one. Cards beneath this one are not considered to be in play.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DidDealDamage && action.DamageType == DamageType.Sonic && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard, this.MoveCardResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After);
        }

        private IEnumerator MoveCardResponse(DealDamageAction action)
        {
            //...you may put the top card of your deck beneath this one.
            List<YesNoDecision> storedResults = new List<YesNoDecision>();
            var yesNo = new YesNoDecision(GameController, DecisionMaker, SelectionType.Custom,
                            cardSource: GetCardSource());
            IEnumerator coroutine = GameController.MakeDecisionAction(yesNo);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidPlayerAnswerYes(yesNo))
            {
                coroutine = GameController.MoveCard(DecisionMaker, TurnTaker.Deck.TopCard, Card.UnderLocation,
                            isPutIntoPlay: false,
                            playCardIfMovingToPlayArea: false,
                            doesNotEnterPlay: true,
                            cardSource: GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard all cards beneath this one. {Cricket} deals 1 target X sonic damage, where X is the number of cards discarded this way.
            int numberOfTargets = GetPowerNumeral(0, 1);
            var movedCards = new List<MoveCardAction>();
            Func<Card, IEnumerator> discardAction = (Card c) => GameController.MoveCard(DecisionMaker, c, FindCardController(c).GetTrashDestination().Location, isDiscard: true, responsibleTurnTaker: TurnTaker, storedResults: movedCards, cardSource: GetCardSource());

            IEnumerator coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation), SelectionType.DiscardCard, discardAction, allowAutoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int numDiscarded = movedCards.Count((MoveCardAction mca) => mca.WasCardMoved);
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), numDiscarded, DamageType.Sonic, numberOfTargets, false, numberOfTargets, cardSource: GetCardSource());
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText($"Do you want to put the top card of your deck beneath {Card.Title}?", $"Should they put the top card of their deck beneath {Card.Title}?", $"Vote for if they should put the top card of their deck beneath {Card.Title}?", $"put the top card of the deck beneath {Card.Title}");

        }
    }
}
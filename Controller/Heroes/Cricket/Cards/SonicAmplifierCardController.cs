using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

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
            //Whenever {Cricket} deals sonic damage to a target, you may put the top card of your deck beneath this one. Cards beneath this one are not considered to be in play.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DidDealDamage && action.DamageType == DamageType.Sonic && action.DamageSource != null && action.DamageSource.Card == base.CharacterCard, this.MoveCardResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After);
        }

        private IEnumerator MoveCardResponse(DealDamageAction action)
        {
            //...you may put the top card of your deck beneath this one.
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.MoveCardToUnderCard, TurnTaker.Deck.TopCard, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidPlayerAnswerYes(storedResults))
            {
                coroutine = GameController.MoveCard(DecisionMaker, TurnTaker.Deck.TopCard, Card.UnderLocation, cardSource: GetCardSource());
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
            IEnumerator coroutine = DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(DecisionMaker, new LinqCardCriteria((Card c) => c.Location == Card.UnderLocation, "cards below " + base.Card.Title), (int X) => this.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), X, DamageType.Sonic, numberOfTargets, false, numberOfTargets, cardSource: GetCardSource()));
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
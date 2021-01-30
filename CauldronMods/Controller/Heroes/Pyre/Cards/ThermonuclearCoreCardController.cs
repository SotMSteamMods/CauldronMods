using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ThermonuclearCoreCardController : PyreUtilityCardController
    {
        public ThermonuclearCoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
            AddInhibitorException((GameAction ga) => (ga is MakeDecisionAction || ga is MoveCardAction || ga is AddStatusEffectAction) && Card.Location.IsHand);
            ShowIrradiatedCount();
        }
        public override void AddStartOfGameTriggers()
        {
            AddTrigger((DrawCardAction d) => d.DrawnCard == Card, (DrawCardAction d) => EntersHandResponse(), new TriggerType[]
            {
                TriggerType.CreateStatusEffect,
                TriggerType.Hidden
            }, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
            AddTrigger((MoveCardAction m) => m.Destination == HeroTurnTaker.Hand && m.CardToMove == Card, (MoveCardAction m) => EntersHandResponse(), new TriggerType[]
            {
                TriggerType.CreateStatusEffect,
                TriggerType.Hidden
            }, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
        }
        public override IEnumerator PerformEnteringGameResponse()
        {
            IEnumerator coroutine = ((!Card.IsInHand) ? base.PerformEnteringGameResponse() : EntersHandResponse());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
        private IEnumerator EntersHandResponse()
        {
            //"When this card enters your hand, select 1 non-{PyreIrradiate} card in your hand. {PyreIrradiate} that card until it leaves your hand.",
            return SelectAndIrradiateCardsInHand(DecisionMaker, TurnTaker, 1, 1);
        }
        public override void AddTriggers()
        {
            //"At the end of your turn, 1 player with no {PyreIrradiate} cards in their hand draws a card. {PyreIrradiate} that card until it leaves their hand."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, SelectHeroToDrawIrradiatedCard, TriggerType.DrawCard);
        }
        private IEnumerator SelectHeroToDrawIrradiatedCard(PhaseChangeAction pc)
        {
            var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, GameController.AllHeroes.Where(htt => !htt.Hand.Cards.Any((Card c) => IsIrradiated(c))).Select(htt => htt as TurnTaker), SelectionType.DrawCard, cardSource: GetCardSource());
            return GameController.SelectTurnTakerAndDoAction(selectHero, DrawAndIrradiateDrawnCard);
        }
        private IEnumerator DrawAndIrradiateDrawnCard(TurnTaker tt)
        {
            var drawStorage = new List<DrawCardAction>();
            IEnumerator coroutine = DrawCard(tt.ToHero(), cardsDrawn: drawStorage);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDrawCards(drawStorage))
            {
                coroutine = IrradiateCard(drawStorage.FirstOrDefault().DrawnCard);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}

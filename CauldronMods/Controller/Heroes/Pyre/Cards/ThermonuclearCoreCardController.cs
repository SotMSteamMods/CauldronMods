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
            this.ShowIrradiatedCount(SpecialStringMaker);
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
            IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} {PyreExtensionMethods.Irradiated} a card in {Card.Location.GetFriendlyName()}", Priority.Medium, GetCardSource(), showCardSource: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = SelectAndIrradiateCardsInHand(DecisionMaker, TurnTaker, 1, 1);
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
        public override void AddTriggers()
        {
            //"At the end of your turn, 1 player with no {PyreIrradiate} cards in their hand draws a card. {PyreIrradiate} that card until it leaves their hand."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, SelectHeroToDrawIrradiatedCard, TriggerType.DrawCard);
        }
        private IEnumerator SelectHeroToDrawIrradiatedCard(PhaseChangeAction pc)
        {
            var validHeroes = GameController.AllHeroControllers.Where(httc => GameController.IsTurnTakerVisibleToCardSource(httc.HeroTurnTaker, GetCardSource()) && !httc.HeroTurnTaker.Hand.Cards.Any((Card c) => c.IsIrradiated()) && GameController.CanDrawCards(httc, GetCardSource())).Select(httc => httc.TurnTaker).ToList();
            IEnumerator coroutine;
            if (!validHeroes.Any())
            {
                coroutine = GameController.SendMessageAction($"There are no heroes that can draw for {Card.Title}", Priority.Medium, GetCardSource());
            }
            else if (validHeroes.Count() == 1)
            {
                var hero = validHeroes.FirstOrDefault();
                var isAre = hero.DeckDefinition.IsPlural ? "are" : "is";
                coroutine = GameController.SendMessageAction($"{hero.Name} {isAre} the only hero that can draw for {Card.Title}", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DrawAndIrradiateDrawnCard(hero);
            }
            else
            {
                var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, validHeroes, SelectionType.DrawCard, cardSource: GetCardSource());
                coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DrawAndIrradiateDrawnCard);
            }

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
                coroutine = this.IrradiateCard(drawStorage.FirstOrDefault().DrawnCard);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Pyre
{
    public class RogueFissionCascadeCardController : PyreUtilityCardController
    {
        public RogueFissionCascadeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
            AddInhibitorException((GameAction ga) => ga is PlayCardAction && Card.Location.IsHand);
            ShowIrradiatedCount();
        }
        private const string LocationKnown = "CascadeLocationKnownKey";
        public override void AddStartOfGameTriggers()
        {
            //"When this card enters your hand, put it into play.",
            AddTrigger((DrawCardAction d) => d.DrawnCard == Card, (DrawCardAction d) => PlayFromHandResponse(), new TriggerType[2]
            {
                TriggerType.PutIntoPlay,
                TriggerType.Hidden
            }, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
            AddTrigger((MoveCardAction m) => m.Destination == HeroTurnTaker.Hand && m.CardToMove == Card, (MoveCardAction m) => PlayFromHandResponse(), new TriggerType[2]
            {
                TriggerType.PutIntoPlay,
                TriggerType.Hidden
            }, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
            AddTrigger((GameAction ga) => IsLocationRevealer(ga), MarkLocationKnown, TriggerType.Hidden, TriggerTiming.After, priority: TriggerPriority.High, outOfPlayTrigger: true);
            AddTrigger((ShuffleCardsAction sc) => sc.Location.IsDeck && sc.Location == this.Card.Location, MarkLocationUnknown, TriggerType.Hidden, TriggerTiming.After, outOfPlayTrigger: true);

            //allows player to opt out of drawing a card if they know a Cascade is on top of their deck
            AddTrigger((DrawCardAction dc) => ShouldWarnForDraw(dc), dc => DoNothing(), TriggerType.DestroySelf, TriggerTiming.Before, outOfPlayTrigger: true);
        }
        private bool ShouldWarnForDraw(DrawCardAction dc)
        {
            if (dc.DrawnCard == Card)
            {
                //Log.Debug("Checking whether to warn about cascade draw...");
                var result = GetCardPropertyJournalEntryBoolean(LocationKnown);
                if (result == true)
                {
                    //Log.Debug("Warning should be generated.");
                    return true;
                }
                else
                {
                    //Log.Debug($"Result was {(result.HasValue ? "false" : "null")}");
                }
            }
            return false;
        }
        private bool IsLocationRevealer(GameAction ga)
        {
            if (ga is RevealCardsAction rc)
            {
                return rc.RevealedCards.Contains(this.Card);
            }
            if (ga is MoveCardAction mc)
            {
                return mc.CardToMove == this.Card && !(mc.Destination.IsDeck || this.Card.IsFlipped);
            }
            if (ga is PlayCardAction pc)
            {
                return pc.CardToPlay == this.Card;
            }
            return false;
        }
        private IEnumerator MarkLocationKnown(GameAction ga)
        {
            SetCardPropertyToTrueIfRealAction(LocationKnown);
            //Log.Debug("Marking cascade location as revealed.");
            return DoNothing();
        }
        private IEnumerator MarkLocationUnknown(GameAction ga)
        {
            if(IsRealAction(ga))
            {
                SetCardProperty(LocationKnown, false);
                //Log.Debug("Marking cascade location as not revealed");
            }
            return DoNothing();
        }
        private IEnumerator PlayFromHandResponse()
        {
            IEnumerator coroutine = GameController.SendMessageAction( Card.Title + " puts itself into play.", Priority.High, GetCardSource(), showCardSource: true);
            if ( UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.PlayCard(TurnTakerController, Card, isPutIntoPlay: true, cardSource: GetCardSource());
            if ( UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
        public override IEnumerator PerformEnteringGameResponse()
        {
            IEnumerator coroutine = ((!Card.IsInHand) ? base.PerformEnteringGameResponse() : PlayFromHandResponse());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator Play()
        {
            //"{Pyre} deals each hero with {PyreIrradiate} cards in their hand X energy damage, where X is the number of {PyreIrradiate} cards in all hands.",
            Func<int> NumIrradiatedCardsInHand = () => GameController.GetAllCards().Where((Card c) => IsIrradiated(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Count();
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) =>  IsHeroCharacterCard(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && c.Owner.ToHero().Hand.Cards.Any((Card inHand) => IsIrradiated(inHand)), c => NumIrradiatedCardsInHand(), DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"Reveal the top card of your deck and draw or discard it."
            var storedReveal = new List<Card>();
            coroutine = GameController.RevealCards(DecisionMaker, TurnTaker.Deck, 1, storedReveal, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(storedReveal.FirstOrDefault() != null)
            {
                var topCard = storedReveal.FirstOrDefault();
                var functions = new List<Function>
                {
                    new Function(DecisionMaker, $"Draw {topCard.Title}", SelectionType.DrawCard, () => ReturnCardAndDoAction(topCard, "draw"), onlyDisplayIfTrue: GameController.CanDrawCards(HeroTurnTakerController, GetCardSource())),
                    new Function(DecisionMaker, $"Discard {topCard.Title}", SelectionType.DiscardCard, () => ReturnCardAndDoAction(topCard, "discard"), forcedActionMessage: $"{TurnTaker.Name} cannot draw cards, so {topCard.Title} is discarded.")
                };
                var functionDecision = new SelectFunctionDecision(GameController, DecisionMaker, functions, false, associatedCards: new Card[] { topCard }, cardSource: GetCardSource());
                coroutine = GameController.SelectAndPerformFunction(functionDecision);
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

        private IEnumerator ReturnCardAndDoAction(Card cardToReturn, string action)
        {
            IEnumerator coroutine = GameController.MoveCard(DecisionMaker, cardToReturn, TurnTaker.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(action == "draw")
            {
                coroutine = DrawCard();
            }
            else
            {
                coroutine = GameController.MoveCard(DecisionMaker, cardToReturn, TurnTaker.Trash, isDiscard: true, cardSource: GetCardSource());
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
    }
}

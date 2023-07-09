using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class SwarmingProtocolCypherCharacterCardController : CypherBaseCharacterCardController
    {
        private const int PowerCardsToDraw = 1;
        private const int Incapacitate2CardsToDiscard = 3;

        public SwarmingProtocolCypherCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Play a card face down next to a hero. Until leaving play it gains keyword 'Augment' and text 'This hero is augmented', 
            var destinations = FindCardsWhere(c =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && c.IsRealCard)
                                    .Select(c => new MoveCardDestination(c.NextToLocation))
                                    .ToArray();

            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
            var coroutine = base.GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, this.HeroTurnTaker.Hand, new LinqCardCriteria(card => true), destinations,
                                    isPutIntoPlay: false,
                                    playIfMovingToPlayArea: false,
                                    flipFaceDown: true,
                                    storedResults: selectCardDecisions,
                                    cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCard(selectCardDecisions))
            {
                var card = GetSelectedCard(selectCardDecisions);
                if (IsRealAction())
                {
                    Journal.RecordCardProperties(card, CypherBaseCardController.NanocloudKey, true);
                }
            }

            // ... draw a card.
            coroutine = base.GameController.DrawCards(this.HeroTurnTakerController, PowerCardsToDraw, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddTrigger<MoveCardAction>(mca => mca.Origin.IsInPlay && !mca.Destination.IsInPlay && mca.CardToMove.Owner == TurnTaker && IsNanocloud(mca.CardToMove), mca => ResetNanocloudFlag(mca.CardToMove), TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger<BulkMoveCardsAction>(bma => bma.Origins.Any(kvp => kvp.Value.IsInPlay) && !bma.Destination.IsInPlay && bma.CardsToMove.Any(c => c.Owner == TurnTaker), bma => BulkResetNanocloudFlags(bma.CardsToMove), TriggerType.HiddenLast, TriggerTiming.After);
        }

        private IEnumerator BulkResetNanocloudFlags(IEnumerable<Card> cards)
        {
            if (IsRealAction())
            {
                foreach (var card in cards)
                {
                    if (card.Owner == TurnTaker && IsNanocloud(card))
                    {
                        Journal.RecordCardProperties(card, CypherBaseCardController.NanocloudKey, (bool?)null);
                    }
                }
            }
            return DoNothing();
        }

        private IEnumerator ResetNanocloudFlag(Card card)
        {
            if (IsRealAction())
            {
                Journal.RecordCardProperties(card, CypherBaseCardController.NanocloudKey, (bool?)null);
            }
            return DoNothing();
        }


        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // One player may play a card now.
                        var coroutine = base.GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    break;
                case 1:
                    {
                        // Discard the top 3 cards of 1 deck.
                        var storedResults = new List<SelectLocationDecision>();
                        var coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.DiscardFromDeck, l => true, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectLocation(storedResults))
                        {
                            var loc = GetSelectedLocation(storedResults);
                            coroutine = GameController.DiscardTopCards(DecisionMaker, loc, Incapacitate2CardsToDiscard, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        // Destroy an environment card.
                        LinqCardCriteria criteria = new LinqCardCriteria(c => c.IsEnvironment && c.IsInPlay && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment");
                        var coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, criteria, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    break;
            }
        }
    }
}
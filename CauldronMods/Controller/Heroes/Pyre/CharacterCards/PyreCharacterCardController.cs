using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class PyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public PyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int numPlayers = GetPowerNumeral(0, 1);
            //"1 player draws a card. {PyreIrradiate} that card until it leaves their hand. 
            var storedDraw = new List<DrawCardAction>();
            var selectHero = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.CanDrawCards(FindHeroTurnTakerController(tt.ToHero()), GetCardSource())), SelectionType.DrawCard, numberOfTurnTakers: numPlayers, numberOfCards: 1, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectHero, DrawAndIrradiateDrawnCard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle a cascade card from your trash into your deck."
            if(TurnTaker.Trash.Cards.Any((Card c) => IsCascade(c)))
            {
                var cardToMove = TurnTaker.Trash.Cards.Where((Card c) => IsCascade(c)).FirstOrDefault();
                coroutine = GameController.SendMessageAction($"{Card.Title} shuffles {cardToMove.Title} into {TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource(), new Card[] { cardToMove });
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.MoveCard(DecisionMaker, cardToMove, TurnTaker.Deck, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
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

            if(DidDrawCards(drawStorage))
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card to draw a card, play a card, and use a power now.",
                        var heroesWithIrradiated = GameController.AllHeroes.Where(htt => htt.Hand.Cards.Any(card => IsIrradiated(card)) && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
                        var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesWithIrradiated, SelectionType.DiscardCard, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DiscardForDrawPlayPower);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"One hero target deals each target 1 energy damage.",
                        var heroTargets = GameController.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && IsHero(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
                        var storedTarget = new List<SelectTargetDecision>();
                        coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, heroTargets, storedTarget, damageAmount: c => 1, damageType: DamageType.Energy, selectionType: SelectionType.CardToDealDamage, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }

                        if(storedTarget.FirstOrDefault() != null && storedTarget.FirstOrDefault().SelectedCard != null)
                        {
                            coroutine = DealDamage(storedTarget.FirstOrDefault().SelectedCard, c => c.IsTarget, 1, DamageType.Energy);
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //"One player may discard 2 cards, then draw 2 cards."
                        var heroesActive = GameController.AllHeroes.Where(htt => !htt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
                        var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesActive, SelectionType.DiscardAndDrawCard, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DiscardTwoDrawTwo);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }

        private IEnumerator DiscardForDrawPlayPower(TurnTaker tt)
        {
            //One player may discard a {PyreIrradiate} card...
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var storedDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCard(heroTTC, true, card => IsIrradiated(card), storedDiscard, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidDiscardCards(storedDiscard))
            {
                //to draw a card...
                coroutine = DrawCard(heroTTC.HeroTurnTaker);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //play a card...
                coroutine = SelectAndPlayCardFromHand(heroTTC, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //and use a power now."
                coroutine = GameController.SelectAndUsePower(heroTTC, false, cardSource: GetCardSource());
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
        private IEnumerator DiscardTwoDrawTwo(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var decision = new YesNoAmountDecision(GameController, heroTTC, SelectionType.DiscardAndDrawCard, 2, false, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.MakeDecisionAction(decision);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if(decision.Answer == true)
            {
                coroutine = GameController.SelectAndDiscardCards(heroTTC, 2, false, 2, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = DrawCards(heroTTC, 2);
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
}

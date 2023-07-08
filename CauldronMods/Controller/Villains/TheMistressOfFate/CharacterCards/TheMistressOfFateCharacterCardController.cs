using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class TheMistressOfFateCharacterCardController : VillainCharacterCardController
    {
        private TurnTaker HeroBeingRevived = null;
        private Location _dayDeck;

        public static readonly string IncapLocation = "IncapLocation";
        public static readonly string LocationIndex = "LocationIndex";
        private Location dayDeck
        {
            get
            {
                if (_dayDeck == null)
                {
                    _dayDeck = TurnTaker.FindSubDeck("DayDeck");
                }
                return _dayDeck;
            }
        }
        public TheMistressOfFateCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //might be needed to keep people from looking through the deck for process-of-elimination
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            SpecialStringMaker.ShowSpecialString(BuildRemovedCardsSpecialString);
            if(Game.IsAdvanced)
            {
                SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
            }
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if(cardSource?.Card != null && !cardSource.Card.IsVillain && card.IsVillain && card.Location == TurnTaker.OutOfGame && card.IsFaceDownNonCharacter)
            {
                return false;
            }
            return true;
        }
        

        public override void AddSideTriggers()
        {
            //"Continue playing if all heroes are incapacitated. 
            AddSideTrigger(AddTrigger((GameOverAction go) => go.EndingResult == EndingResult.HeroesDestroyedDefeat, ContinueGameWithMessage, TriggerType.CancelAction, TriggerTiming.Before, priority: TriggerPriority.High));

            //"Incapacitated heroes keep the cards from their hand, deck, and trash separarte from each other when removing them from the game. Cards that were in play go to their trash."
            AddSideTrigger(AddTrigger<BulkMoveCardsAction>(IsCleaningUpIncappedHeroCards, PreserveHero, TriggerType.Hidden, TriggerTiming.Before));

            AddSideTrigger(AddTrigger((UnincapacitateHeroAction uha) => true, uha => SetRevivingHero(uha), TriggerType.Hidden, TriggerTiming.Before));
            AddSideTrigger(AddTrigger((ShuffleCardsAction sc) => sc.Location.OwnerTurnTaker == HeroBeingRevived, sc => CancelAction(sc), TriggerType.CancelAction, TriggerTiming.Before));
            AddSideTrigger(AddTrigger<BulkMoveCardsAction>(IsDefaultCardRestoration, RestoreHeroCards, TriggerType.Hidden, TriggerTiming.Before));
            AddSideTrigger(AddTrigger((UnincapacitateHeroAction uha) => true, uha => SetRevivingHero(uha, clear: true), TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false));

            if (!Card.IsFlipped)
            {

                //"At the start of the villain turn, flip the left-most face down day card.",
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, FlipDayCardResponse, TriggerType.FlipCard));

                //"{TheMistressOfFate} is immune to villain damage. 
                AddSideTrigger(AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource.IsVillain));

                //If there are no cards in the villain deck, the heroes lose.",
                AddTrigger((GameAction ga) => IsDeckEmptier(ga) && TurnTaker.Deck.IsEmpty,
                                SpecialDefeatResponse,
                                TriggerType.GameOver,
                                TriggerTiming.After);

                if(Game.IsAdvanced)
                {
                    //"advanced": "At the end of the villain turn, {TheMistressOfFate} deals the hero with the lowest HP {H} psychic damage.",
                    AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) =>  IsHeroCharacterCard(c), TargetType.LowestHP, H, DamageType.Psychic));
                }
            }
            //flip side is all handled in AfterFlipCardImmediateResponse
            AddDefeatedIfDestroyedTriggers();
        }

        private bool IsDeckEmptier(GameAction ga)
        {
            if(ga is MoveCardAction mc)
            {
                return mc.CardToMove.IsVillain && mc.Destination != TurnTaker.Revealed && TurnTaker.Revealed.IsEmpty;
            }
            if(ga is PlayCardAction pc)
            {
                return pc.CardToPlay.IsVillain;
            }
            if(ga is DiscardCardAction dc)
            {
                return dc.Origin == TurnTaker.Deck;
            }
            return false;
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine;

            coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var faceUpDayCards = GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && GameController.DoesCardContainKeyword(c, "day")).ToList();
            foreach(Card day in faceUpDayCards)
            {
                var dayCC = FindCardController(day) as DayCardController;
                coroutine = dayCC.ReclaimStoredCard(FakeAction);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (this.Card.IsFlipped)
            {
                //"When {TheMistressOfFate} flips to this side:",
                if(Game.IsAdvanced)
                {
                    //"flippedAdvanced": "When {TheMistressOfFate} flips to this side, she regains 10 HP.",
                    coroutine = GameController.GainHP(this.Card, 10, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                if (IsGameChallenge)
                {
                    //Challenge: immediately discard the top card of the villain deck for each face-down Day card.
                    int faceDownDayCardsCount = GameController.FindCardsWhere((Card c) => c.IsFaceDownNonCharacter && c.Location == TurnTaker.PlayArea && GameController.DoesCardContainKeyword(c, "day", evenIfFaceDown: true)).Count();
                    coroutine = GameController.DiscardTopCards(DecisionMaker, TurnTaker.Deck, faceDownDayCardsCount, showCards: c => true, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //"{Bulletpoint} Restore all other targets to their maximum HP.",
                coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c != Card && c.HitPoints < c.MaximumHitPoints), SelectionType.SetHP,
                                (Card c) => GameController.SetHP(c, c.MaximumHitPoints ?? 0, GetCardSource()), allowAutoDecide: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //"{Bulletpoint} Destroy all environment cards.",
                coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), null, false, null, allowAutoDecide: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //"{Bulletpoint} Flip all day cards face down.",
                coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && GameController.DoesCardContainKeyword(c, "day"), "day"), 
                                                            SelectionType.FlipCardFaceDown,
                                                            (Card c) => GameController.FlipCard(FindCardController(c), cardSource: GetCardSource(), allowBackToFront: false), 
                                                            allowAutoDecide: true,
                                                            cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //"{Bulletpoint} Remove all cards in the villain trash [...from the game],
                coroutine = GameController.MoveCards(DecisionMaker, TurnTaker.Trash, TurnTaker.OutOfGame, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //[...remove] the top {H - 1} cards of the villain deck from the game without looking at them."
                for (int i = 0; i < H - 1; i++)
                {
                    if(TurnTaker.Deck.IsEmpty)
                    {
                        break;
                    }
                    var topCard = TurnTaker.Deck.TopCard;
                    coroutine = GameController.FlipCard(FindCardController(topCard), cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.MoveCard(DecisionMaker, topCard, TurnTaker.OutOfGame, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //"{Bulletpoint} Flip all incapacitated heroes and restore them to their maximum HP. Cards that were in a hero's deck, hand, or trash when they were incapacitated are returned to those respective locations.",
                var fakeList = new List<UnincapacitateHeroAction>();
                coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlay &&  IsHeroCharacterCard(c) && c.IsIncapacitated, "incapacitated hero character"),
                                                        SelectionType.UnincapacitateHero,
                                                        (Card c) => GameController.UnincapacitateHero(FindCardController(c), c.Definition.HitPoints ?? 0, null, fakeList, GetCardSource()),
                                                        allowAutoDecide: true,
                                                        cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //"Then, flip {TheMistressOfFate}'s villain character cards."
                coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if(!this.Card.IsFlipped && TurnTaker.Deck.IsEmpty)
            {
                coroutine = SpecialDefeatResponse(FakeAction);
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

        private Card FirstFaceDownDayCard()
        {
            var days = GameController.FindCardsWhere((Card c) => IsDay(c) && c.IsFaceDownNonCharacter).OrderBy((Card c) => c.PlayIndex);
            return days.FirstOrDefault();
        }

        private IEnumerator FlipDayCardResponse(PhaseChangeAction pc)
        {
            var dayCC = FindCardController(FirstFaceDownDayCard());
            if (dayCC != null)
            {
                IEnumerator coroutine = GameController.FlipCard(dayCC, cardSource: GetCardSource());
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

        private IEnumerator SpecialDefeatResponse(GameAction ga)
        {
            string ending = "The heroes' wills have been broken, and the Mistress of Fate has escaped her imprisonment. Game over.";
            IEnumerator coroutine = GameController.GameOver(EndingResult.AlternateDefeat, ending, true, cardSource: GetCardSource());
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

        private IEnumerator ContinueGameWithMessage(GameOverAction go)
        {
            IEnumerator coroutine = GameController.SendMessageAction("The Timeline continues to turn...", Priority.High, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = CancelAction(go);
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
        private IEnumerator SetRevivingHero(UnincapacitateHeroAction uha, bool clear = false)
        {
            if (clear)
                HeroBeingRevived = null;
            else
                HeroBeingRevived = uha.HeroCharacterCard?.TurnTaker;
            yield return null;
            yield break;
        }
        private bool IsCleaningUpIncappedHeroCards(BulkMoveCardsAction bmc)
        {
            if(bmc.Destination.IsOutOfGame && IsHero(bmc.Destination.OwnerTurnTaker))
            {
                return true;
            }
            return false;
        }
        private IEnumerator PreserveHero(BulkMoveCardsAction bmc)
        {
            var hero = bmc.Destination.OwnerTurnTaker;
            var cardsInHand = hero.ToHero().Hand.Cards;
            var cardsInDeck = hero.Deck.Cards;
            var cardsInTrash = hero.Trash.Cards;
            string hand = "hand";
            string deck = "deck";
            string trash = "trash";
            foreach(Card card in cardsInHand)
            {

                Game.Journal.RecordCardProperties(card, IncapLocation, new List<string>() { hand, cardsInHand.IndexOf(card).ToString() });
            }

            foreach (Card card in cardsInDeck)
            {
                Game.Journal.RecordCardProperties(card, IncapLocation, new List<string>() { deck, cardsInDeck.IndexOf(card).ToString() });

            }

            foreach (Card card in cardsInTrash)
            {
                Game.Journal.RecordCardProperties(card, IncapLocation, new List<string>() { trash, cardsInTrash.IndexOf(card).ToString() });
            }


            var cardsRemaining = GameController.GetAllCards().Where((Card c) => !c.IsCharacter && c.Owner == hero && !hero.Deck.HasCard(c) && !hero.Trash.HasCard(c) && !hero.ToHero().Hand.HasCard(c));
            int index = cardsInTrash.Count();
            foreach (Card card in cardsRemaining)
            {
                Game.Journal.RecordCardProperties(card, IncapLocation, new List<string>() { trash, index.ToString() });
                index++;
            }

            yield break;
        }

        private IEnumerator RestoreHeroCards(BulkMoveCardsAction bmc)
        {
            var hero = bmc.Destination.OwnerTurnTaker;
            var toHand = hero.GetAllCards().Where(c => Game.Journal.GetCardPropertiesStringList(c, IncapLocation) != null && Game.Journal.GetCardPropertiesStringList(c, IncapLocation).First() == "hand").OrderBy(c => Convert.ToInt32(Game.Journal.GetCardPropertiesStringList(c, IncapLocation).Last()));
            var toDeck = hero.GetAllCards().Where(c => Game.Journal.GetCardPropertiesStringList(c, IncapLocation) != null && Game.Journal.GetCardPropertiesStringList(c, IncapLocation).First() == "deck").OrderBy(c => Convert.ToInt32(Game.Journal.GetCardPropertiesStringList(c, IncapLocation).Last()));
            var toTrash = hero.GetAllCards().Where(c => Game.Journal.GetCardPropertiesStringList(c, IncapLocation) != null && Game.Journal.GetCardPropertiesStringList(c, IncapLocation).First() == "trash").OrderBy(c => Convert.ToInt32(Game.Journal.GetCardPropertiesStringList(c, IncapLocation).Last()));


            IEnumerator coroutine = GameController.BulkMoveCards(TurnTakerController, toDeck, hero.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.BulkMoveCards(TurnTakerController, toHand, hero.ToHero().Hand, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.BulkMoveCards(TurnTakerController, toTrash, hero.Trash, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = CancelAction(bmc);
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

        private bool IsDefaultCardRestoration(BulkMoveCardsAction bmc)
        {
            if (bmc.Destination.IsDeck && bmc.Destination.OwnerTurnTaker == HeroBeingRevived && bmc.CardSource == null)
            {
                return true;
            }
            return false;
        }

        protected bool IsDay(Card c)
        {
            if (c != null && c.Definition.Keywords.Contains("day"))
            {
                return true;
            }
            return false;
        }

        private string BuildRemovedCardsSpecialString()
        {
            var response = "Villain cards removed from the game: ";
            var removedCards = TurnTaker.GetCardsWhere((Card c) => c.Location.IsOutOfGame);
            if(removedCards.Any())
            {
                var faceUp = removedCards.Where((Card c) => !c.IsFlipped);
                var faceDown = removedCards.Where((Card c) => c.IsFaceDownNonCharacter);
                string faceUpString = "";
                string faceDownString = "";
                string joiner = "";
                if(faceUp.Any())
                {
                    faceUpString = String.Join(", ", faceUp.Select((Card c) => c.Title).ToArray());
                }
                if(faceDown.Any())
                {
                    faceDownString = $"{faceDown.Count()} unknown cards";
                }
                if(faceUpString != "" && faceDownString != "")
                {
                    joiner = " and ";
                }
                response += faceUpString + joiner + faceDownString + ".";
            }
            else
            {
                response += "None.";
            }
            return response;
        }
        private GameAction FakeAction => new PhaseChangeAction(GetCardSource(), GameController.ActiveTurnPhase, GameController.ActiveTurnPhase, true);
    }
}

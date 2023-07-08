using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class FutureEchelonCharacterCardController : HeroCharacterCardController
    {
        public FutureEchelonCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsDestroyedThisTurnEx(new LinqCardCriteria((Card c) => IsTactic(c), "tactic"));
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int numPlayers = GetPowerNumeral(0, 1);
            int numExtraDraw = GetPowerNumeral(1, 1);

            //"1 other player draws X+1 cards, where X is the number of tactic cards destroyed this turn."
            var selectTurnTakers = new SelectTurnTakersDecision(GameController, DecisionMaker, 
                                                new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && tt != DecisionMaker.TurnTaker && !tt.IsIncapacitatedOrOutOfGame), 
                                                SelectionType.DrawCard, 
                                                numPlayers, false, numPlayers,
                                                cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectTurnTakers, (TurnTaker tt) => DrawCardsBasedOnDestroyedTactics(tt, numExtraDraw), cardSource: GetCardSource());
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

        private IEnumerator DrawCardsBasedOnDestroyedTactics(TurnTaker tt, int numExtraCards)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var numToDraw = numExtraCards + NumberOfTacticCardsDestroyedThisTurn;
            IEnumerator coroutine = GameController.DrawCards(heroTTC, numToDraw, cardSource: GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now.",
                        coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"Play the top card of the environment deck.",
                        coroutine = GameController.PlayTopCardOfLocation(DecisionMaker, FindEnvironment().TurnTaker.Deck, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"Reveal the top card of 2 decks, play 1 and replace the other."
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        coroutine = SelectDecks(DecisionMaker, 2, SelectionType.RevealTopCardOfDeck, (Location l) => GameController.IsLocationVisibleToSource(l, GetCardSource()), storedResults);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        IEnumerable<Location> decks = from l in storedResults
                                                      where l.Completed && l.SelectedLocation.Location != null
                                                      select l.SelectedLocation.Location;
                        List<Card> storedCards = new List<Card>();
                        List<RevealCardsAction> storedReveal = new List<RevealCardsAction>();
                        for (int i = 0; i < decks.Count(); i++)
                        {
                            coroutine = GameController.RevealCards(DecisionMaker, decks.ElementAt(i), 1, storedCards, fromBottom: false, RevealedCardDisplay.Message, storedReveal, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        if (!storedCards.Any())
                        {
                            yield break;
                        }
                        coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => storedCards.Contains(c)), SelectionType.PlayCard, (Card c) => GameController.PlayCard(TurnTakerController, c, cardSource: GetCardSource()), 1, optional: false, 1, cardSource: GetCardSource(), ignoreBattleZone: true);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        var reveals = storedReveal.Where((RevealCardsAction rca) => rca.RevealedCards.FirstOrDefault()?.Location.IsRevealed == true);
                        foreach (RevealCardsAction reveal in reveals)
                        {
                            var otherCardDeck = reveal.SearchLocation;
                            var otherCard = reveal.RevealedCards.FirstOrDefault();
                            coroutine = GameController.MoveCard(DecisionMaker, otherCard, otherCardDeck, showMessage: true, cardSource: GetCardSource());
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
            }
            yield break;
        }

        private int NumberOfTacticCardsDestroyedThisTurn
        {
            get
            {
                return Journal.DestroyCardEntriesThisTurn().Count(dcje => dcje.DidCardHaveKeyword("tactic"));
            }
        }

        private bool IsTactic(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "tactic");
        }
    }
}

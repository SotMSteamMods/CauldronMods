using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class ShardmasterMalichaeCharacterCardController : HeroCharacterCardController
    {
        public ShardmasterMalichaeCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => c.DoKeywordsContain(MalichaeCardController.DjinnKeyword), "djinn"));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            var coroutine = SearchForCards(DecisionMaker, true, false, 1, 1,
                new LinqCardCriteria(c => c.DoKeywordsContain(MalichaeCardController.DjinnKeyword), "djinn"),
                false, true, false,
                shuffleAfterwards: true);
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
            switch (index)
            {
                case 0:
                    {
                        List<RevealCardsAction> storedResultsReveal = new List<RevealCardsAction>();
                        List<SelectTurnTakerDecision> storedResultsTurnTaker = new List<SelectTurnTakerDecision>();
                        var coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.RevealCardsFromDeck, true, false, storedResultsTurnTaker,
                                            cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectTurnTaker(storedResultsTurnTaker))
                        {
                            var tt = GetSelectedTurnTaker(storedResultsTurnTaker);
                            var httc = base.FindHeroTurnTakerController(tt.ToHero());

                            var orderedDestinations = new[]
                            {
                                new MoveCardDestination(tt.Trash),
                                new MoveCardDestination(tt.Deck)
                            };

                            coroutine = base.RevealCardsFromDeckToMoveToOrderedDestinations(httc, tt.Deck, orderedDestinations,
                                            numberOfCardsToReveal: 2);
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
                case 1:
                    {
                        var coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria(c => IsOngoing(c) && c.IsInPlay, "ongoing"), false, cardSource: GetCardSource());
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
                case 2:
                    {
                        IEnumerator drawCardRoutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(drawCardRoutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(drawCardRoutine);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ExpeditionOblaskPyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public ExpeditionOblaskPyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Select 3 Non-{PyreIrradiate} cards among all players' hands and {PyreIrradiate} them until they leave those players' hands."
            int numCards = GetPowerNumeral(0, 3);
            IEnumerator coroutine = SelectAndIrradiateCardsInHand(DecisionMaker, null, numCards, additionalCriteria: (Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()));
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card. If they do, each player may play a card now.",
                        var heroesWithIrradiated = GameController.AllHeroes.Where(htt => htt.Hand.Cards.Any(card => card.IsIrradiated()) && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
                        var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesWithIrradiated, SelectionType.DiscardCard, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DiscardForEverybodyPlays);
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
                        //"Destroy 2 environment cards.",
                        coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), 2, false, 2, cardSource: GetCardSource());
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
                case 2:
                    {
                        //"One player may draw a card now."
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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

        private IEnumerator DiscardForEverybodyPlays(TurnTaker tt)
        {
            //May discard a {PyreIrradiate} card
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var storedDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCard(heroTTC, true, card => card.IsIrradiated(), storedDiscard, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //If they do, each player may play a card now.
            if(DidDiscardCards(storedDiscard))
            {
                var activeHeroes = new LinqTurnTakerCriteria((TurnTaker turntaker) => IsHero(turntaker) && !turntaker.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(turntaker, GetCardSource()));
                var selectHero = new SelectTurnTakersDecision(GameController, DecisionMaker, activeHeroes, SelectionType.PlayCard, allowAutoDecide: true, cardSource: GetCardSource());
                coroutine = GameController.SelectTurnTakersAndDoAction(selectHero, (turntaker) => GameController.SelectAndPlayCardFromHand(FindHeroTurnTakerController(turntaker.ToHero()), true, cardSource: GetCardSource()));
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

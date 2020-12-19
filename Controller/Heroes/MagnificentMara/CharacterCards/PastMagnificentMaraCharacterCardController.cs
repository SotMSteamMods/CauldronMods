using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.MagnificentMara
{
    class PastMagnificentMaraCharacterCardController : MaraUtilityCharacterCardController
    {
        public PastMagnificentMaraCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"One player may play a relic card. Another may move a relic from their trash to the top of their deck. If neither happens, draw a card."
            var storedPlay = new List<PlayCardAction>();
            var storedDecidables = new List<bool>();
            var storedHero = new List<SelectTurnTakerDecision>();

            //One player may play a relic
            IEnumerator coroutine = SelectHeroToPlayRelic(storedPlay, storedHero, storedDecidables);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool couldSelectCardPlay = true;
            if(storedDecidables.Count() > 0)
            {
                couldSelectCardPlay = false;
            }

            TurnTaker excludedHero = null;
            if(DidPlayCards(storedPlay))
            {
                excludedHero = GetSelectedTurnTaker(storedHero);
            }
            var storedMove = new List<SelectCardDecision>();

            //Another may move a relic from their trash to the top of their deck.
            coroutine = SelectHeroToMoveRelicOnDeck(excludedHero, storedMove, couldSelectCardPlay);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If neither happens, draw a card.
            if (!DidPlayCards(storedPlay) && !DidSelectCard(storedMove))
            {
                coroutine = DrawCard(optional: false);
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

        private IEnumerator SelectHeroToPlayRelic(List<PlayCardAction> storedPlay, List<SelectTurnTakerDecision> storedHero, List<bool> couldDecide)
        {
            var heroesWithPlayableRelics = FindActiveHeroTurnTakerControllers()
                                                .Where(httc => GameController.GetPlayableCardsInHand(httc, false).Any(c => c.IsRelic) && AskIfTurnTakerIsVisibleToCardSource(httc.TurnTaker, GetCardSource()) != false)
                                                .Select(httc => httc.TurnTaker);
            if(heroesWithPlayableRelics.Count() == 0)
            {
                couldDecide.Add(false);
                yield break;
            }
            var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesWithPlayableRelics, SelectionType.PlayCard, isOptional: true, cardSource: GetCardSource());

            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, tt => PlayRelic(tt, storedPlay));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(selectHero.SelectedTurnTaker != null && DidPlayCards(storedPlay))
            {
                storedHero.Add(selectHero);
            }
            yield break;
        }

        private IEnumerator PlayRelic(TurnTaker tt, List<PlayCardAction> storedPlay)
        {
            IEnumerator coroutine = GameController.SelectAndPlayCardFromHand(FindHeroTurnTakerController(tt.ToHero()), true, storedPlay, new LinqCardCriteria((Card c) => c.IsRelic, "relic"), cardSource: GetCardSource());
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

        private IEnumerator SelectHeroToMoveRelicOnDeck(TurnTaker excludedHero, List<SelectCardDecision> storedMove, bool couldDecidePlay)
        {
            var heroesWithMovableRelics = FindActiveHeroTurnTakerControllers()
                                    .Where(httc => httc.TurnTaker != excludedHero && httc.TurnTaker.Trash.Cards.Any(c => c.IsRelic) && AskIfTurnTakerIsVisibleToCardSource(httc.TurnTaker, GetCardSource()) != false)
                                    .Select(httc => httc.TurnTaker);
            string message;
            IEnumerator coroutine;
            if(heroesWithMovableRelics.Count() == 0)
            {
                if (couldDecidePlay)
                {
                    message = "No hero could return a relic from their trash.";
                }
                else
                {
                    message = $"No heroes could play relics or return relics from their trash, so {DecisionMaker.TurnTaker.Name} will draw a card.";
                }

                coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
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

            if(!couldDecidePlay)
            {
                message = "No hero could play a relic.";
                coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            var selectDecision = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesWithMovableRelics, SelectionType.MoveCardOnDeck, isOptional: true, cardSource: GetCardSource());

            coroutine = GameController.SelectTurnTakerAndDoAction(selectDecision, tt => SelectAndMoveCardOptional(FindHeroTurnTakerController(tt.ToHero()), c => c.Location == tt.Trash && c.IsRelic, tt.Deck, optional: true, storedResults: storedMove, cardSource: GetCardSource()));
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

        private IEnumerator SelectAndMoveCardOptional(HeroTurnTakerController hero, Func<Card, bool> criteria, Location toLocation, bool toBottom = false, bool optional = true, bool isPutIntoPlay = false, bool playIfMovingToPlayArea = true, List<SelectCardDecision> storedResults = null, CardSource cardSource = null)
        {
            BattleZone battleZone = null;
            if (cardSource != null)
            {
                battleZone = cardSource.BattleZone;
            }
            SelectCardDecision selectCardDecision = new SelectCardDecision(GameController, hero, SelectionType.MoveCard, GameController.FindCardsWhere(criteria, realCardsOnly: true, null, battleZone), isOptional: optional, allowAutoDecide: false, null, null, null, null, null, maintainCardOrder: false, actionCanBeCancelled: true, cardSource);
            selectCardDecision.BattleZone = battleZone;
            storedResults?.Add(selectCardDecision);
            IEnumerator coroutine = GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision d) => GameController.MoveCard(hero, d.SelectedCard, toLocation, toBottom, isPutIntoPlay, playIfMovingToPlayArea, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, cardSource));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case (0):
                    {
                        //"One target deals another target 1 melee damage.",
                        break;
                    }
                case (1):
                    {
                        //"Put a card in a trash pile under its associated deck.",

                        break;
                    }
                case (2):
                    {
                        //"One player selects a keyword and reveals the top 3 cards of their deck, putting any revealed cards with that keyword into their hand and discarding the rest."
                        break;
                    }
            }
            yield break;
        }
    }
}

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Starlight
{
    public class Area51StarlightCharacterCardController : StarlightSubCharacterCardController
    {
        public Area51StarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria(c => c.Identifier == "PillarsOfCreation" && !c.IsInPlayAndHasGameText, "pillars of creation")).Condition = () => FindCardsWhere(c => c.Identifier == "PillarsOfCreation" && !c.IsInPlayAndHasGameText).Any();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Discard a card. Search your deck and trash for {PillarsOfCreation}. Play it. Shuffle your deck."
            IEnumerator coroutine = GameController.SelectAndDiscardCard(DecisionMaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var choices = FindCardsWhere(new LinqCardCriteria(c => c.Identifier == "PillarsOfCreation" && (TurnTaker.Deck.HasCard(c) || TurnTaker.Trash.HasCard(c)), "pillars of creation"));
            coroutine = GameController.SelectAndPlayCard(DecisionMaker, choices, cardSource: GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
             * "One player may draw a card now.",
             * "One hero may use a power now.",
             * "Destroy any number of hero ongoing cards. Then destroy 1 ongoing card."
             */
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        //"One player may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //* "Destroy any number of hero ongoing cards. Then destroy 1 ongoing card."
                        IEnumerator coroutine3 = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsHero && c.IsOngoing && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing"), null,
                                                    requiredDecisions: 0,
                                                    cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine3);
                        }
                        coroutine3 = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsOngoing && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "ongoing"), false,
                                                    cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
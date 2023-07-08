using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;

namespace Cauldron.Necro
{
    public class WardenOfChaosNecroCharacterCardController : HeroCharacterCardController
    {
        public WardenOfChaosNecroCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //Put the top card of your deck into play.
            IEnumerator coroutine;
            if (IsHero(base.TurnTaker))
            {
                coroutine = ((!base.TurnTaker.Deck.HasCards) ? base.GameController.SendMessageAction("There are no cards in " + base.TurnTaker.Name + "'s deck to play.", Priority.High, GetCardSource(), null, showCardSource: true) : base.GameController.PlayCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, isPutIntoPlay: true, cardSource: GetCardSource()));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = base.GameController.SendMessageAction(base.Card.Title + " has no deck to play cards from.", Priority.Medium, GetCardSource(), null, showCardSource: true);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One hero may put a random card from their trash into play.
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.PutIntoPlay, false, false, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectTurnTaker(storedResults))
                        {
                            TurnTaker tt = GetSelectedTurnTaker(storedResults);
                            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
                            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
                            coroutine = GameController.MakeYesNoCardDecision(httc, SelectionType.PlayCard, base.Card, storedResults: storedYesNoResults, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            if (DidPlayerAnswerYes(storedYesNoResults))
                            {
                                coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, tt.Trash, false, true, false, new LinqCardCriteria((Card c) => true), 1, shuffleBeforehand: true);
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
                    }
                case 1:
                    {
                        //Destroy 1 ongoing card.
                        IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController,
                                new LinqCardCriteria((Card c) => IsOngoing(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "ongoing"),
                                optional: false,
                                cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //The next time a target is destroyed, 1 player may draw 2 cards.
                        WhenCardIsDestroyedStatusEffect effect = new WhenCardIsDestroyedStatusEffect(base.Card, "DrawTwoCardsResponse", "The next time a target is destroyed, 1 player may draw 2 cards", new TriggerType[] { TriggerType.DrawCard }, DecisionMaker.HeroTurnTaker, base.Card);
                        effect.NumberOfUses = 1;
                        effect.CanEffectStack = true;
                        effect.CardDestroyedCriteria.IsTarget = true;
                        effect.CanEffectStack = true;
                        IEnumerator coroutine3 = AddStatusEffect(effect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }
            yield break;
        }

        public IEnumerator DrawTwoCardsResponse(DestroyCardAction dca, HeroTurnTaker htt, WhenCardIsDestroyedStatusEffect _, int[] _2 = null)
        {
            if (dca.WasCardDestroyed)
            {
                //1 player may draw 2 cards.
                HeroTurnTakerController httc = FindHeroTurnTakerController(htt);
                IEnumerator coroutine = GameController.SelectHeroToDrawCards(httc, 2, cardSource: GetCardSource());
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

    }
}

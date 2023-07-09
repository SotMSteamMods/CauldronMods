using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class FirstResponseEchelonCharacterCardController : HeroCharacterCardController
    {
        public FirstResponseEchelonCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Draw a card. 
            IEnumerator coroutine = DrawCard(optional: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may play The Kestrel Mark II from your hand."
            coroutine = GameController.SelectAndPlayCardsFromHand(DecisionMaker, 1, false, 0, new LinqCardCriteria((Card c) => c.Identifier == "TheKestrelMarkII", "The Kestrel Mark II"), cardSource: GetCardSource());
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
                        //"Up to 2 equipment cards may be played now.",
                        var storedPlay = new List<PlayCardAction> { };
                        Func<Card, bool> playableEquipmentInHand = (delegate (Card c)
                        {
                            return c.IsInHand && IsHero(c) && IsEquipment(c) &&
                                      GameController.IsCardVisibleToCardSource(c, GetCardSource()) &&
                                      GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay;
                        });
                        coroutine = GameController.SelectAndPlayCard(DecisionMaker, playableEquipmentInHand, true, cardSource: GetCardSource(), noValidCardsMessage: "There were no playable equipment cards", storedResults: storedPlay);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //If we did pick one, play another
                        if (storedPlay.FirstOrDefault() != null && storedPlay.FirstOrDefault().CardToPlay != null)
                        {
                            coroutine = GameController.SelectAndPlayCard(DecisionMaker, playableEquipmentInHand, true, cardSource: GetCardSource(), noValidCardsMessage: "There were no more playable equipment cards");
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
                        //"Look at the top 3 cards of a hero deck and replace them in any order."
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        coroutine = GameController.SelectTurnTaker(DecisionMaker, SelectionType.RevealCardsFromDeck, storedResults, optional: false, allowAutoDecide: false, (TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame, 3, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectTurnTaker(storedResults))
                        {
                            TurnTaker selectedTurnTaker = GetSelectedTurnTaker(storedResults);
                            coroutine = RevealTheTopCardsOfDeck_MoveInAnyOrder(DecisionMaker, DecisionMaker, selectedTurnTaker, 3);
                            if (base.UseUnityCoroutines)
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
                        //"Destroy an environment card."
                        coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment"), false, cardSource: GetCardSource());
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
            }
            yield break;
        }
    }
}

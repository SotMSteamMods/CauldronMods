using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class TheCervantesClubCardController : CatchwaterHarborUtilityCardController
    {
        public TheCervantesClubCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.GainHP,
                TriggerType.MoveCard
            });
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
            IEnumerator coroutine = GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.GainHP, (Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(!DidSelectCards(storedResults))
            {
                yield break;
            }
            if(storedResults.Count == 0 || storedResults.First() == null)
            {
                yield break;
            }
            Card hero = storedResults.First().SelectCardDecisions.First().SelectedCard;
            HeroTurnTakerController httc = FindHeroTurnTakerController(hero.Owner.ToHero());
            var response1 = RegainAndDiscardTop(httc, hero, 1);
            var op1 = new Function(httc , $"Regain 1 HP and discard the top card of your deck.", SelectionType.GainHP, () => response1);

            var response2 = RegainAndDiscardTop(httc, hero, 2);
            var op2 = new Function(httc, $"Regain 2 HP and discard the top 2 cards of your deck.", SelectionType.GainHP, () => response2);
            
            var response3 = RegainAndDiscardTop(httc, hero, 3);
            var op3 = new Function(httc, $"Regain 3 HP and discard the top 3 cards of your deck.", SelectionType.GainHP, () => response3);

            //Execute
            var options = new Function[] { op1, op2, op3 };
            var selectFunctionDecision = new SelectFunctionDecision(base.GameController, this.DecisionMaker, options, false, cardSource: base.GetCardSource());
           coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
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

        private IEnumerator RegainAndDiscardTop(HeroTurnTakerController httc, Card hero, int X)
        {
            IEnumerator coroutine = GameController.GainHP(hero, X, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<MoveCardAction> storedMove = new List<MoveCardAction>();
            coroutine = GameController.DiscardTopCards(httc, httc.TurnTaker.Deck, X, storedResults: storedMove, showCards: (Card c) => true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidMoveCard(storedMove))
            {
                IEnumerable<Card> movedCards = storedMove.Select(mca => mca.CardToMove);
                if(movedCards.Any(c => c.IsOneShot))
                {
                    //that player discards 2 cards, then draws a card.
                    coroutine = GameController.SelectAndDiscardCards(httc, 2, false, 2, cardSource: GetCardSource());
                    IEnumerator coroutine2 = DrawCard(httc.HeroTurnTaker);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                        yield return base.GameController.StartCoroutine(coroutine2);

                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                        base.GameController.ExhaustCoroutine(coroutine2);
                    }
                }
            }
        }
    }
}

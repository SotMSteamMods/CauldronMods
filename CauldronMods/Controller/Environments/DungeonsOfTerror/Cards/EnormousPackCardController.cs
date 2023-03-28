using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class EnormousPackCardController : DungeonsOfTerrorUtilityCardController
    {
        public EnormousPackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildTopCardOfLocationSpecialString(TurnTaker.Trash));
            SpecialStringMaker.ShowHeroWithMostCards(true).Condition = () => IsTopCardOfLocationFate(TurnTaker.Trash) == false || IsRingOfForesightInPlay();
        }

        public override IEnumerator Play()
        {
            //When this card enters play, check the top card of the environment trash.
            List<int> storedResults = new List<int>();
            Card cardToCheck = TurnTaker.Trash.TopCard;
            List<bool> suppressMessage = new List<bool>();
            IEnumerator coroutine = CheckForNumberOfFates(cardToCheck.ToEnumerable(), storedResults, TurnTaker.Trash, suppressMessage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator message = DoNothing();
            IEnumerator effect = DoNothing();
            if (storedResults.Any() && storedResults.First() == 1)
            {
                //If it is a fate card, 1 hero may play card, draw a card, or use a power
                message = GameController.SendMessageAction($"The top card of the environment trash is a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = OneHeroMayPlayDrawOrUsePower(); ;
            }
            else if (storedResults.Any() && storedResults.First() == 0)
            {
                //If it is not a fate card, the player with the most cards in hand discards 2 cards
                if (suppressMessage.Any() && suppressMessage.First() != true)
                {
                    message = GameController.SendMessageAction($"The top card of the environment trash is not a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                }
                effect = MostCardsInHandDiscards2();
            }
            else
            {
                message = GameController.SendMessageAction("There are no cards in the environment trash!", Priority.High, GetCardSource(), showCardSource: true);
            }
            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);
                yield return base.GameController.StartCoroutine(effect);
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
                base.GameController.ExhaustCoroutine(effect);
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator OneHeroMayPlayDrawOrUsePower()
        {
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.MakeDecision, false, false, storedResults: storedResults, heroCriteria: new LinqTurnTakerCriteria(tt => !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), cardSource: GetCardSource());
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
                HeroTurnTakerController httc = FindHeroTurnTakerController(GetSelectedTurnTaker(storedResults).ToHero());

                //op1: play a card
                var response1 = GameController.SelectAndPlayCardFromHand(httc, false, cardSource: GetCardSource());
                var op1 = new Function(httc, "Play a card", SelectionType.PlayCard, () => response1);

                //op2: draw a card
                var response2 = DrawCard(hero: httc.HeroTurnTaker);
                var op2 = new Function(httc, "Draw a card", SelectionType.DrawCard, () => response2);

                //op3: use a power
                var response3 = GameController.SelectAndUsePower(httc, optional: false, cardSource: GetCardSource());
                var op3 = new Function(httc, "Use a power", SelectionType.UsePower, () => response3);

                //Execute
                var options = new Function[] { op1, op2, op3 };
                var selectFunctionDecision = new SelectFunctionDecision(base.GameController, httc, options, false, cardSource: base.GetCardSource());
                coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
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

        private IEnumerator MostCardsInHandDiscards2()
        {
            List<TurnTaker> storedHero = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInHand(storedHero);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedHero.Count() <= 0)
            {
                yield break;
            }
            TurnTaker turnTaker = storedHero.First();
            if (turnTaker != null && IsHero(turnTaker))
            {
                coroutine = SelectAndDiscardCards(FindHeroTurnTakerController(turnTaker.ToHero()), 2);
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

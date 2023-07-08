using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class SubharmonicReceiverCardController : CardController
    {
        public SubharmonicReceiverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Each player may draw a card. When a player draws a card this way, 1 other player must discard a card.
            int otherPlayers = GetPowerNumeral(0, 1);

            var criteria = new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()));

            var selectTurnTakersDecision = new SelectTurnTakersDecision(GameController, DecisionMaker, criteria, SelectionType.DrawCard,
                                            allowAutoDecide: true, cardSource: GetCardSource());

            var coroutine = GameController.SelectTurnTakersAndDoAction(selectTurnTakersDecision, (TurnTaker hero) => OptionalDrawAndDiscard(hero.ToHero(), otherPlayers), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator OptionalDrawAndDiscard(HeroTurnTaker hero, int numberOfDiscards)
        {
            List<DrawCardAction> result = new List<DrawCardAction>();
            var coroutine = DrawCard(hero, optional: true, cardsDrawn: result, allowAutoDraw: numberOfDiscards == 0);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDrawCards(result, 1))
            {
                for (int i = 0; i < numberOfDiscards; i++)
                {
                    //...1 other player must discard a card.
                    IEnumerable<TurnTaker> choices = GameController.FindTurnTakersWhere(tt => !tt.IsIncapacitatedOrOutOfGame && IsHero(tt) && tt != hero);
                    SelectTurnTakerDecision decision = new SelectTurnTakerDecision(GameController, HeroTurnTakerController, choices, SelectionType.DiscardCard, cardSource: GetCardSource());
                    coroutine = base.GameController.SelectTurnTakerAndDoAction(decision, (TurnTaker tt) => GameController.SelectAndDiscardCard(FindHeroTurnTakerController(tt.ToHero())));
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
        }
    }
}
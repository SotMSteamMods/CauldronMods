using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class ForewarnedCardController : CardController
    {
        public ForewarnedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var query = GameController.HeroTurnTakerControllers.Select(httc => httc.HeroTurnTaker)
                                                               .Where(htt => htt.NumberOfCardsInHand < 3)
                                                               .Select(htt => htt.Hand);
            var ss = SpecialStringMaker.ShowNumberOfCardsAtLocations(() => query);
            ss.Condition = () => GameController.HeroTurnTakerControllers.Any(HeroTurnTakerController => HeroTurnTakerController.HeroTurnTaker.NumberOfCardsInHand < 3);

            ss = SpecialStringMaker.ShowSpecialString(() => "No hero has fewer than 3 cards in hand");
            ss.Condition = () => GameController.HeroTurnTakerControllers.All(HeroTurnTakerController => HeroTurnTakerController.HeroTurnTaker.NumberOfCardsInHand >= 3);
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(tt => tt == this.TurnTaker, StartOfTurnResponse, new TriggerType[]
                {
                    TriggerType.DestroySelf,
                    TriggerType.DrawCard,
                });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction action)
        {
            var results = new List<YesNoCardDecision>();
            var coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, Card, action: action, storedResults: results, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidPlayerAnswerYes(results))
            {
                coroutine = GameController.DestroyCard(DecisionMaker, Card,
                                actionSource: action,
                                postDestroyAction: DestroyCardReponse,
                                cardSource: GetCardSource());
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

        private IEnumerator DestroyCardReponse()
        {
            var cardSource = GetCardSource();
            SelectTurnTakersDecision decision = new SelectTurnTakersDecision(base.GameController, DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsPlayer && base.GameController.IsTurnTakerVisibleToCardSource(tt, cardSource) && CanDrawCards(FindHeroTurnTakerController(tt.ToHero()))), SelectionType.DrawCard, allowAutoDecide: true, cardSource: cardSource);
            IEnumerator coroutine = base.GameController.SelectTurnTakersAndDoActionEx(decision, (TurnTaker tt) => DrawCardsUntilHandSizeReached(FindHeroTurnTakerController(tt.ToHero()), 3), cardSource: cardSource);
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
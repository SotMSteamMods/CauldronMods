using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class ZephaerensCompassCardController : MalichaeCardController
    {
        public ZephaerensCompassCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerable<Card> choices = FindCardsWhere(c => c.IsTarget && c.IsInPlayAndHasGameText && IsDjinn(c), visibleToCard: GetCardSource());
            var scd = new SelectCardDecision(GameController, DecisionMaker, SelectionType.MoveCardToHand, choices,
                            cardSource: GetCardSource());
            var coroutine = base.GameController.SelectCardAndDoAction(scd, CompassPowerResponse);
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

        private IEnumerator CompassPowerResponse(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null)
            {
                int plus = GetPowerNumeral(0, 1);
                var number = scd.SelectedCard.NextToLocation.Cards.Select(c => IsDjinn(c) && IsOngoing(c)).Count() + plus;
                List<MoveCardAction> results = new List<MoveCardAction>();
                var coroutine = GameController.MoveCard(DecisionMaker, scd.SelectedCard, DecisionMaker.HeroTurnTaker.Hand,
                                    decisionSources: (IEnumerable<IDecision>)scd.ToEnumerable(),
                                    storedResults: results,
                                    cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidMoveCard(results))
                {
                    coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsInPlay && (c.IsEnvironment || IsOngoing(c)), "ongoing or enviroment"), number,
                                        requiredDecisions: 0,
                                        responsibleCard: DecisionMaker.CharacterCard,
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
            yield break;
        }
    }
}

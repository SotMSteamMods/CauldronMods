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
            var scd = new SelectCardDecision(GameController, DecisionMaker, SelectionType.MoveCardToHand, DecisionMaker.HeroTurnTaker.PlayArea.Cards,
                            additionalCriteria: c => c.IsTarget && IsDjinn(c),
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
                var number = scd.SelectedCard.NextToLocation.NumberOfCards + plus;
                List<MoveCardAction> results = new List<MoveCardAction>();
                var coroutine = GameController.MoveCard(DecisionMaker, scd.SelectedCard, DecisionMaker.HeroTurnTaker.Hand,
                                    decisionSources: scd.ToEnumerable(),
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
                    coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment || c.IsOngoing, "ongoing or enviroment"), number,
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

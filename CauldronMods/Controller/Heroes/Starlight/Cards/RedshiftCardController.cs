using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class RedshiftCardController : StarlightCardController
    {
        public RedshiftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Draw a card."
            IEnumerator draw = DrawCard(HeroTurnTaker);
            //"2 players may play 1 card."
            IEnumerator allowPlays = GameController.SelectTurnTakersAndDoAction(DecisionMaker,
                                                                                new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && CanPlayCardsFromHand(GameController.FindHeroTurnTakerController(tt.ToHero()))),
                                                                                SelectionType.PlayCard,
                                                                                (TurnTaker tt) => SelectAndPlayCardFromHand(GameController.FindHeroTurnTakerController(tt.ToHero())),
                                                                                numberOfTurnTakers: 2,
                                                                                optional: false,
                                                                                requiredDecisions: 0,
                                                                                cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(draw);
                yield return GameController.StartCoroutine(allowPlays);
            }
            else
            {
                GameController.ExhaustCoroutine(draw);
                GameController.ExhaustCoroutine(allowPlays);
            }

            yield break;
        }
    }
}
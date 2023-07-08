using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class IntrepidReporterCardController : ResponderCardController
    {
        public IntrepidReporterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override IEnumerator PerformActionOnDestroy()
        {
            //1 player may draw a card
            IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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

        public override IEnumerator Play()
        {
            //When this card enters play, 2 players may each draw a card.

            SelectTurnTakersDecision selectTurnTakersDecision = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.DrawCard, numberOfTurnTakers: 2, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectTurnTakersDecision, (TurnTaker tt) => DrawCard(tt.ToHero(), optional: true), cardSource: GetCardSource());
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
    }
}

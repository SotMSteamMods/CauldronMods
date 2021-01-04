using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class MapToLostChothCardController : ArtifactCardController
    {
        public MapToLostChothCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UniqueOnPlayEffect()
        {
            //1 player discards a card...
            List<TurnTaker> usedTurnTakers = new List<TurnTaker>();
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = base.GameController.SelectHeroToDiscardCard(DecisionMaker, storedResultsTurnTaker: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectTurnTaker(storedResults))
            {
                usedTurnTakers.Add(GetSelectedTurnTaker(storedResults));
            }
            //... a second player plays a card...
            storedResults = new List<SelectTurnTakerDecision>();
            coroutine = SelectHeroToPlayCard(DecisionMaker, optionalPlayCard: false, storedResultsTurnTaker: storedResults, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !usedTurnTakers.Contains(tt)));
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
                usedTurnTakers.Add(GetSelectedTurnTaker(storedResults));
            }
            //... and a third player’s hero uses a power
            coroutine = GameController.SelectHeroToUsePower(DecisionMaker, optionalUsePower: false, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !usedTurnTakers.Contains(tt)), cardSource: GetCardSource());
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

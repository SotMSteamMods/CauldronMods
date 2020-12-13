using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public abstract class AugBaseCardController : CypherBaseCardController
    {
        protected AugBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowCardThisCardIsNextTo(Card);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources,
            Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, place it next to a hero target.
            LinqCardCriteria validTargets = new LinqCardCriteria(c => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, "hero target");

            IEnumerator routine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(c => validTargets.Criteria(c) &&
                    (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), validTargets.Description),
                storedResults, isPutIntoPlay, decisionSources);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}
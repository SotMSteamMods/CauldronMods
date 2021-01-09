using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class StolenFutureCardController : TheMistressOfFateUtilityCardController
    {
        public StolenFutureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to the hero with the highest HP.
            return base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
        }

        public override IEnumerator Play()
        {
            //[The hero this is next to] may deal 1 target 5 radiant damage. 
            //Then, destroy this card.
            yield break;
        }

        public override void AddTriggers()
        {
            //"When this card is destroyed, destroy all hero characters in its play area."
        }
    }
}

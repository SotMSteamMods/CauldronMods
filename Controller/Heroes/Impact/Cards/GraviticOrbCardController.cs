using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class GraviticOrbCardController : CardController
    {
        public GraviticOrbCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a target.
            return base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
        }
        public override IEnumerator Play()
        {
            //"Play this card next to a target. {Impact} deals that target 2 infernal damage.",
            yield break;  
        }

        public override void AddTriggers()
        {
            //"When the target next to this card would deal damage, destroy this card and prevent that damage."
        }
    }
}
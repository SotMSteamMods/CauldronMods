using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class IllusionOfFreeWillCardController : TheMistressOfFateUtilityCardController
    {
        public IllusionOfFreeWillCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to the hero with the lowest HP.
            return base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
        }

        public override void AddTriggers()
        {
            //Destroy this card if {TheMistressOfFate} flips or there are ever less than 5 cards in the villain deck.",
            //"Redirect damage dealt by this hero’s cards to the hero with the highest HP. At the end of this hero's turn, play the top 2 cards of their deck."
        }
    }
}

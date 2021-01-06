using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public abstract class MythosUtilityCardController : CardController
    {
        protected MythosUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected const string MythosClueDeckIdentifier = "MythosClue";
        protected const string MythosDangerDeckIdentifier = "MythosDanger";
        protected const string MythosMadnessDeckIdentifier = "MythosMadness";

        public bool IsTopCardMatching(string type)
        {
            //Advanced: Activate all {MythosDanger} effects.
            if (base.Game.IsAdvanced && type == MythosDangerDeckIdentifier)
            {
                return true;
            }
            return base.TurnTaker.Deck.TopCard.ParentDeck.Identifier == type;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            storedResults.Add(new MoveCardDestination(base.TurnTaker.PlayArea));
            yield return null;
            yield break;
        }

        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(base.TurnTaker.Trash);
        }
    }
}

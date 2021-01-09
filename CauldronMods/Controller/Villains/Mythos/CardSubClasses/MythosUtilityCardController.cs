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

        protected const string MythosEyeDeckIdentifier = "MythosEye";
        protected const string MythosFearDeckIdentifier = "MythosFear";
        protected const string MythosMindDeckIdentifier = "MythosMind";

        public abstract string DeckIdentifier { get; }

        public bool IsTopCardMatching(string type)
        {
            //Advanced: Activate all {MythosDanger} effects.
            if (base.Game.IsAdvanced && type == MythosFearDeckIdentifier)
            {
                return true;
            }
            string[] eyeIdentifiers = { "DangerousInvestigation", "PallidAcademic", "Revelations", "RitualSite", "RustedArtifact", "TornPage" };
            string[] fearIdentifiers = { "AclastyphWhoPeers", "FaithfulProselyte", "OtherworldlyAlignment", "PreyUponTheMind" };
            string[] mindIdentifiers = { "ClockworkRevenant", "DoktorVonFaust", "HallucinatedHorror", "WhispersAndLies", "YourDarkestSecrets" };
            string topIdentifier = null;
            if (eyeIdentifiers.Contains(base.TurnTaker.Deck.TopCard.Identifier))
            {
                topIdentifier = MythosEyeDeckIdentifier;
            }
            if (fearIdentifiers.Contains(base.TurnTaker.Deck.TopCard.Identifier))
            {
                topIdentifier = MythosFearDeckIdentifier;
            }
            if (mindIdentifiers.Contains(base.TurnTaker.Deck.TopCard.Identifier))
            {
                topIdentifier = MythosMindDeckIdentifier;
            }
            return topIdentifier == type;
            //Once the UI allows for this, remove above.
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

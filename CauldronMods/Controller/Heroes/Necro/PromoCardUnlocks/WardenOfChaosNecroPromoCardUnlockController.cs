using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Necro
{
    public class WardenOfChaosNecroPromoCardUnlockController : PromoCardUnlockController
    {

        public WardenOfChaosNecroPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Necro", "WardenOfChaosNecroCharacter")
        {

        }



        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame("Necro", "NecroCharacter");

        }

        private bool CheckChaoticSummonPlays()
        {

            string chaoticSummonIdentifier = "ChaoticSummon";
            IEnumerable<MoveCardJournalEntry> moveCardJournalEntries = from mcje in Journal.MoveCardEntriesThisTurn()
                                                                       where mcje.Card.Identifier == chaoticSummonIdentifier && mcje.CardSource != null &&  mcje.CardSource.Identifier == chaoticSummonIdentifier
                                                                       select mcje;
            return moveCardJournalEntries.Count() >= 2;
        }

        public override bool CheckForUnlock(GameAction action)
        {

            bool condition1 = CheckChaoticSummonPlays();

            if (condition1)
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }


    }
}
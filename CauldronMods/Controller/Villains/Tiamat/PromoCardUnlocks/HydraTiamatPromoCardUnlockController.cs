using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Tiamat
{
    public class HydraTiamatPromoCardUnlockController : PromoCardUnlockController
    {

        public HydraTiamatPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Tiamat", "HydraWinterTiamatCharacter")
        {

        }


        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame("Tiamat", "WinterTiamatCharacter") && IsInGame("TheKnight") && IsInGame("Echelon") && IsInGame("Necro");
        }
       
        public override bool CheckForUnlock(GameAction action)
        {
            string theKnightIdentifier = "TheKnightCharacter";
            string echelonIdentifier = "EchelonCharacter";
            string necroIdentifier = "NecroCharacter";
            string infernoTiamatIdentifier = "InfernoTiamatCharacter";
            string stormTiamatIdentifier = "StormTiamatCharacter";
            string winterTiamatIdentifier = "WinterTiamatCharacter";

            if (!IsGameOver(action))
                return IsUnlocked;

            bool condition1 = (action as GameOverAction).ResultIsVictory;
            bool condition2 = DidDecapitateHead((Card c) => c.Identifier == theKnightIdentifier, infernoTiamatIdentifier);
            bool condition3 = DidDecapitateHead((Card c) => c.Identifier == echelonIdentifier, stormTiamatIdentifier);
            bool condition4 = DidDecapitateHead((Card c) => c.Identifier == necroIdentifier, winterTiamatIdentifier);

            if (condition1 && condition2 && condition3 && condition4)
            {
                IsUnlocked = true;

            }

            return IsUnlocked;
        }

        protected bool DidDecapitateHead(Func<Card, bool> damageSourceCriteria, string targetIdentifier, Func<DealDamageJournalEntry, bool> additionalCriteria = null)
        {
            DealDamageJournalEntry dealDamageJournalEntry = FindMostRecentDamage(damageSourceCriteria, targetIdentifier, additionalCriteria);

            FlipCardJournalEntry flipCardJournalEntry = FindMostRecentFlip(targetIdentifier);
            if (dealDamageJournalEntry != null && flipCardJournalEntry != null)
            {
                return dealDamageJournalEntry.SourceCard == flipCardJournalEntry.DamageSource;
            }
            
            return false;
        }

    }
}
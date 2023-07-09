using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.MissInformation;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.DocHavoc
{
    public class FirstResponseDocHavocPromoCardUnlockController : PromoCardUnlockController
    {

        public FirstResponseDocHavocPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.DocHavoc", "FirstResponseDocHavocCharacter")
        {

        }

        public override bool IsUnlockPossibleThisGame()
        {
            return AreInGame(new string[]
            {
                "Cypher",
                "Cricket",
                "Echelon",
                "Vanish",
                "DocHavoc",
                "WindmillCity"
            });
        }
       
        public override bool CheckForUnlock(GameAction action)
        {
           
            if (!IsGameOver(action))
                return IsUnlocked;

            bool condition1 = (action as GameOverAction).ResultIsVictory;
            bool condition2 = CheckDocHavocHeals();
            bool condition3 = CheckDocHavocEnvironmentDestruction();

            if (condition1 && condition2 && condition3)
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }

        private bool CheckDocHavocEnvironmentDestruction()
        {
            string docHavocName = "Doc Havoc";
            string emergencyKeyword = "emergency";
            IEnumerable<DestroyCardJournalEntry> destroyCardJournalEntries = from dcje in base.Journal.DestroyCardEntries()
                                                                   where dcje.DidCardHaveKeyword(emergencyKeyword) && dcje.Card.IsEnvironment && dcje.CardSource.Owner.Name == docHavocName
                                                                   select dcje;
            return destroyCardJournalEntries.Count() > 1;
        }

        private bool CheckDocHavocHeals()
        {
            string cricketIdentifier = "CricketCharacter";
            string cypherIdentifier = "CypherCharacter";
            string echelonIdentifier = "EchelonCharacter";
            string docHavocName = "Doc Havoc";
            string vanishIdentifier = "VanishCharacter";

            int gainHpThreshold = 9;

            IEnumerable<GainHPJournalEntry> gainHPJournalEntries = from ghje in base.Journal.GainHPEntries()
                                                                  where ghje.SourceCard != null && ghje.SourceCard.Owner.Name == docHavocName && ghje.TargetCard != null && ghje.TargetCard.IsHeroCharacterCard
                                                                  select ghje;
    
            bool cricketFlag = gainHPJournalEntries.Where(ghje => ghje.TargetCard.Identifier == cricketIdentifier).Sum(ghje => ghje.Amount) > gainHpThreshold;
            bool echelonFlag = gainHPJournalEntries.Where(ghje => ghje.TargetCard.Identifier == echelonIdentifier).Sum(ghje => ghje.Amount) > gainHpThreshold;
            bool cypherFlag = gainHPJournalEntries.Where(ghje => ghje.TargetCard.Identifier == cypherIdentifier).Sum(ghje => ghje.Amount) > gainHpThreshold;
            bool vanishFlag = gainHPJournalEntries.Where(ghje => ghje.TargetCard.Identifier == vanishIdentifier).Sum(ghje => ghje.Amount) > gainHpThreshold;

            return cricketFlag && echelonFlag && cypherFlag && vanishFlag;
        }

       

    }
}
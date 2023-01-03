using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Vanish
{
    public class FirstResponseVanishPromoCardUnlockController : PromoCardUnlockController
    {

        public FirstResponseVanishPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Vanish", "FirstResponseVanishCharacter")
        {

        }

        public const string FirstResponseVanishUnlockCondition1 = "FirstResponseVanishUnlockCondition1";


        public override bool IsUnlockPossibleThisGame()
        {
            return AreInGame(new string[]
            {
                "Gray",
                "Vanish",
                "Echelon",
                "DocHavoc",
                "Cricket",
                "Cypher",
                "WindmillCity"
            }) && HasFlagBeenSetToTrue(FirstResponseVanishUnlockCondition1);
        }

        public override bool IsFlagPossibleThisGame()
        {
            return !HasFlagBeenSetToTrue(FirstResponseVanishUnlockCondition1) &&
                    AreInGame(new string[]
                    {
                        "Gray",
                        "Vanish",
                        "WindmillCity"
                    });
        }

        public override void CheckForFlags(GameAction action)
        {
         
            if (IsGameOverDefeat(action))
            {
                bool condition1 = DidGrayDestroyBuilding();
                if(condition1)
                {
                    SetFlag(FirstResponseVanishUnlockCondition1, value: true);
                    ContinueCheckingForFlags = false;
                }
            }
        }

        private bool DidGrayDestroyBuilding()
        {
            string buildingIdentifier = "GrayPharmaceuticalBuilding";
            string grayName = "Gray";
            IEnumerable<DestroyCardJournalEntry> destroyCardJournalEntries = from dcje in base.Journal.DestroyCardEntries()
                                                                             where dcje.Card.Identifier == buildingIdentifier && dcje.CardSource.Owner.Name == grayName
                                                                             select dcje;

            return destroyCardJournalEntries.Any();
        }

        public override bool CheckForUnlock(GameAction action)
        {

            if (!IsGameOver(action))
                return IsUnlocked;

            bool condition1 = (action as GameOverAction).ResultIsVictory;
            bool condition2 = DidVanishCauseEachOtherHeroToDealDamageToGray();

       
            if (condition1 && condition2)
            {
                IsUnlocked = true;

            }

            return IsUnlocked;
        }

        private bool DidVanishCauseEachOtherHeroToDealDamageToGray()
        {
            string vanishName = "Vanish";
            string grayIdentifier = "GrayCharacter";
            string cricketIdentifier = "CricketCharacter";
            string cypherIdentifier = "CypherCharacter";
            string echelonIdentifier = "EchelonCharacter";
            string docHavocIdentifier = "DocHavocCharacter";
            IEnumerable<DealDamageJournalEntry> dealDamageJournalEntries = from ddje in base.Journal.DealDamageEntries()
                                                                             where ddje.CardThatCausedDamageToOccur != null && ddje.CardThatCausedDamageToOccur.Owner.Name == vanishName && ddje.TargetCard.Identifier == grayIdentifier && ddje.SourceCard != null && ddje.SourceCard.IsHeroCharacterCard
                                                                             select ddje;

            bool cricketFlag = dealDamageJournalEntries.Where(ddje => ddje.SourceCard.Identifier == cricketIdentifier).Sum(ddje => ddje.Amount) > 0;
            bool cypherFlag = dealDamageJournalEntries.Where(ddje => ddje.SourceCard.Identifier == cypherIdentifier).Sum(ddje => ddje.Amount) > 0;
            bool echelonFlag = dealDamageJournalEntries.Where(ddje => ddje.SourceCard.Identifier == echelonIdentifier).Sum(ddje => ddje.Amount) > 0;
            bool docHavocFlag = dealDamageJournalEntries.Where(ddje => ddje.SourceCard.Identifier == docHavocIdentifier).Sum(ddje => ddje.Amount) > 0;


            return cricketFlag && cypherFlag && echelonFlag && docHavocFlag;
        }
    }
}
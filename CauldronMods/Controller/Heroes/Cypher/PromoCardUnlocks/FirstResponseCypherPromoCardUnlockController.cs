using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Cypher
{
    public class FirstResponseCypherPromoCardUnlockController : PromoCardUnlockController
    {

        public FirstResponseCypherPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Cypher", "FirstResponseCypherCharacter")
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
            bool condition2 = CheckCypherAugmentPlays();

            if (condition1 && condition2)
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }

        private bool CheckCypherAugmentPlays()
        {
            string cricketIdentifier = "Cricket";
            string echelonIdentifier = "Echelon";
            string docHavocIdentifier = "Doc Havoc";
            string vanishIdentifier = "Vanish";

            IEnumerable<MoveCardJournalEntry> moveCardJournalEntries = from mcje in base.Journal.MoveCardEntries()
                                                     where IsAugment(mcje.Card) && mcje.ToLocation.IsHeroPlayAreaRecursive
                                                     select mcje;
            bool cricketFlag = moveCardJournalEntries.Count(mcje => mcje.ToLocation.HighestRecursiveLocation.GetOwnerName() == cricketIdentifier) > 1;
            bool echelonFlag = moveCardJournalEntries.Count(mcje => mcje.ToLocation.HighestRecursiveLocation.GetOwnerName() == echelonIdentifier) > 1;
            bool docHavocFlag = moveCardJournalEntries.Count(mcje => mcje.ToLocation.HighestRecursiveLocation.GetOwnerName() == docHavocIdentifier) > 1;
            bool vanishFlag = moveCardJournalEntries.Count(mcje => mcje.ToLocation.HighestRecursiveLocation.GetOwnerName() == vanishIdentifier) > 1;


            return cricketFlag && echelonFlag && docHavocFlag && vanishFlag;
        }

        protected bool IsAugment(Card card)
        {

            if (card != null)
            {
                if (card.HasGameText && base.GameController.DoesCardContainKeyword(card, "augment"))
                    return true;

                if (card.IsInPlay && card.IsFaceDownNonCharacter && IsNanocloud(card) == true)
                    return true;
            }
            return false;
        }

        protected bool IsNanocloud(Card c)
        {
            return GameController.GetCardPropertyJournalEntryBoolean(c, CypherBaseCardController.NanocloudKey) == true;
        }

    }
}
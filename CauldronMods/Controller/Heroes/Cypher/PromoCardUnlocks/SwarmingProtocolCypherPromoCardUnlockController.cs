using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Cypher
{
    public class SwarmingProtocolCypherPromoCardUnlockController : PromoCardUnlockController
    {

        public SwarmingProtocolCypherPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Cypher", "SwarmingProtocolCypherCharacter")
        {

        }

        public override bool IsUnlockPossibleThisGame()
        {
            return AreInGame(new string[]
            {
                "Cypher"
            });
        }

        public override bool CheckForUnlock(GameAction action)
        {
           
            bool condition1 = CheckCypherAugmentPlays();

            if (condition1)
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }

        private bool CheckCypherAugmentPlays()
        {
            string cypherIdentifier = "Cypher";


            IEnumerable<MoveCardJournalEntry> moveCardJournalEntries = from mcje in base.Journal.MoveCardEntries()
                                                     where IsAugment(mcje.Card) && mcje.ToLocation.IsHeroPlayAreaRecursive
                                                     select mcje;

            bool cypherFlag = moveCardJournalEntries.Distinct(new AugmentEqualityComparer()).Count(mcje => mcje.ToLocation.HighestRecursiveLocation.GetOwnerName() == cypherIdentifier) > 4;
        


            return cypherFlag;
        }

        private sealed class AugmentEqualityComparer : IEqualityComparer<MoveCardJournalEntry>
        {
            public bool Equals(MoveCardJournalEntry x, MoveCardJournalEntry y)
            {
                return x.Card == y.Card;
            }

            public int GetHashCode(MoveCardJournalEntry obj)
            {
                return (obj.Card != null ? obj.Card.GetHashCode() : 0);
            }
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
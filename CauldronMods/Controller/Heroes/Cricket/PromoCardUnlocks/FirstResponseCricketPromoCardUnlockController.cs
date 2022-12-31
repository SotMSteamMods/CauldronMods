using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Cricket
{
    public class FirstResponseCricketPromoCardUnlockController : PromoCardUnlockController
    {

        public FirstResponseCricketPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Cricket", "FirstResponseCricketCharacter")
        {

        }

        private readonly string cricketIdentifier = "CricketCharacter";
        private readonly string echelonTurnTakerIdentifier = "Echelon";

        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame("Cricket") && IsInGame("Echelon") && IsInGame("WindmillCity");
        }
       
        public override bool CheckForUnlock(GameAction action)
        {
           
            if (!IsGameOver(action))
                return IsUnlocked;

            bool condition1 = (action as GameOverAction).ResultIsVictory;
            

            if (condition1 && CheckCricketHP())
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }

        private bool CheckCricketHP()
        {
            bool flag = false;
            bool flag2 = false;
            DealDamageJournalEntry dealDamageJournalEntry = (from ddje in base.Journal.DealDamageEntries()
                                                             where ddje.TargetCard.Identifier == cricketIdentifier && ddje.TargetCardsHitPointsAfterDamage < 5
                                                             select ddje).FirstOrDefault();
            if (dealDamageJournalEntry != null)
            {
                flag = true;
                int? lessFiveHpIndex = base.Journal.GetEntryIndex(dealDamageJournalEntry);
                IEnumerable<GainHPJournalEntry> source = from ghje in base.Journal.GainHPEntries()
                                                         where ghje.SourceCard != null && ghje.SourceCard.Owner.Identifier == echelonTurnTakerIdentifier && ghje.TargetCard != null && ghje.TargetCard.Identifier == cricketIdentifier && base.Journal.GetEntryIndex(ghje) >= lessFiveHpIndex
                                                         select ghje;
             
                if (source.Select(ghje => ghje.Amount).Sum() >= 10 )
                {
                    flag2 = true;
                }
            }
            return flag && flag2;
        }

    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Quicksilver
{
    public class UncannyQuicksilverPromoCardUnlockController : PromoCardUnlockController
    {

        public UncannyQuicksilverPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Quicksilver", "UncannyQuicksilverCharacter")
        {

        }

        private bool ComboChainConditionHasBeenMet { get; set; } = false;
        private string QuicksilverCharacterIdentifier = "QuicksilverCharacter";
        private string QuicksilverTurnTakerIdentifier = "Quicksilver";
        private string HalberdExperimentalResearchCenterIdentifier = "HalberdExperimentalResearchCenter";

        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame(QuicksilverTurnTakerIdentifier, QuicksilverCharacterIdentifier) && IsInGame(HalberdExperimentalResearchCenterIdentifier);
        }

        private void CheckComboChain()
        {
            IEnumerable<PlayCardJournalEntry> playCardJournalEntries = from pcje in Journal.PlayCardEntriesThisTurn()
                                                                       where pcje.CardSource != null && IsCombo(pcje.CardSource)
                                                                       select pcje;
            if(playCardJournalEntries.Count() >= 4 && IsFinisher(playCardJournalEntries.Last().CardPlayed))
            {
                ComboChainConditionHasBeenMet = true;
            }
        }

        private bool DestroyedHalcyonCleaners()
        {
            string halcyonCleanerIdentifier = "HalcyonCleaners";
            IEnumerable<DestroyCardJournalEntry> destroyCardJournalEntries = from dcje in Journal.DestroyCardEntries()
                                                                       where dcje.Card.Identifier == halcyonCleanerIdentifier && dcje.ResponsibleCard != null && dcje.ResponsibleCard.Identifier == QuicksilverCharacterIdentifier
                                                                             select dcje;
            return destroyCardJournalEntries.Any();
        }

        public override bool CheckForUnlock(GameAction action)
        {

            if (!ComboChainConditionHasBeenMet)
            {
                CheckComboChain();
            }

            if (!IsGameOver(action))
                return false;

            bool condition2 = DestroyedHalcyonCleaners();
            bool condition3 = !IsIncapacitated(QuicksilverTurnTakerIdentifier);
            bool condition4 = IsGameOverVictory(action);

            if (ComboChainConditionHasBeenMet && condition2 && condition3 && condition4)
            {
                return true;
            }

            return false;
        }

        protected bool IsCombo(Card card)
        {
            return card != null && card.DoKeywordsContain("combo");
        }

        protected bool IsFinisher(Card card)
        {
            return card != null && card.DoKeywordsContain("finisher");
        }


    }
}
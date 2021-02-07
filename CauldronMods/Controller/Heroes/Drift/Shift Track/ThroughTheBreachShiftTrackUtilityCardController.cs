using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class ThroughTheBreachShiftTrackUtilityCardController : ShiftTrackUtilityCardController
    {
        protected ThroughTheBreachShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(1) != null, () => "The card at position 1 is " + this.GetBreachedCard(1).Title, () => "There is no card at position 1");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(2) != null, () => "The card at position 2 is " + this.GetBreachedCard(2).Title, () => "There is no card at position 2");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(3) != null, () => "The card at position 3 is " + this.GetBreachedCard(3).Title, () => "There is no card at position 3");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(4) != null, () => "The card at position 4 is " + this.GetBreachedCard(4).Title, () => "There is no card at position 4");
            Card.UnderLocation.OverrideIsInPlay = false;
        }

        protected const string BreachPosition = "BreachPosition";

        private Card GetBreachedCard(int position)
        {
            IEnumerable<CardPropertiesJournalEntry> entries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Key == BreachPosition + position && entry.BoolValue.Value);
            foreach (CardPropertiesJournalEntry entry in entries)
            {
                if (base.GameController.GetCardPropertyJournalEntryBoolean(entry.Card, BreachPosition + position) ?? false)
                {
                    return entry.Card;
                }
            }
            return null;
        }
    }
}

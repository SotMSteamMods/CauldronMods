using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class ThroughTheBreachShiftTrackUtilityCardController : ShiftTrackUtilityCardController
    {
        public ThroughTheBreachShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(1) != null, () => "The card at position 1 is " + this.GetBreachedCard(1).Title, () => "There is no card at position 1");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(2) != null, () => "The card at position 2 is " + this.GetBreachedCard(1).Title, () => "There is no card at position 2");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(3) != null, () => "The card at position 3 is " + this.GetBreachedCard(1).Title, () => "There is no card at position 3");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(4) != null, () => "The card at position 4 is " + this.GetBreachedCard(1).Title, () => "There is no card at position 4");
        }

        private Card GetBreachedCard(int position)
        {
            return base.GetCardPropertyJournalEntryCard("BreachPosition" + position);
        }
    }
}

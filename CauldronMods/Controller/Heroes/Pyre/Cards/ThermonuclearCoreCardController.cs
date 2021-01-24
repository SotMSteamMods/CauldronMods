using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ThermonuclearCoreCardController : PyreUtilityCardController
    {
        public ThermonuclearCoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddStartOfGameTriggers()
        {
            //"When this card enters your hand, select 1 non-{PyreIrradiate} card in your hand. {PyreIrradiate} that card until it leaves your hand.",
        }
        public override void AddTriggers()
        {
            //"At the end of your turn, 1 player with no {PyreIrradiate} cards in their hand draws a card. {PyreIrradiate} that card until it leaves their hand."
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class IrradiatedMarkerCardController : PyreUtilityCardController
    {
        public IrradiatedMarkerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // The check for related cards SpecialStrings is based around Cards in Play that have Game Text.
            // Since the irradiated marker is in the hand rather than in play, we can't get the special string to appear on the irradiated card
            SpecialStringMaker.ShowSpecialString(() => "You may need to right click the card this is next to and select 'Play Card' in order to play it.").Condition = () => Card.Location.IsNextToCard;
        }

    }
}

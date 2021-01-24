using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DimensionalInsinuationCardController : OutlanderUtilityCardController
    {
        public DimensionalInsinuationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, Search the villain deck for a copy of Anchored Fragment and put it into play. Shuffle the villain deck and play its top card.
        //Damage dealt by {Outlander} is irreducible. At the start of the villain turn, destroy this card.
    }
}

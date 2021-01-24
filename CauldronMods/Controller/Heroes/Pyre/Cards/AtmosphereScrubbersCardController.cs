using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class AtmosphereScrubbersCardController : PyreUtilityCardController
    {
        public AtmosphereScrubbersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, you may use a power.",
            yield break;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"2 heroes may each discard a card to regain 2 HP. Each hero who discards a {PyreIrradiate} card this way may draw a card."
            yield break;
        }
    }
}

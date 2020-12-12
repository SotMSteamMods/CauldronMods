using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class LocalMicrogravityCardController : CardController
    {
        public LocalMicrogravityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} regains 1HP.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"The first time {Impact} would be dealt damage each environment turn, prevent that damage."
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} deals 1 target 3 sonic damage. Destroy this card."
            yield break;
        }
    }
}
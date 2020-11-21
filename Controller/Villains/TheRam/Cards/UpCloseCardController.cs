using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class UpCloseCardController : TheRamUtilityCardController
    {
        public UpCloseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When this card enters play, if all active heroes are Up Close, this card is destroyed.",
            //"Play this card next to an active hero who is not Up Close. That hero and all their targets are Up Close. That hero gains:",
            //"Power: Destroy this card."
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy this card."
            yield break;
        }

    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class HandIsFasterThanTheEyeCardController : CardController
    {
        public HandIsFasterThanTheEyeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a non-relic, non-character villain card would activate start of turn or end of turn text, destroy it instead. Then, destroy this card.",
        }
    }
}
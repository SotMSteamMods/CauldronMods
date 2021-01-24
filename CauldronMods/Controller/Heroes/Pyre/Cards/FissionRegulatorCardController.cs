using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class FissionRegulatorCardController : PyreUtilityCardController
    {
        public FissionRegulatorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When Rogue Fission Cascade would enter play, instead discard it and draw a card. Then put a Cascade card from your trash on top of your deck and destroy this card.",
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Each player selects 1 non-{PyreIrradiate} card in their hand. {PyreIrradiate} those cards until they leave their hands."
            yield break;
        }
    }
}

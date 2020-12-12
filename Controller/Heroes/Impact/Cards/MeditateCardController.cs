using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class MeditateCardController : CardController
    {
        public MeditateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, draw a card.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"When {Impact} damages a target, you may destroy this card. If you do, {Impact} deals that target X infernal damage, where X is the amount of damage he just dealt."
        }
    }
}
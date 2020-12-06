using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cricket
{
    public class SonicAmplifierCardController : CardController
    {
        public SonicAmplifierCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        //Whenever {Cricket} deals sonic damage to a target, you may put the top card of your deck beneath this one. Cards beneath this one are not considered to be in play.

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard all cards beneath this one. {Cricket} deals 1 target X sonic damage, where X is the number of cards discarded this way.
            yield break;
        }
    }
}
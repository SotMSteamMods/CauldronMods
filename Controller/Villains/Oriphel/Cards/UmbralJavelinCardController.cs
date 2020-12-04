using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class GrandOriphelCardController : OriphelUtilityCardController
    {
        public GrandOriphelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {

            //"If Jade is in play, reveal the top {H} cards of the villain deck. Put any revealed Goons and Guardians into play and discard the rest.",
            //"If {Oriphel} is in play, destroy {H} hero ongoing and/or equipment cards. Play the top card of the villain deck."

            yield break;
        }
    }
}
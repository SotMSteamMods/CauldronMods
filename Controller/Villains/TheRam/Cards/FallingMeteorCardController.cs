using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class FallingMeteorCardController : TheRamUtilityCardController
    {
        public FallingMeteorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Search the villain deck and trash for {H - 2} copies of Up Close and play them next to the {H - 2} heroes with the highest HP that are not up close. If you searched the deck, shuffle it. {TheRam} deals each non-villain target {H} projectile damage."

            yield break;
        }
    }
}
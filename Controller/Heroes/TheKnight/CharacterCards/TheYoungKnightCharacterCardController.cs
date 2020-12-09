using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheYoungKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public TheYoungKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheYoungKnightCharacter} deals herself and 2 other targets 2 toxic damage each"

            yield break;
        }

        //"flippedBody": "When {TheYoungKnightCharacter} flips to this side, destroy all equipment cards next to her.",

    }
}
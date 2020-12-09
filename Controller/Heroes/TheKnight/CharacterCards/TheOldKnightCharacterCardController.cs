using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheOldKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public TheOldKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheOldKnightCharacter} deals 1 target 2 irreducible lightning damage. Return 1 of your equipment cards in play to your hand."
            yield break;
        }

        //"flippedBody": "When {TheOldKnightCharacter} flips to this side, destroy all equipment cards next to him.",

    }
}
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class CelestialAuraCardController : StarlightCardController
    {
        public CelestialAuraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever {Starlight} would deal damage to a hero target next to a constellation, instead that target regains that much HP.",
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Starlight deals 1 target 1 radiant damage. Draw a card."
            yield break;
        }
    }
}
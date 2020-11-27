using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class DowsingCrystalCardController : CardController
    {
        public DowsingCrystalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Once before your next turn when a non-hero card enters play, one hero target may deal a non-hero target 2 damage of a type of their choosing. You may destroy this card to increase that damage by 2."

            yield break;
        }
    }
}
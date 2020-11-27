using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class SmokeAndMirrorsCardController : CardController
    {
        public SmokeAndMirrorsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a hero target would be dealt damage, you may prevent that damage. If you do, destroy this card.",

        }
    }
}
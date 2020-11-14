using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class PillarsOfCreationCardController : StarlightCardController
    {
        public PillarsOfCreationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of your play phase, put a constellation card from your hand or trash into play."
        }

    }
}
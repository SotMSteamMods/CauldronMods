using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public abstract class TacticBaseCardController : CardController
    {
        public TacticBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {

            base.AddTriggers();
        }
    }
}
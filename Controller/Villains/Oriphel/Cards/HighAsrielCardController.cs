using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighAsrielCardController : OriphelGuardianCardController
    {
        public HighAsrielCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Guardian destroy tregger
            base.AddTriggers();

            //"When a hero card is played, this card deals the hero with the highest HP 2 psychic damage.",

        }
    }
}
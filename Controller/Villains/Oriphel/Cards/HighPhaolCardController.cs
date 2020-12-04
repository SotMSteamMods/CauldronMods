using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighPhaolCardController : OriphelGuardianCardController
    {
        public HighPhaolCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Guardian destroy trigger
            base.AddTriggers();

            //"The first time any hero target deals damage to any villain target each turn, this card deals that hero target 3 cold damage.",

        }
    }
}
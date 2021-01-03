using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class AncientWardCardController : CardController
    {
        public AncientWardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to Heads by 1.
            base.AddReduceDamageTrigger((Card c) => c.DoKeywordsContain("head"), 1);
        }
    }
}
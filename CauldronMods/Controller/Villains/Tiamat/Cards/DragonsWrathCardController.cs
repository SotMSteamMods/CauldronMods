using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class DragonsWrathCardController : CardController
    {
        public DragonsWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by heads targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.DoKeywordsContain("head"), (DealDamageAction dd) => 1);
        }
    }
}
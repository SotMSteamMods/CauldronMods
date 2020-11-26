using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class TortureChamberCardController : StSimeonsRoomCardController
    {
        public static readonly string Identifier = "TortureChamber";

        public TortureChamberCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by villain targets by 1.
            bool criteria(DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.IsVillainTarget;
            base.AddIncreaseDamageTrigger(criteria, (DealDamageAction dd) => 1);

            base.AddTriggers();
        }
    }
}
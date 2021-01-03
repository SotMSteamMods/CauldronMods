using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class CursedVaultCardController : StSimeonsRoomCardController
    {
        public static readonly string Identifier = "CursedVault";

        public CursedVaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override void AddTriggers()
        {
            //Reduce damage dealt to villain targets by 1.
            AddReduceDamageTrigger((Card c) => IsVillainTarget(c), 1);

            base.AddTriggers();
        }
    }
}
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class CursedVaultCardController : RoomCardController
    {
        #region Constructors

        public CursedVaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Reduce damage dealt to villain targets by 1.
            AddReduceDamageTrigger((Card c) => c.IsVillainTarget, 1);

            base.AddTriggers();
        }

        #endregion Methods
    }
}
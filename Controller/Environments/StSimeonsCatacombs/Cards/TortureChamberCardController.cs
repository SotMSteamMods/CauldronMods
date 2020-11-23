using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class TortureChamberCardController : CardController
    {
        #region Constructors

        public TortureChamberCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Increase damage dealt by villain targets by 1.
            Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.IsVillainTarget;
            base.AddIncreaseDamageTrigger(criteria, (DealDamageAction dd) => 1);
        }

        #endregion Methods
    }
}
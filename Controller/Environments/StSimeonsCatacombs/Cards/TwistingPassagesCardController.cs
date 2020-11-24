using System;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class TwistingPassagesCardController : RoomCardController
    {
        #region Constructors

        public TwistingPassagesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Increase damage dealt by environment targets by 1.
            Func<DealDamageAction, bool> envCriteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsEnvironmentTarget;
            base.AddIncreaseDamageTrigger(envCriteria, (DealDamageAction dd) => 1);

            //If there are fewer than 2 environment targets in play, increase damage dealt by hero targets by 1.
            Func<DealDamageAction, bool> heroCriteria = (DealDamageAction dd) => this.GetNumberOfEnvironmentTargetsInPlay() < 2 && dd.DamageSource != null && dd.DamageSource.Card.IsHero && dd.DamageSource.Card.IsTarget;
            base.AddIncreaseDamageTrigger(heroCriteria, (DealDamageAction dd) => 1);

            base.AddTriggers();
        }

        protected int GetNumberOfEnvironmentTargetsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsEnvironmentTarget).Count();
        }

        #endregion Methods
    }
}
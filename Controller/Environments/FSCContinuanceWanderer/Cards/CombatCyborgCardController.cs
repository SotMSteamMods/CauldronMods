using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class CombatCyborgCardController : CardController
    {
        #region Constructors

        public CombatCyborgCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Reduce damage dealt to environment targets by 2.
            base.AddReduceDamageTrigger((Card c) => c.IsEnvironmentTarget, 2);
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsNonEnvironmentTarget, TargetType.LowestHP, base.H - 2, DamageType.Projectile);
        }

        #endregion Methods
    }
}
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class CoalKidCardController : GhostCardController
    {
        #region Constructors

        public CoalKidCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new string[] { "TwistingPassages" }, true)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 2 fire damage.
            IEnumerator dealDamage = base.DealDamage(base.Card, (Card c) => c.IsTarget && c.IsHero, 2, DamageType.Fire);
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => dealDamage, TriggerType.DealDamage);
            
            //add unaffected triggers from GhostCardControllers
            base.AddTriggers();
        }
        #endregion Methods
    }
}
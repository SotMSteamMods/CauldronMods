using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class SacrificialShrineCardController : CardController
    {
        #region Constructors

        public SacrificialShrineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each target 2 psychic damage.
            IEnumerator dealDamage = base.DealDamage(base.Card, (Card c) => c.IsTarget, 2, DamageType.Psychic);
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => dealDamage, TriggerType.DealDamage);
        }

        #endregion Methods
    }
}
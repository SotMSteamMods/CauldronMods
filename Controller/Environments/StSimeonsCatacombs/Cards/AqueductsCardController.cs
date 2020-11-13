using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class AqueductsCardController : CardController
    {
        #region Constructors

        public AqueductsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //At the end of the environment turn, each target regains 1 HP
            IEnumerator allTargetsGainHP = base.GameController.GainHP(this.DecisionMaker, (Card c) => true, 1, cardSource: base.GetCardSource());
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => allTargetsGainHP, TriggerType.GainHP);
        }

        #endregion Methods
    }
}
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class AqueductsCardController : RoomCardController
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
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => this.AllTargetsGainHP(), TriggerType.GainHP);

            base.AddTriggers();
        }

        private IEnumerator AllTargetsGainHP()
        {
            IEnumerator allTargetsGainHP = base.GameController.GainHP(this.DecisionMaker, (Card c) => true, 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(allTargetsGainHP);
            }
            else
            {
                base.GameController.ExhaustCoroutine(allTargetsGainHP);
            }

            yield break;

        }

        #endregion Methods
    }
}
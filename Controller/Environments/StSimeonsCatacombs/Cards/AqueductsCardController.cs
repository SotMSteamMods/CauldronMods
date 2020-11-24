using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class AqueductsCardController : StSimeonsRoomCardController
    {
        public static readonly string Identifier = "Aqueducts";

        public AqueductsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

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
    }
}
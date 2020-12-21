using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class StrategicDeploymentCardController : EchelonBaseCardController
    {
        //==============================================================
        // Search your deck for a Tactic and put it into play. Shuffle your deck.
        //==============================================================

        public static string Identifier = "StrategicDeployment";

        public StrategicDeploymentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            LinqCardCriteria criteria = new LinqCardCriteria(this.IsTactic, "tactic");
            IEnumerator routine = base.SearchForCards(this.DecisionMaker, true, false, 1, 1, criteria, false, true, false, shuffleAfterwards: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}
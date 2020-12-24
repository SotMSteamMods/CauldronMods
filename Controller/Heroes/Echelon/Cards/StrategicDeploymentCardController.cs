using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(IsTactic, "tactic"));
        }

        public override IEnumerator Play()
        {
            LinqCardCriteria criteria = new LinqCardCriteria(this.IsTactic, "tactic");
            IEnumerator routine = base.SearchForCards(this.DecisionMaker, true, false, 1, 1, criteria, true, false, false, shuffleAfterwards: true);
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
using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class NetworkedAttackCardController : CypherBaseCardController
    {
        //==============================================================
        // Each augmented hero may use a power now.
        //==============================================================

        public static string Identifier = "NetworkedAttack";

        public NetworkedAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerable<Function> FunctionsBasedOnCard(Card c) => new Function[]
            {
                new Function(base.FindCardController(c).DecisionMaker, "Use a power", SelectionType.UsePower, 
                    () => this.UsePowerResponse(c))
            };


            IEnumerator routine = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, 
                new LinqCardCriteria(IsAugmented, "augmented heroes", false), FunctionsBasedOnCard, false, base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator UsePowerResponse(Card card)
        {
            if (card == null)
            {
                yield break;
            }

            CardController cc = base.FindCardController(card);
            IEnumerator routine = base.SelectAndUsePower(cc, true);
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
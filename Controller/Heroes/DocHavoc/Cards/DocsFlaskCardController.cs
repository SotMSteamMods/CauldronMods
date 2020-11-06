using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class DocsFlaskCardController : CardController
    {
        public static string Identifier = "DocsFlask";

        public DocsFlaskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the start of your turn, 1 hero target regains 1 HP
            this.AddStartOfTurnTrigger((Func<TurnTaker, bool>)(tt => tt == this.TurnTaker),
                (Func<PhaseChangeAction, IEnumerator>)(p =>
                    this.GameController.SelectAndGainHP(this.DecisionMaker, 1,
                        additionalCriteria: ((Func<Card, bool>) (c => c.IsHero)),
                        numberOfTargets: 1, requiredDecisions: new int?(1), cardSource: this.GetCardSource())),
                TriggerType.GainHP);

                base.AddTriggers();
        }
    }
}

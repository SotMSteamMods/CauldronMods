using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class CoalKidCardController : StSimeonsGhostCardController
    {
        public static readonly string Identifier = "CoalKid";

        public CoalKidCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new string[] { TwistingPassagesCardController.Identifier }, true)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 2 fire damage.
            IEnumerator dealDamage = base.DealDamage(base.Card, (Card c) => IsHeroTarget(c), 2, DamageType.Fire);
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => dealDamage, TriggerType.DealDamage);

            //add unaffected triggers from GhostCardControllers
            base.AddTriggers();
        }
    }
}
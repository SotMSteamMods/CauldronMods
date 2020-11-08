using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdFoamcoreCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdFoamcoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //If there are no Chemical Triggers in play, reduce damage dealt to the hero target with the lowest HP by 1.
            base.AddReduceDamageTrigger((Card c) => !base.IsChemicalTriggerInPlay() && base.CanCardBeConsideredLowestHitPoints(c,(Card card) => card.IsHero && card.IsTarget), 1);
            //Otherwise, reduce damage dealt to villain targets by 1.
            base.AddReduceDamageTrigger((Card c) => base.IsChemicalTriggerInPlay() && c.IsVillainTarget, 1);
        }
        #endregion Methods
    }
}
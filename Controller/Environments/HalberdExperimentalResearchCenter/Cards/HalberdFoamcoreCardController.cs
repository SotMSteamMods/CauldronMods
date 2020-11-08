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
            //criteria for finding hero target with the lowest hp when no chemical triggers are in play
            Func<DealDamageAction, bool> heroCriteria = (DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && base.CanCardBeConsideredLowestHitPoints(dd.Target, (Card c) => c.IsTarget && c.IsHero);

            //criteria for finding all villain targets when chemical triggers are in play
            Func<DealDamageAction, bool> villainCriteria = (DealDamageAction dd) => base.IsChemicalTriggerInPlay() && dd.Target.IsVillainTarget;

            //If there are no Chemical Triggers in play, reduce damage dealt to the hero target with the lowest HP by 1.
            base.AddReduceDamageTrigger(heroCriteria, (DealDamageAction dd) => 1);
            //Otherwise, reduce damage dealt to villain targets by 1.
            base.AddReduceDamageTrigger(villainCriteria, (DealDamageAction dd) => 1);
        }
        #endregion Methods
    }
}
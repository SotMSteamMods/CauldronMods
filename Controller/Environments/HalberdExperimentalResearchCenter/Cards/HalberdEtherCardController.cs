using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdEtherCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdEtherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //If there are no Chemical Triggers in play, increase damage dealt by the hero target with the highest HP by 1.
            //Otherwise, increase damage dealt by villain targets by 1

            Func<DealDamageAction, bool> criteria;
            //set targets based on whether there are chemical triggers in play
            if (!base.IsChemicalTriggerInPlay())
            {
                //find hero target with the highest hp
                criteria = (DealDamageAction dd) => dd.DamageSource != null && base.CanCardBeConsideredHighestHitPoints(dd.DamageSource.Card,(Card c) => c.IsTarget && c.IsHero);
            }
            else
            {
                //find all villain targets
                criteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.IsVillainTarget;
            }
            
            //increase damage dealt by hero/villain targets by 1
            base.AddIncreaseDamageTrigger(criteria, (DealDamageAction dd) => 1);
        }
        #endregion Methods
    }
}
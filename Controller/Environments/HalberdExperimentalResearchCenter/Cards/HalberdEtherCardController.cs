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

            //criteria for finding hero target with the highest hp when no chemical triggers are in play
            Func<DealDamageAction, bool> heroCriteria = (DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && dd.DamageSource != null && base.CanCardBeConsideredHighestHitPoints(dd.DamageSource.Card, (Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText);

            //criteria for finding all villain targets when chemical triggers are in play
            Func<DealDamageAction, bool> villainCriteria = (DealDamageAction dd) => base.IsChemicalTriggerInPlay() && dd.DamageSource != null && dd.DamageSource.Card.IsVillainTarget;

            //increase damage dealt by villain targets by 1 if chemical trigger is in play
            base.AddIncreaseDamageTrigger(villainCriteria, 1);

            //increase damage dealt by hero targets by 1 if no chemical trigger is in play
            base.AddIncreaseDamageTrigger(heroCriteria, 1);
        }
        #endregion Methods
    }
}
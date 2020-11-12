using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdSirenCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdSirenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            base.SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //If there are no Chemical Triggers in play, the first time the hero target with the highest HP would be dealt damage each turn, redirect that damage to the villain target with the highest HP. 
            base.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && base.CanCardBeConsideredHighestHitPoints(dd.Target, (Card c) => c.IsTarget && c.IsHero), FirstTimeHeroDamage, TargetType.HighestHP, (Card c) => c.IsVillainTarget);

            //Otherwise, the first time a non-hero target would be dealt damage each turn, redirect that damage to the hero target with the highest HP.
            base.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => base.IsChemicalTriggerInPlay() && !dd.Target.IsHero && dd.Target.IsTarget, FirstTimeNonHeroDamage, TargetType.HighestHP, (Card c) => c.IsTarget && c.IsHero);

        }

        private const string FirstTimeHeroDamage = "FirstTimeHeroDamage";
        private const string FirstTimeNonHeroDamage = "FirstTimeNonHeroDamage";
        #endregion Methods
    }
}
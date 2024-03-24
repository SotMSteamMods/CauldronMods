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
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => IsChemicalTriggerInPlay() ;
            base.SpecialStringMaker.ShowVillainTargetWithHighestHP().Condition = () => !IsChemicalTriggerInPlay();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //If there are no Chemical Triggers in play, the first time the hero target with the highest HP would be dealt damage each turn, redirect that damage to the villain target with the highest HP. 
            base.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && base.CanCardBeConsideredHighestHitPoints(dd.Target, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText), FirstTimeHeroDamage, TargetType.HighestHP, (Card c) => IsVillainTarget(c));

            //Otherwise, the first time a non-hero target would be dealt damage each turn, redirect that damage to the hero target with the highest HP.
            base.AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => base.IsChemicalTriggerInPlay() && !IsHeroTarget(dd.Target), FirstTimeNonHeroDamage, TargetType.HighestHP, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText);

        }

        private const string FirstTimeHeroDamage = "FirstTimeHeroDamage";
        private const string FirstTimeNonHeroDamage = "FirstTimeNonHeroDamage";
        #endregion Methods
    }
}
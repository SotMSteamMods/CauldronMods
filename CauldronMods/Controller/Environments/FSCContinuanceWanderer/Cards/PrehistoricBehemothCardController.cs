using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class PrehistoricBehemothCardController : CardController
    {

        public PrehistoricBehemothCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: Game.H - 2);
        }

        public override void AddTriggers()
        {
            //This card is immune to damage dealt by targets with less than 10HP.
            base.AddImmuneToDamageTrigger((DealDamageAction damageAction) => damageAction.DamageSource != null && damageAction.DamageSource.Card.HitPoints < 10 && damageAction.Target == base.Card);
            //At the end of the environment turn, this card deals the {H - 2} hero target 2 melee damage each.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, 2, DamageType.Melee, numberOfTargets: base.Game.H - 2);
        }

    }
}
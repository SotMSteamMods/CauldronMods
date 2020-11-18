using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron
{
    public class StressHardeningCardController : CardController
    {
        public StressHardeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard && !action.Target.IsHero, (DealDamageAction action) => DamageIncrease(action));
        }

        private int DamageIncrease(DealDamageAction action)
        {
            int increase = 0;
            if (base.CharacterCard.MaximumHitPoints > base.CharacterCard.HitPoints)
            {
                increase++;
            }
            if (base.CharacterCard.HitPoints <= 10)
            {
                increase++;
            }
            return increase;
        }
    }
}
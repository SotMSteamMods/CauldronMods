using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public class OutOfSyncCardController(Card card, TurnTakerController turnTakerController) : DriftBaseCardController(card, turnTakerController)
{
    public override void AddTriggers()
    {
        //{DriftPast} Reduce damage dealt to {Drift} by 1.
        base.AddReduceDamageTrigger((Card c) => this.IsTimeMatchingPast && c == base.GetActiveCharacterCard(), 1);

        //{DriftFuture} Increase damage dealt by {Drift} to other targets by 1.
        base.AddIncreaseDamageTrigger((DealDamageAction action) => this.IsTimeMatchingFuture && action.DamageSource.IsSameCard(base.GetActiveCharacterCard()) && action.Target != base.GetActiveCharacterCard(), 1);
    }
}

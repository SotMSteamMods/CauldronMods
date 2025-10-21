using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class ShadedOwlCardController : CardController
    {
        //==============================================================
        // Increase damage dealt by villain targets by 1.
        // At the end of the villain turn, this card deals each
        // hero target 1 sonic damage.
        //==============================================================

        public static readonly string Identifier = "ShadedOwl";

        private const int DamageAmountToIncrease = 1;
        private const int DamageToDeal = 1;

        public ShadedOwlCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Increase damage dealt by villain targets by 1.
            base.AddIncreaseDamageTrigger(action => action.DamageSource != null && action.DamageSource.IsVillainTarget, DamageAmountToIncrease);

            // At the end of the villain turn, this card deals each hero target 1 sonic damage.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnDealDamageResponse(PhaseChangeAction pca)
        {
            // At the end of the villain turn, this card deals each hero target 1 sonic damage.
            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, card => IsHeroTarget(card) && card.IsInPlay, DamageToDeal, DamageType.Sonic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}

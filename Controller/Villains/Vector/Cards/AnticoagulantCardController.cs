using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class AnticoagulantCardController : CardController
    {
        //==============================================================
        // Increase damage dealt to {Vector} by 1.
        // When {Vector} is dealt damage, destroy this card and {Vector}
        // deals each hero target X toxic damage, where X is the amount
        // of damage that was dealt to {Vector}.
        //==============================================================

        public static readonly string Identifier = "Anticoagulant";

        private const int IncreaseDamageAmount = 1;

        public AnticoagulantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {

            AddTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard, DealDamageResponse,
                TriggerType.DealDamage, timing: TriggerTiming.After);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Increase damage dealt to {Vector} by 1.
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(IncreaseDamageAmount);
            idse.TargetCriteria.IsSpecificCard = base.CharacterCard;
            idse.UntilCardLeavesPlay(this.Card);

            IEnumerator coroutine = base.AddStatusEffect(idse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageResponse(DealDamageAction dda)
        {
            // When {Vector} is dealt damage, destroy this card and {Vector}
            // deals each hero target X toxic damage, where X is the amount
            // of damage that was dealt to {Vector}.

            IEnumerator damageRoutine = base.GameController.DealDamage(base.DecisionMaker, base.CharacterCard, 
                card => card.IsHero && card.IsInPlay,
                dda.Amount, DamageType.Toxic, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
            }

            IEnumerator destroyRoutine = base.GameController.DestroyCard(this.DecisionMaker, this.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
            }
        }
    }
}
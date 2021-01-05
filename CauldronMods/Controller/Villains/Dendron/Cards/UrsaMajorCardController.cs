using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class UrsaMajorCardController : CardController
    {
        //==============================================================
        // Reduce damage dealt to this card by 1.
        // At the end of the villain turn, this card deals the hero
        // target with the highest HP 2 melee damage.
        //==============================================================

        public static readonly string Identifier = "UrsaMajor";

        private const int DamageAmountToReduce = 1;
        private const int DamageToDeal = 2;

        public UrsaMajorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            base.AddReduceDamageTrigger(c => c == this.Card, DamageAmountToReduce);

            this.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            List<Card> storedCardResults = new List<Card>();
            IEnumerator findTargetWithHighestHpRoutine = this.GameController.FindTargetWithHighestHitPoints(1, card => card.IsHero && !card.IsIncapacitatedOrOutOfGame, storedCardResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetWithHighestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetWithHighestHpRoutine);
            }

            if (!storedCardResults.Any())
            {
                yield break;
            }

            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, storedCardResults.First(), DamageToDeal, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }

            yield break;
        }
    }
}
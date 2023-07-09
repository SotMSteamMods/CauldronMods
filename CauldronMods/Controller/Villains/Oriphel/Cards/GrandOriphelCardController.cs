using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class GrandOriphelCardController : OriphelUtilityCardController
    {
        public GrandOriphelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of the villain turn, destroy this card unless it entered play this turn.",
            AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf, DidNotEnterPlayThisTurn);

            //"Increase damage dealt by {Oriphel} by 1. Reduce damage dealt to {Oriphel} by 1.",
            AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource != null && oriphelIfInPlay != null && dda.DamageSource.IsSameCard(oriphelIfInPlay), 1);
            AddReduceDamageTrigger((Card c) => c != null && c == oriphelIfInPlay, 1);

            //"At the end of the villain turn, {Oriphel} deals each hero target 1 infernal damage and 1 projectile damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, OriphelDealsDamage, TriggerType.DealDamage, (pca) => oriphelIfInPlay != null);
        }

        private IEnumerator OriphelDealsDamage(PhaseChangeAction pca)
        {
            var damageDetails = new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, oriphelIfInPlay), null, 1, DamageType.Infernal),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, oriphelIfInPlay), null, 1, DamageType.Projectile)

            };
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damageDetails, (Card c) => c.IsTarget && IsHero(c) && oriphelIfInPlay != null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private bool DidNotEnterPlayThisTurn(PhaseChangeAction pca)
        {
            if (Journal.CardEntersPlayEntriesThisTurn().Any((CardEntersPlayJournalEntry entry) => entry.Card == this.Card))
            {
                return false;
            }
            return true;
        }
    }
}
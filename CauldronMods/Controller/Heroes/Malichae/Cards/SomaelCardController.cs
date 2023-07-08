using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class SomaelCardController : DjinnTargetCardController
    {
        public SomaelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == this.TurnTaker, EndOfTurnReponse, TriggerType.CreateStatusEffect);
            base.AddImmuneToDamageTrigger(dda => dda.DamageType == DamageType.Projectile && dda.Target == Card);
        }

        private IEnumerator EndOfTurnReponse(PhaseChangeAction pca)
        {
            List<SelectCardDecision> storedResult = new List<SelectCardDecision>();
            var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.ReduceDamageTaken, new LinqCardCriteria(c => IsHeroTarget(c) && c.IsInPlayAndHasGameText, "hero target"), storedResult, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCard(storedResult))
            {
                var card = GetSelectedCard(storedResult);
                ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(1);
                effect.TargetCriteria.IsSpecificCard = card;
                effect.UntilStartOfNextTurn(DecisionMaker.TurnTaker);
                effect.UntilTargetLeavesPlay(card);
                effect.CardSource = Card;

                coroutine = AddStatusEffect(effect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            yield break;
        }
    }
}

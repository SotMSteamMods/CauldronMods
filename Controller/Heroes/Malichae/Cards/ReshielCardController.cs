using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class ReshielCardController : DjinnTargetCardController
	{
		public ReshielCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == this.TurnTaker, EndOfTurnReponse, TriggerType.DealDamage);
            base.AddImmuneToDamageTrigger(dda => dda.DamageType == DamageType.Sonic && dda.Target == Card);
        }

        private IEnumerator EndOfTurnReponse(PhaseChangeAction pca)
        {
            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 1, DamageType.Sonic, 2, false, 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

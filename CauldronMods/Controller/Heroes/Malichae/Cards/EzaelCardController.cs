using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class EzaelCardController : DjinnTargetCardController
	{
		public EzaelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == this.TurnTaker, EndOfTurnReponse, TriggerType.GainHP);
            base.AddImmuneToDamageTrigger(dda => dda.DamageType == DamageType.Radiant && dda.Target == Card);
        }

        private IEnumerator EndOfTurnReponse(PhaseChangeAction pca)
        {

            var coroutine = GameController.SelectAndGainHP(this.DecisionMaker, 2,
                additionalCriteria: c => IsHeroTarget(c) && c.IsInPlayAndHasGameText,
                cardSource: GetCardSource());
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

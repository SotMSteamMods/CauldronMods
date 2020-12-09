using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class ResidualDesynchronizationCardController : PhaseCardController
    {
        public ResidualDesynchronizationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), () => "A villain target has been dealt damage this turn.", () => "A villain target has not been dealt damage this turn.");
        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //Reduce damage dealt to Obstacles by 1.
            base.AddReduceDamageTrigger((Card c) => base.IsObstacle(c), 1);
            //The first time a villain target is dealt damage each turn, it deals the source of that damage 2 energy damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt") && action.Target.IsVillainTarget, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            ////The first time a villain target is dealt damage each turn...
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt");
            //...it deals the source of that damage 2 energy damage.
            IEnumerator coroutine = base.DealDamage(action.Target, action.DamageSource.Card, 2, DamageType.Energy, cardSource: base.GetCardSource());
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
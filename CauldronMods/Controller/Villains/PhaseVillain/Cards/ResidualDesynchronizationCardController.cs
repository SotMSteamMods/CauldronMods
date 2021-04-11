using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class ResidualDesynchronizationCardController : PhaseVillainCardController
    {
        public ResidualDesynchronizationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(FirstTimeDamageDealt),
                () => "A villain target has been dealt damage this turn.",
                () => "A villain target has not been dealt damage this turn.");
            ss.Condition = () => Card.IsInPlay;
        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //Reduce damage dealt to Obstacles by 1.
            base.AddReduceDamageTrigger((Card c) => base.IsObstacle(c), 1);
            //The first time a villain target is dealt damage each turn, it deals the source of that damage 2 energy damage.
            //per @Tosx, this is once per turn, NOT once per turn per target.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DidDealDamage && !base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt") && base.IsVillainTarget(action.Target) && !action.Target.IsBeingDestroyed,
                    this.DealDamageResponse,
                    TriggerType.DealDamage,
                    TriggerTiming.After,
                    actionType: ActionDescription.DamageTaken);
            
            base.AddAfterLeavesPlayAction(() => ResetFlagAfterLeavesPlay(FirstTimeDamageDealt));
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            ////The first time a villain target is dealt damage each turn...
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
            //...it deals the source of that damage 2 energy damage.
            IEnumerator coroutine = base.DealDamage(action.Target, action.DamageSource.Card, 2, DamageType.Energy, cardSource: base.GetCardSource(), isCounterDamage: true);
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
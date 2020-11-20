using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class ComboChainCardController : CardController
    {
        public ComboChainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //The first time each turn that {Quicksilver} would deal herself damage to play a Combo card, prevent that damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt") && base.CharacterCardController.IsPropertyTrue("ComboSelfDamage"), (DealDamageAction action) => this.PreventDamageResponse(action), TriggerType.CancelAction, TriggerTiming.Before, isActionOptional: false);
        }

        private IEnumerator PreventDamageResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt");
            IEnumerator coroutine = base.CancelAction(action, isPreventEffect: true);
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
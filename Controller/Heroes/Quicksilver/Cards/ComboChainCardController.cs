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
            base.AddTrigger(base.AddTrigger<ComboDamageAction>((ComboDamageAction action) => !base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), (ComboDamageAction action) => this.PreventDamageResponse(action), TriggerType.CancelAction, TriggerTiming.Before));
        }

        private IEnumerator PreventDamageResponse(ComboDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt", null);
            IEnumerator coroutine = base.CancelAction(action);
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
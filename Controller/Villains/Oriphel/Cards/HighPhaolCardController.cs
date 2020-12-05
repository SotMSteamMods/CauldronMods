using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighPhaolCardController : OriphelGuardianCardController
    {
        private const string phaolKey = "PhaolRetaliationUsed";
        public HighPhaolCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Guardian destroy trigger
            base.AddTriggers();

            //"The first time any hero target deals damage to any villain target each turn, this card deals that hero target 3 cold damage.",
            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource != null && dd.DamageSource.Card.IsHero && dd.DamageSource.Card.IsTarget && dd.Target.IsVillain && RetaliationAvailable(),
                            DealRetaliationDamage,
                            TriggerType.DealDamage,
                            TriggerTiming.After);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(phaolKey), TriggerType.Hidden);
        }

        private bool RetaliationAvailable()
        {
            return !IsPropertyTrue(phaolKey);
        }

        private IEnumerator DealRetaliationDamage(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(phaolKey);
            IEnumerator coroutine = DealDamage(this.Card, dd.CardSource.Card, 3, DamageType.Cold);
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
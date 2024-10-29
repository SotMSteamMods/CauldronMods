using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Oriphel
{
    public class HighPhaolCardController : OriphelGuardianCardController
    {
        private const string phaolKey = "PhaolRetaliationUsedThisTurn";
        public HighPhaolCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} has used its retaliation damage this turn").Condition = () => !RetaliationAvailable();
        }

        public override void AddTriggers()
        {
            //Guardian destroy trigger
            base.AddTriggers();

            //"The first time any hero target deals damage to any villain target each turn, this card deals that hero target 3 cold damage.",

            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource != null && dd.DamageSource.IsHeroTarget && IsVillainTarget(dd.Target) && RetaliationAvailable(),
                            DealRetaliationDamage,
                            TriggerType.DealDamage,
                            TriggerTiming.After,
                            ActionDescription.DamageTaken);

            base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay(phaolKey), TriggerType.Hidden);
        }

        private bool RetaliationAvailable()
        {
            return !Journal.CardPropertiesEntriesThisTurn(Card).Any(j => j.Key == phaolKey && j.BoolValue == true);
        }

        private IEnumerator DealRetaliationDamage(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(phaolKey);
            IEnumerator coroutine = DealDamage(this.Card, dd.DamageSource.Card, 3, DamageType.Cold);
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

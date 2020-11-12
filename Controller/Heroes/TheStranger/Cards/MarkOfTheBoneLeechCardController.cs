using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfTheBoneLeechCardController : RuneCardController
    {
        #region Constructors

        public MarkOfTheBoneLeechCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && !c.IsIncapacitatedOrOutOfGame, "hero targets", false, false, null, null, false))
        {
            base.SpecialStringMaker.ShowHasBeenUsedThisTurn("FirstTimeDealingDamage", null, null, null);
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //The first time that target deals damage each turn, it regains 1HP.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource != null && dd.DamageSource.Card ==  base.GetCardThisCardIsNextTo(true) && !base.IsPropertyTrue("FirstTimeDealingDamage", null), new Func<DealDamageAction, IEnumerator>(this.GainHpResponse), TriggerType.GainHP, TriggerTiming.After, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);
        }

        private IEnumerator GainHpResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDealingDamage", null);
            //it regains 1HP.
            IEnumerator coroutine = base.GameController.GainHP(base.GetCardThisCardIsNextTo(true), new int?(1), null, null, base.GetCardSource(null));
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

        private const string FirstTimeDealingDamage = "FirstTimeDealingDamage";

        #endregion Methods
    }
}
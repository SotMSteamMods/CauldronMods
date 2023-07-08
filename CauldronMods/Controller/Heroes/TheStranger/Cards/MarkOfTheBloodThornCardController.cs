using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.TheStranger
{
    public class MarkOfTheBloodThornCardController : RuneCardController
    {
        #region Constructors

        public MarkOfTheBloodThornCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeWouldBeDealtDamage, null, null, null);
        }

        #endregion Constructors

        #region Properties
        public override LinqCardCriteria NextToCardCriteria => new LinqCardCriteria((Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, "hero targets", useCardsSuffix: false);
        #endregion

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //Play this next to a hero target. The first time that target is dealt damage each turn, it deals 1 target 1 toxic damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.GetCardThisCardIsNextTo(true) && !base.IsPropertyTrue(FirstTimeWouldBeDealtDamage), new Func<DealDamageAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, TriggerTiming.After);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeWouldBeDealtDamage), TriggerType.Hidden);
        }

        private IEnumerator DealDamageResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage, null);
            //it deals 1 target 1 toxic damage.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.FindHeroTurnTakerController(base.GetCardThisCardIsNextTo(true).Owner.ToHero()), new DamageSource(base.GameController, base.GetCardThisCardIsNextTo(true)), 1, DamageType.Toxic, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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

        private const string FirstTimeWouldBeDealtDamage = "FirstTimeWouldBeDealtDamage";
        #endregion Methods
    }
}
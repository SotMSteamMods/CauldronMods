using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfTheFadedCardController : RuneCardController
    {
        #region Constructors

        public MarkOfTheFadedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties
        public override LinqCardCriteria NextToCardCriteria => new LinqCardCriteria((Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, "hero targets", useCardsSuffix: false);
        #endregion

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //Play this next to a hero target. When that target would be dealt damage by a non-hero card, you may redirect that damage to a hero with higher HP.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.GetCardThisCardIsNextTo(true) && dd.DamageSource != null && dd.DamageSource.Card != null && !IsHeroTarget(dd.DamageSource.Card), this.RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dealDamage)
        {
            //you may redirect that damage to a hero with higher HP.

            //select hero with a higher hp
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.RedirectDamage, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && !c.IsOffToTheSide && c.HitPoints.Value > base.GetCardThisCardIsNextTo(true).HitPoints, "a hero with higher HP"), storedResults, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            if (storedResults.Any())
            {
                //redirect the damage to them
                Card newTarget = storedResults.First().SelectedCard;
                IEnumerator coroutine3 = base.GameController.RedirectDamage(dealDamage, newTarget, false, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine3);
                }

            }
            yield break;
        }
        #endregion Methods
    }
}
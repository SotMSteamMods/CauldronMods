using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class MarkOfDestructionCardController : RuneCardController
    {
        #region Constructors

        public MarkOfDestructionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new LinqCardCriteria((Card c) => !c.IsHeroCharacterCard && !c.IsVillainCharacterCard && c.IsInPlay && !c.IsOneShot, "non-Character card", false, false, null, null, false))
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //purposely don't call base.AddTrigger as this card should not have the destroy or deal damage effect

            //If either card is destroyed, destroy the other. 
            base.AddWhenDestroyedTrigger((DestroyCardAction destroy) => base.GameController.DestroyCard(this.DecisionMaker, base.GetCardThisCardIsNextTo(true), cardSource: GetCardSource()), TriggerType.DestroyCard);
            base.AddTrigger<DestroyCardAction>((DestroyCardAction destroy) => destroy.CardToDestroy != null && destroy.CardToDestroy.Card == base.GetCardThisCardIsNextTo(true), (DestroyCardAction destroy) => base.DestroyThisCardResponse(destroy), TriggerType.DestroySelf, TriggerTiming.After);
            //Redirect damage dealt to this card by non-hero targets to the hero target with the highest HP.
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.Card && dd.DamageSource != null && dd.DamageSource.Card != null && !dd.DamageSource.Card.IsHero && dd.DamageSource.Card.IsTarget, this.RedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
        }

        private IEnumerator RedirectResponse(DealDamageAction dd)
        {
            //find the hero target with the highest hp
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsHero && c.IsTarget, storedResults, null, null, false, false, null, false, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResults.FirstOrDefault();
            if (card != null)
            {
                //redirect damage to that target
                coroutine = base.GameController.RedirectDamage(dd, card, false, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
        #endregion Methods
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Gray
{
    public class UnwittingHenchmenCardController : GrayCardController
    {
        public UnwittingHenchmenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, this.DestroyEquipmentResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DealDamage, TriggerType.GainHP });
        }

        private IEnumerator DestroyEquipmentResponse(PhaseChangeAction action)
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            //At the end of the villain turn, destroy 1 equipment card.
            IEnumerator coroutine;

            coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsEquipment(c), "equipment"), false, storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If a card is destroyed this way 
            DestroyCardAction destroyAction = storedResults.FirstOrDefault<DestroyCardAction>();
            if(destroyAction != null && destroyAction.WasCardDestroyed)
            {
                //...{Gray} regains 3 HP...
                coroutine = base.GameController.GainHP(base.CharacterCard, new int?(3), cardSource: base.GetCardSource());  
            }
            else
            {
                //...Otherwise this card deals the hero target with the highest HP 1 melee damage.
                coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHero(c), (Card c) => 1, DamageType.Melee);
            }
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
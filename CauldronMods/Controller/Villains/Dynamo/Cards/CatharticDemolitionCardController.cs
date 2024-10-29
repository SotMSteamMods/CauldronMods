using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class CatharticDemolitionCardController : DynamoUtilityCardController
    {
        public CatharticDemolitionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsDestroyedThisTurnEx(new LinqCardCriteria(c => IsVillain(c), "villain"));
        }

        public override void AddTriggers()
        {
            //At the start of the villain turn, destroy all Plot cards and this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyCardsResponse, new TriggerType[] { TriggerType.DestroySelf, TriggerType.DestroyCard });

            //When this card is destroyed, {Dynamo} deals each non-villain target X energy damage, where X is 2 times the number of villain cards destroyed this turn.
            base.AddWhenDestroyedTrigger(this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DestroyCardsResponse(PhaseChangeAction action)
        {
            //...destroy all Plot cards...
            IEnumerator coroutine = base.GameController.DestroyCards(base.DecisionMaker, new LinqCardCriteria((Card c) => base.IsPlot(c)));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and this card.
            coroutine = base.DestroyThisCardResponse(action);
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

        private IEnumerator DealDamageResponse(DestroyCardAction action)
        {
            //...{Dynamo} deals each non-villain target X energy damage, where X is 2 times the number of villain cards destroyed this turn.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => !base.IsVillainTarget(c), (Card c) => new int?(base.Journal.DestroyCardEntriesThisTurn().Where((DestroyCardJournalEntry entry) => base.IsVillain(entry.Card)).Count() * 2) ?? default, DamageType.Energy);
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

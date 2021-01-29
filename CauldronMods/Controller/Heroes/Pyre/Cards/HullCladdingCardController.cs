using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class HullCladdingCardController : PyreUtilityCardController
    {
        public HullCladdingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to and by {Pyre} by 1.",
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(CharacterCard), dd => 1);

            //"If Containment Breach is ever in play, destroy it or destroy this card."
            AddTrigger((CardEntersPlayAction cep) => GameController.GetAllCards().Any((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "ContainmentBreach" && GameController.IsCardVisibleToCardSource(c, GetCardSource())), DestroyCladdingOrBreachResponse, TriggerType.DestroyCard, TriggerTiming.After);
        }

        private IEnumerator DestroyCladdingOrBreachResponse(CardEntersPlayAction cep)
        {
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c == Card || c.Identifier == "ContainmentBreach"), false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

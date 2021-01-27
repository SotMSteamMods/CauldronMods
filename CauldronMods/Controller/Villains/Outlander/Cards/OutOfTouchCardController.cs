using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class OutOfTouchCardController : OutlanderUtilityCardController
    {
        public OutOfTouchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, {Outlander} deals the non-villain target with the highest HP X+3 melee damage, where X is the number of Trace cards in play.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => !base.IsVillain(c), (Card c) => base.FindCardsWhere(new LinqCardCriteria((Card card) => base.IsTrace(card) && card.IsInPlayAndHasGameText)).Count() + 3, DamageType.Melee);
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

        public override void AddTriggers()
        {
            //Reduce all HP recovery by 1.
            base.AddTrigger<GainHPAction>((GainHPAction action) => action.CardSource != null, (GainHPAction action) => base.GameController.ReduceHPGain(action, 1, cardSource: base.GetCardSource()), TriggerType.IncreaseHPGain, TriggerTiming.Before);

            //At the start of the villain turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class AngryLethovoreCardController : CardController
    {
        public AngryLethovoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, play the top card of the villain deck.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard);
            //Whenever a target enters play, this card deals that target 2 melee damage and regains 6HP.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => action.CardEnteringPlay != Card && action.CardEnteringPlay.IsTarget && action.IsSuccessful, this.DealDamageAndHealResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.GainHP }, TriggerTiming.After);
        }

        private IEnumerator DealDamageAndHealResponse(CardEntersPlayAction action)
        {
            //...this card deals that target 2 melee damage...
            IEnumerator coroutine = base.DealDamage(base.Card, action.CardEnteringPlay, 2, DamageType.Melee, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and regains 6HP.
            coroutine = base.GameController.GainHP(base.Card, 6, cardSource: base.GetCardSource());
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
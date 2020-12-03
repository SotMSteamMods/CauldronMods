using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class CombatPragmatismCardController : CardController
    {
        public CombatPragmatismCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //When a non-hero card enters play, you may destroy this card...
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !action.CardEnteringPlay.IsHero, this.DestroySelfResponse, TriggerType.DestroySelf, TriggerTiming.After, isActionOptional: true);
            //...If you do, you may use a power now.
            base.AddWhenDestroyedTrigger(this.OnDestroyResponse, TriggerType.UsePower);
        }

        private IEnumerator DestroySelfResponse(CardEntersPlayAction action)
        {
            IEnumerator coroutine = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, true, cardSource: base.GetCardSource());
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

        private IEnumerator OnDestroyResponse(DestroyCardAction action)
        {
            //you may use a power now.
            IEnumerator coroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            int hpNumeral = base.GetPowerNumeral(0, 3);
            //{Titan} regains 3HP.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, hpNumeral, cardSource: base.GetCardSource());
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
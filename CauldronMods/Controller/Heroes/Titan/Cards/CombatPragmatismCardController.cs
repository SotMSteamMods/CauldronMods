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
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => action.IsSuccessful && !IsHero(action.CardEnteringPlay), DestroySelfResponse, TriggerType.DestroySelf, TriggerTiming.After, isActionOptional: true);
        }

        private IEnumerator DestroySelfResponse(CardEntersPlayAction action)
        {
            //...If you do, you may use a power now.
            IEnumerator coroutine = base.GameController.DestroyCard(DecisionMaker, Card,
                            optional: true,
                            actionSource: action,
                            postDestroyAction: () => OnDestroyResponse(),
                            cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator OnDestroyResponse()
        {
            //you may use a power now.
            IEnumerator coroutine = base.GameController.SelectAndUsePower(DecisionMaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
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
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class MeditateCardController : CardController
    {
        public MeditateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, draw a card.",
            IEnumerator coroutine = DrawCard();
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
            //"When {Impact} damages a target, you may destroy this card. If you do, {Impact} deals that target X infernal damage, where X is the amount of damage he just dealt."
            AddTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.CharacterCard) && dd.Amount > 0 && !this.IsBeingDestroyed && !AskIfCardIsIndestructible(this.Card), MaybeRepeatDamageResponse, TriggerType.DealDamage, TriggerTiming.After, isActionOptional: true);
        }

        private IEnumerator MaybeRepeatDamageResponse(DealDamageAction dd)
        {
            var storedYesNo = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroySelf, this.Card, storedResults: storedYesNo, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if(DidPlayerAnswerYes(storedYesNo))
            {
                var destroyTrigger = AddWhenDestroyedTrigger(dca => RepeatDamageResponse(dd, dca), TriggerType.DealDamage);
                coroutine = GameController.DestroyCard(DecisionMaker, this.Card, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                RemoveTrigger(destroyTrigger);
            }
            yield break;
        }

        private IEnumerator RepeatDamageResponse(DealDamageAction dd, DestroyCardAction dca)
        {
            if(dca.CardSource == null || dca.CardSource.Card != this.Card)
            {
                yield break;
            }
            IEnumerator coroutine = DealDamage(dd.DamageSource.Card, dd.Target, dd.Amount, DamageType.Infernal, cardSource: GetCardSource());
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class OnARollCardController : GyrosaurUtilityCardController
    {
        public OnARollCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowCrashInHandCount();
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, draw a card. 
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, _ => DrawCard(), TriggerType.DrawCard);
            //Then if you have at least 2 Crash cards in your hand, {Gyrosaur} deals each non-hero target 1 melee damage and this card is destroyed."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, CheckCrashInHandResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });
        }

        private IEnumerator CheckCrashInHandResponse(PhaseChangeAction pc)
        {
            //Then if you have at least 2 Crash cards in your hand...
            var storedModifier = new List<int>();
            Func<bool> showDecisionIf = delegate
            {
                int trueCrash = TrueCrashInHand;
                return trueCrash == 1 || trueCrash == 2;
            };
            IEnumerator coroutine = EvaluateCrashInHand(storedModifier, showDecisionIf);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int crashMod = storedModifier.FirstOrDefault();

            if(TrueCrashInHand + crashMod >= 2)
            {
                coroutine = GameController.SendMessageAction($"{Card.Title} goes out of control!", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...{Gyrosaur} deals each non-hero target 1 melee damage...
                coroutine = DealDamage(CharacterCard, (Card c) => !IsHero(c), 1, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...and this card is destroyed.
                coroutine = DestroyThisCardResponse(pc);
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
    }
}

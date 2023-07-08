using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class TerrifyingMomentumCardController : GyrosaurUtilityCardController
    {
        public TerrifyingMomentumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowCrashInHandCount(true);
        }

        public override IEnumerator Play()
        {
            RemoveTemporaryTriggers();
            //Where X is the number of Crash cards in your hand:
            var storedModifier = new List<int>();
            IEnumerator coroutine = EvaluateCrashInHand(storedModifier);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            var crashMod = storedModifier.FirstOrDefault();

            //"If X is more than 4, redirect this damage to the non-hero target with the lowest HP.",
            if(TrueCrashInHand + crashMod > 4)
            {
                AddToTemporaryTriggerList(AddTrigger((DealDamageAction dd) => dd.CardSource.Card == Card, dd => RedirectDamage(dd, TargetType.LowestHP, (Card c) => !IsHero(c)), TriggerType.RedirectDamage, TriggerTiming.Before));
            }

            //"{Gyrosaur} deals 1 target X+2 melee damage",
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), (Card c) => TrueCrashInHand + crashMod + 2, DamageType.Melee, () => 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"Draw a card."
            coroutine = DrawCard();
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

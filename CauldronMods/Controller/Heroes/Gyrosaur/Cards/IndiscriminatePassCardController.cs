using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class IndiscriminatePassCardController : GyrosaurUtilityCardController
    {
        public IndiscriminatePassCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowCrashInHandCount(true);
        }

        public override IEnumerator Play()
        {
            //"If you have at least 1 Crash card in your hand...",
            var storedModifier = new List<int>();
            IEnumerator coroutine = EvaluateCrashInHand(storedModifier, () => TrueCrashInHand <= 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int crashMod = storedModifier.FirstOrDefault();
            if(TrueCrashInHand + crashMod > 0)
            {
                //...{Gyrosaur} deals another hero target 2 melee damage.
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => IsHero(c) && c != CharacterCard, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"{Gyrosaur} deals 1 non-hero target 4 melee damage."
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 4, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !IsHero(c), cardSource: GetCardSource());
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

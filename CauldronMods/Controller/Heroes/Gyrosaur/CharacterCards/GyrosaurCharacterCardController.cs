using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Gyrosaur
{
    public class GyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public GyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"If you have at least 2 crash cards in your hand, {Gyrosaur} deals up to 3 targets 1 melee damage each. If not, draw a card."
            int numCrashThreshold = GetPowerNumeral(0, 2);
            int numTargets = GetPowerNumeral(1, 3);
            int numDamage = GetPowerNumeral(2, 1);

            Func<bool> showDecisionIf = delegate
            {
                if(TrueCrashInHand == numCrashThreshold || TrueCrashInHand == numCrashThreshold - 1)
                {
                    return true;
                }
                return false;
            };

            //"If you have at least 2 crash cards in your hand...
            var storedModifier = new List<int>();
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
            if(TrueCrashInHand + crashMod >= numCrashThreshold)
            {
                //...{Gyrosaur} deals up to 3 targets 1 melee damage each.
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), numDamage, DamageType.Melee, numTargets, false, 0, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //If not, draw a card.
                coroutine = DrawCard();
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        break;
                    }
                case 1:
                    {
                        //"One target with more than 10 HP deals itself 3 melee damage.",
                        break;
                    }
                case 2:
                    {
                        //"Select a non-character target. Increase damage dealt to that target by 1 until the start of your turn."
                        break;
                    }
            }
            yield break;
        }
    }
}

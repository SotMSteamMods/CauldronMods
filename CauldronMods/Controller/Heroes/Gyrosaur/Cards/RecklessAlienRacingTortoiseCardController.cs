using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Gyrosaur
{
    public class RecklessAlienRacingTortoiseCardController : GyrosaurUtilityCardController
    {
        private int? OverrideCrashCount = null;
        private const string RARTUsedKey = "RecklessAlienRacingTortoiseUsedKey";
        public RecklessAlienRacingTortoiseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowCrashInHandCount();
        }

        public override void AddTriggers()
        {
            //"During your turn, the first time you have more than 3 Crash cards in your hand, immediately use this card's power and then destroy it.",
            AddTrigger((MakeDecisionAction md) => Game.ActiveTurnTaker == TurnTaker && md.Decision is SelectWordDecision sw && (IsCrashEvaluation(sw) && CrashCountEvaluation(sw) > 3) && !HasBeenSetToTrueThisTurn(RARTUsedKey), ExcessCrashEvaluationResponse, new TriggerType[] { TriggerType.UsePower, TriggerType.DestroySelf }, TriggerTiming.After);

            AddTrigger((PhaseChangeAction pc) => pc.ToPhase.TurnTaker == TurnTaker && !HasBeenSetToTrueThisTurn(RARTUsedKey), EvaluateCrashForSelf, new TriggerType[] { TriggerType.UsePower, TriggerType.DestroySelf }, TriggerTiming.After);
            AddTrigger((GameAction ga) => Game.ActiveTurnTaker == TurnTaker && IsCrashInHandIncreaser(ga) && !HasBeenSetToTrueThisTurn(RARTUsedKey), EvaluateCrashForSelf, new TriggerType[] { TriggerType.UsePower, TriggerType.DestroySelf }, TriggerTiming.After);

            //This trigger is to prevent autodraws while this card is out
            AddTrigger((DrawCardAction dc) => dc.HeroTurnTaker == HeroTurnTaker, _ => DoNothing(), TriggerType.DestroySelf, TriggerTiming.After);
        }
        private IEnumerator EvaluateCrashForSelf(GameAction ga)
        {
            IEnumerator coroutine;
            var noChoiceMade = true;
            if(CanActivateEffect(DecisionMaker, StabilizerKey))
            {
                var storedModifier = new List<int>();
                var didSkip = new List<bool>();
                coroutine = EvaluateCrashInHand(storedModifier, RARTShowDecision, didSkip);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                noChoiceMade = didSkip.Any();
                if(noChoiceMade && TrueCrashInHand > 3)
                {
                    OverrideCrashCount = TrueCrashInHand;
                }
            }
            if(noChoiceMade && TrueCrashInHand > 3 && this.Card.IsInPlayAndHasGameText)
            {
                coroutine = ExcessCrashEvaluationResponse(ga);
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
        private bool IsCrashEvaluation(SelectWordDecision sw)
        {
            if(sw.CardSource.Card.Owner == TurnTaker && sw.AssociatedCards.Any((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "GyroStabilizer"))
            {
                return sw.Choices.Any(s => s.Contains("crash card"));
            }
            return false;
        }
        private bool IsCrashInHandIncreaser(GameAction ga)
        {
            if(ga is MoveCardAction mc)
            {
                return mc.Destination == HeroTurnTaker.Hand && IsCrash(mc.CardToMove);
            }
            if(ga is DrawCardAction dc)
            {
                return dc.HeroTurnTaker == HeroTurnTaker && IsCrash(dc.DrawnCard);
            }
            return false;
        }
        private int CrashCountEvaluation(SelectWordDecision sw)
        {
            return TrueCrashInHand + CrashModifierFromDecision(sw);
        }
        private IEnumerator ExcessCrashEvaluationResponse(GameAction ga)
        {
            SetCardPropertyToTrueIfRealAction(RARTUsedKey);

            IEnumerator coroutine;
            int detectedCrashCount;
            //Immediately use this card's power and then destroy it.
            if(ga is MakeDecisionAction md && md.Decision is SelectWordDecision sw)
            {
                detectedCrashCount = CrashCountEvaluation(sw);
                OverrideCrashCount = detectedCrashCount;
            }
            else
            {
                detectedCrashCount = TrueCrashInHand;
            }

            coroutine = GameController.SendMessageAction($"With {detectedCrashCount} crash cards in hand, {Card.Title} goes out of control!", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = UsePowerOnOtherCard(this.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(ga);
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
            //"{Gyrosaur} deals 1 target X+1 melee damage, where X is the number of Crash cards in your hand."
            int numTargets = GetPowerNumeral(0, 1);
            int numBonusDamage = GetPowerNumeral(1, 1);

            int crashMod = 0;
            IEnumerator coroutine;
            if(OverrideCrashCount == null)
            {
                var storedModifier = new List<int>();
                coroutine = EvaluateCrashInHand(storedModifier);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                crashMod = storedModifier.FirstOrDefault();
            }

            Func<Card, int?> dynamicDamage = delegate (Card c)
            {
                if (OverrideCrashCount.HasValue)
                {
                    return OverrideCrashCount.Value + numBonusDamage;
                }
                return TrueCrashInHand + crashMod + numBonusDamage;
            };
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), dynamicDamage, DamageType.Melee, () => numTargets, false, numTargets, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            OverrideCrashCount = null;
            yield break;
        }
    }
}

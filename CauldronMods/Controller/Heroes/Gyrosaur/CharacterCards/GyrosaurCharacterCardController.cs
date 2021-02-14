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
            ShowCrashInHandCount();
        }


        // DEBUG CODE - Use AddStartOfGameTriggers in order to specify cards you want the character to start with. This allows
        // us to debug specific cards live without having to wait until we draw the card at random.
        public override void AddStartOfGameTriggers()
        {
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "AMerryChase" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "GyroStabilizer" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "HiddenDetour" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "HiddenDetour" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"If you have at least 2 crash cards in your hand, {Gyrosaur} deals up to 3 targets 1 melee damage each. If not, draw a card."
            int numCrashThreshold = GetPowerNumeral(0, 2);
            int numTargets = GetPowerNumeral(1, 3);
            int numDamage = GetPowerNumeral(2, 1);

            Func<bool> showDecisionIf = delegate
            {
                int trueCrash = TrueCrashInHand;
                // Since Gyro Stabilizer may be in play, number of crash cards may be considered 1 higher than actually are in hand.
                if(trueCrash == numCrashThreshold || trueCrash == numCrashThreshold - 1)
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
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"One target with more than 10 HP deals itself 3 melee damage.",
                        coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 3, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => c.IsTarget && c.HitPoints > 10, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"Select a non-character target. Increase damage dealt to that target by 1 until the start of your turn."
                        var storedTarget = new List<SelectCardDecision>();
                        coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.IncreaseDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "", false, singular: "non-character target", plural: "non-character targets"), storedTarget, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        var selectedTarget = storedTarget.FirstOrDefault()?.SelectedCard;
                        if(selectedTarget != null)
                        {
                            var damageEffect = new IncreaseDamageStatusEffect(1);
                            damageEffect.TargetCriteria.IsSpecificCard = selectedTarget;
                            damageEffect.UntilTargetLeavesPlay(selectedTarget);
                            damageEffect.UntilStartOfNextTurn(TurnTaker);
                            damageEffect.CardSource = Card;

                            coroutine = AddStatusEffect(damageEffect);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}

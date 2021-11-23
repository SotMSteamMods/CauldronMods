using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class GargoyleCharacterCardController : GargoyleUtilityCharacterCardController
    {
        private const int REDUCE_DAMAGE_POWER = 1;
        private const int REDUCE_DAMAGE_INCAP = 2;
        private const int INCREASE_DAMAGE_POWER = 1;
        private const int INCREASE_DAMAGE_INCAP = 2;
        private const int NUMBER_USES_ONCE = 1;

        #region Gargoyle character card utilities
        private int ReduceDamagePower => GetPowerNumeral(0, REDUCE_DAMAGE_POWER);
        private int IncreaseDamagePower => GetPowerNumeral(1, INCREASE_DAMAGE_POWER);
        #endregion

        public GargoyleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        // DEBUG CODE - Use AddStartOfGameTriggers in order to specify cards you want the character to start with. This allows
        // us to debug specific cards live without having to wait until we draw the card at random.
        public override void AddStartOfGameTriggers()
        {
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "LeechField" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "ViolentAssist" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "Ultimatum" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "MarkForExecution" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "Wither" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "YourStrenghtIsMine" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "SomethingToFear" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
        }

        /*
         * Power:
         * "Select a target. Reduce the next damage it deals by 1. Increase the next damage Gargoyle deals by 1."
         */
        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
            ReduceDamageStatusEffect reduceDamageStatusEffect;
            IncreaseDamageStatusEffect increaseDamageStatusEffect; 
            IEnumerator coroutine;
            Card selectedCard;

            // Select a target. 
            coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTargetNoDamage, criteria, storedResults, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null && storedResults.FirstOrDefault().SelectedCard != null)
            {
                selectedCard = storedResults.FirstOrDefault().SelectedCard;
                

                // Reduce the next damage it deals by 1
                reduceDamageStatusEffect = new ReduceDamageStatusEffect(ReduceDamagePower);
                reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = selectedCard;
                reduceDamageStatusEffect.NumberOfUses = NUMBER_USES_ONCE;
                reduceDamageStatusEffect.UntilTargetLeavesPlay(selectedCard);

                coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // Increase the next damage Gargoyle deals by 1
            increaseDamageStatusEffect = new IncreaseDamageStatusEffect(IncreaseDamagePower);
            increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            increaseDamageStatusEffect.NumberOfUses = NUMBER_USES_ONCE;
            increaseDamageStatusEffect.UntilTargetLeavesPlay(base.CharacterCard);

            coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
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

        /* 
         * Incap
         * "Increase the next damage dealt by a hero target by 2."
         * "Reduce the next damage dealt to a hero target by 2."
         * "One player may draw a card now."
        */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            IncreaseDamageStatusEffect increaseDamageStatusEffect;
            ReduceDamageStatusEffect reduceDamageStatusEffect;

            switch (index)
            {
                // "Increase the next damage dealt by a hero target by 2."
                case 0:
                    {
                        increaseDamageStatusEffect = new IncreaseDamageStatusEffect(INCREASE_DAMAGE_INCAP);
                        increaseDamageStatusEffect.SourceCriteria.IsHero = true;
                        increaseDamageStatusEffect.SourceCriteria.IsTarget = true;
                        increaseDamageStatusEffect.NumberOfUses = NUMBER_USES_ONCE;

                        coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
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
                    // "Reduce the next damage dealt to a hero target by 2."
                    {
                        reduceDamageStatusEffect = new ReduceDamageStatusEffect(REDUCE_DAMAGE_INCAP);
                        reduceDamageStatusEffect.TargetCriteria.IsHero = true;
                        reduceDamageStatusEffect.TargetCriteria.IsTarget = true;
                        reduceDamageStatusEffect.NumberOfUses = NUMBER_USES_ONCE;

                        coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
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
                    // "One player may draw a card now."
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, optionalDrawCard: true, cardSource: GetCardSource());
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
            }
            yield break;
        }
    }
}

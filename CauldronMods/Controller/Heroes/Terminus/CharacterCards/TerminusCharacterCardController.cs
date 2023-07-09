using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class TerminusCharacterCardController : TerminusBaseCharacterCardController
    {
        private int SelfColdDamage => GetPowerNumeral(0, 2);
        private int TargetCount => GetPowerNumeral(1, 1);
        private int TargetColdDamage => GetPowerNumeral(2, 3);

        public TerminusCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        
        public override void AddStartOfGameTriggers()
        {
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.GameController.SetHP(base.CharacterCard, 2), TriggerType.PhaseChange, TriggerTiming.After);

            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "StainedBadge" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "TheLightAtTheEnd" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "CovenantOfWrath" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.GameController.AddTokensToPool(base.CharacterCard.FindTokenPool("TerminusWrathPool"), 3, base.GetCardSource()), TriggerType.PhaseChange, TriggerTiming.After);
        }

        // "{Terminus} deals herself 2 cold damage and 1 target 3 cold damage."
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.DealDamageToSelf(DecisionMaker, (card) => card == base.CharacterCard, SelfColdDamage, DamageType.Cold, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), TargetColdDamage, DamageType.Cold, TargetCount, false, TargetCount, additionalCriteria: (card) => card != base.CharacterCard, cardSource: base.GetCardSource());
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
         * One player may play a card now. 
         * Each hero may use a power printed on one of their non-character cards. Then destroy any card whose power was used this way. 
         * Each target regains 1 HP.
         */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One player may play a card now. 
                        coroutine = base.GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: base.GetCardSource());
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
                        // Each hero may use a power printed on one of their non-character cards. Then destroy any card whose power was used this way. 
                        coroutine = base.GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => c.IsRealCard &&  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, "hero"), SelectionType.UsePower, UsePowerResponse, null, optional: false, 0, null, allowAutoDecide: false, null, GetCardSource());
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
                        // Each target regains 1 HP.
                        coroutine = base.GameController.GainHP(DecisionMaker, (card) => card.IsTarget, 1, cardSource: base.GetCardSource());
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

        public IEnumerator UsePowerResponse(Card card)
        { 
            IEnumerator coroutine;
            CardController cardController = FindCardController(card);
            List<UsePowerDecision> usePowerDecisions = new List<UsePowerDecision>();

            coroutine = base.GameController.SelectAndUsePower(cardController.DecisionMaker, optional: true, (power)=>!IsHeroCharacterCard(power.CardSource.Card), 1, eliminateUsedPowers: true, storedResults: usePowerDecisions, showMessage: false, allowAnyHeroPower: false, allowReplacements: true, canBeCancelled: true, null, forceDecision: false, allowOutOfPlayPower: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (usePowerDecisions != null && usePowerDecisions.Count() > 0)
            {
                coroutine = base.GameController.DestroyCard(cardController.DecisionMaker, usePowerDecisions.FirstOrDefault().SelectedPower.CardSource.Card, cardSource: base.GetCardSource());
                
                if(base.UseUnityCoroutines)
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

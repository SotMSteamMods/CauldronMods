using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class WastelandRoninCricketCharacterCardController : HeroCharacterCardController
    {
        public WastelandRoninCricketCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine = null;
            switch (index)
            {
                case 0:
                    {
                        //One hero may use a power now.
                        coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        List<SelectLocationDecision> storedLocation = new List<SelectLocationDecision>();
                        //Look at the top card of a deck and replace or discard it.
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, null, storedLocation, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectLocation(storedLocation))
                        {
                            coroutine = base.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.TurnTakerController, storedLocation.FirstOrDefault().SelectedLocation.Location, false);
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
                case 2:
                    {
                        //Each target deals themselves 1 sonic damage.
                        coroutine = base.GameController.DealDamageToSelf(base.HeroTurnTakerController, null, 1, DamageType.Sonic, cardSource: base.GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            //Increase damage dealt by {Cricket} during your next turn by 1. {Cricket} may deal 1 target 1 sonic damage.
            int increaseNumeral = GetPowerNumeral(0, 1);
            int targetNumeral = GetPowerNumeral(1, 1);
            int damageNumeral = GetPowerNumeral(2, 1);

            string increaseString = "One";
            if (increaseNumeral == 2)
            {
                increaseString = "Two";
            }
            else if (increaseNumeral == 0)
            {
                increaseString = "Zero";
            }
            //Increase damage dealt by {Cricket} during your next turn by 1.
            OnPhaseChangeStatusEffect statusEffect = new OnPhaseChangeStatusEffect(base.Card, "IncreaseDamageResponse" + increaseNumeral, "Increase damage dealt by {Cricket} during your next turn by 1", new TriggerType[] { TriggerType.IncreaseDamage }, base.Card);
            statusEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
            statusEffect.TurnPhaseCriteria.Phase = Phase.Start;
            statusEffect.UntilEndOfNextTurn(base.TurnTaker);
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);

            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Cricket} may deal 1 target 1 sonic damage.
            coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Sonic, targetNumeral, false, 0, cardSource: base.GetCardSource());
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

        private IEnumerator IncreaseDamageResponse(int increaseNumeral)
        {
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(increaseNumeral);
            statusEffect.UntilEndOfPhase(base.TurnTaker, Phase.End);
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);
            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
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

        public IEnumerator IncreaseDamageResponse1(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            IEnumerator coroutine = this.IncreaseDamageResponse(1);
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

        public IEnumerator IncreaseDamageResponse2(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            IEnumerator coroutine = this.IncreaseDamageResponse(2);
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
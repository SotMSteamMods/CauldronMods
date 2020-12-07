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

            //Increase damage dealt by {Cricket} during your next turn by 1.
            //OnPhaseChangeStatusEffect

            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(increaseNumeral);
            statusEffect.UntilEndOfNextTurn(base.TurnTaker);
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);
            //Only during next turn
            IncreaseDamageStatusEffect reduceEffect = new IncreaseDamageStatusEffect(-1 * increaseNumeral);
            reduceEffect.UntilStartOfNextTurn(base.TurnTaker);
            reduceEffect.UntilTargetLeavesPlay(base.CharacterCard);

            statusEffect.CombineWithStatusEffect(reduceEffect);
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
    }
}
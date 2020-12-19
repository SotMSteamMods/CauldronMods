using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    class MinistryOfStrategicScienceMagnificentMaraCharacterCardController : MaraUtilityCharacterCardController
    {
        public MinistryOfStrategicScienceMagnificentMaraCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{MagnificentMara} deals 1 target 2 radiant damage. Increase damage dealt by that target by 1 until the start of your next turn."
            int numTargets = GetPowerNumeral(0, 1);
            int numDamage = GetPowerNumeral(1, 2);
            int numBoost = GetPowerNumeral(2, 1);

            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), numDamage, DamageType.Radiant, numTargets, false, numTargets,
                                                                addStatusEffect: dd => IncreaseDamageDealtByThatTargetResponse(dd, numBoost), selectTargetsEvenIfCannotDealDamage: true, cardSource: this.GetCardSource());
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

        private IEnumerator IncreaseDamageDealtByThatTargetResponse(DealDamageAction dd, int numBoost)
        {
            //Increase damage dealt by that target by 1 until the start of your next turn.
            var target = dd.Target;
            var boostEffect = new IncreaseDamageStatusEffect(numBoost);
            boostEffect.UntilStartOfNextTurn(this.TurnTaker);
            boostEffect.SourceCriteria.IsSpecificCard = target;
            boostEffect.UntilCardLeavesPlay(target);

            IEnumerator coroutine = AddStatusEffect(boostEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch(index)
            {
                case (0):
                    {
                        //"One player may draw a card now.",
                        break;
                    }
                case (1):
                    {
                        //"Reduce the next damage dealt to a hero target by 2.",

                        break;
                    }
                case (2):
                    {
                        //"The next time a hero ongoing card is destroyed, put that card in its owner's hand."
                        break;
                    }
            }
            yield break;
        }
    }
}

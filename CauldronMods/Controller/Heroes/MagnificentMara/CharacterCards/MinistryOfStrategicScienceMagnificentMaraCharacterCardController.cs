using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MinistryOfStrategicScienceMagnificentMaraCharacterCardController : MaraUtilityCharacterCardController
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
                case (1):
                    {
                        //"Reduce the next damage dealt to a hero target by 2.",
                        var reduceEffect = new ReduceDamageStatusEffect(2);
                        reduceEffect.NumberOfUses = 1;
                        reduceEffect.TargetCriteria.IsHero = true;
                        reduceEffect.TargetCriteria.IsTarget = true;
                        coroutine = AddStatusEffect(reduceEffect);
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
                case (2):
                    {
                        //"The next time a hero ongoing card is destroyed, put that card in its owner's hand."
                        var rescueEffect = new WhenCardIsDestroyedStatusEffect(CardWithoutReplacements, nameof(RescueOngoingResponse), "The next time a hero ongoing card is destroyed, put that card in its owner's hand.", new TriggerType[] { TriggerType.MoveCard }, DecisionMaker.HeroTurnTaker, this.Card);
                        rescueEffect.NumberOfUses = 1;
                        rescueEffect.CardDestroyedCriteria.IsHero = true;
                        rescueEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string> { "ongoing" };

                        coroutine = AddStatusEffect(rescueEffect);
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

        public IEnumerator RescueOngoingResponse(DestroyCardAction dc, HeroTurnTaker _1, StatusEffect _2, int[] _3 = null)
        {
            if(dc.PostDestroyDestinationCanBeChanged && dc.CardToDestroy != null)
            {
                dc.PostDestroyDestinationCanBeChanged = false;
                dc.AddAfterDestroyedAction(() => GameController.MoveCard(DecisionMaker, dc.CardToDestroy.Card, dc.CardToDestroy.HeroTurnTaker.Hand, cardSource: GetCardSource()), this);
            }
            yield break;
        }
    }
}

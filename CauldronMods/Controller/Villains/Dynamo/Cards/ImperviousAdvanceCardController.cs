using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class ImperviousAdvanceCardController : DynamoUtilityCardController
    {
        public ImperviousAdvanceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(CopperheadIdentifier);
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public readonly string CopperheadIdentifier = "Copperhead";

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //If Copperhead is in play, reduce damage dealt to villain targets by 1 until the start of the next villain turn.
            if (base.FindCopperhead().IsInPlayAndHasGameText)
            {
                ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(1);
                statusEffect.TargetCriteria.IsVillain = true;
                statusEffect.TargetCriteria.IsTarget = true;
                statusEffect.UntilStartOfNextTurn(base.TurnTaker);

                coroutine = base.AddStatusEffect(statusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //The villain target with the highest HP...
            IEnumerable<Card> highestVillains = base.GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => base.IsVillainTarget(c), base.GetCardSource());
            Card highestVillain = highestVillains.FirstOrDefault();
            if (highestVillains.Count() > 1)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => highestVillains.Contains(c)), storedResults, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                highestVillain = storedResults.FirstOrDefault().SelectedCard;
            }

            //...deals the hero target with the second highest HP {H} melee damage.
            coroutine = base.DealDamageToHighestHP(highestVillain, 2, (Card c) => IsHeroTarget(c), (Card c) => base.Game.H, DamageType.Melee);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class AethiumRageCardController : NightloreCitadelUtilityCardController
    {
        public AethiumRageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 2);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, the villain target with the highest HP

            List<Card> storedHighest = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsVillainTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedHighest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedHighest.Any())
            {
                Card highestVillain = storedHighest.First();
                if (storedHighest.Count() > 1)
                {

                    List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                    coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.GainHP, new LinqCardCriteria((Card c) => storedHighest.Contains(c)), storedResults, false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (DidSelectCard(storedResults))
                    {
                        highestVillain = GetSelectedCard(storedResults);
                    }
                }

                //...regains {H} HP...
                coroutine = GameController.GainHP(highestVillain, Game.H, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...deals the 2 non-villain targets with the highest HP 3 radiant damage each.
                coroutine = DealDamageToHighestHP(highestVillain, 1, (Card c) => !IsVillainTarget(c), (Card c) => 3, DamageType.Radiant, numberOfTargets: () => 2);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            
            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(null);
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

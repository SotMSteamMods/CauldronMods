using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class HeresThePlanCardController : DynamoUtilityCardController
    {
        public HeresThePlanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, cardCriteria: new LinqCardCriteria(c => IsPlot(c), "plot"));
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until a Plot is revealed. Put it into play and shuffle the other revealed cards back into the villain deck.
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => base.IsPlot(c)), 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
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

            //...deals each hero target {H - 1} melee damage.
            coroutine = base.DealDamage(highestVillain, (Card c) => IsHeroTarget(c), base.Game.H - 1, DamageType.Melee);
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

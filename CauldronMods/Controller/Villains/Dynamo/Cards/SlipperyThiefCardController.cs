using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class SlipperyThiefCardController : DynamoUtilityCardController
    {
        public SlipperyThiefCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //If Python is in play...
            if (base.FindPython().IsInPlayAndHasGameText)
            {
                //...he deals each hero target 1 toxic damage...
                coroutine = base.DealDamage(base.FindPython(), (Card c) => IsHeroTarget(c), (Card c) => 1, DamageType.Toxic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...regains {H} HP...
                coroutine = base.GameController.GainHP(base.FindPython(), base.Game.H, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...and discards the top card of the villain deck.
                List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
                coroutine = base.GameController.DiscardTopCard(base.TurnTaker.Deck, moveCardActions);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //The villain target with the lowest HP...
            IEnumerable<Card> lowestVillains = base.GameController.FindAllTargetsWithLowestHitPoints(1, (Card c) => base.IsVillainTarget(c), base.GetCardSource());
            Card lowestVillain = lowestVillains.FirstOrDefault();
            if (lowestVillains.Count() > 1)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => lowestVillains.Contains(c)), storedResults, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                lowestVillain = storedResults.FirstOrDefault().SelectedCard;
            }

            //...deals the hero target with the lowest HP {H - 2} melee damage.
            coroutine = base.DealDamageToLowestHP(lowestVillain, 1, (Card c) => IsHeroTarget(c), (Card c) => base.Game.H - 2, DamageType.Melee);
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

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class ObsidianGraspCardController : TitanCardController
    {
        public ObsidianGraspCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override IEnumerator Play()
        {
            //{Titan} deals 1 target 3 melee damage.
            List<SelectCardDecision> storedTarget = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Melee, 1, false, 1, storedResultsDecisions: storedTarget, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If Titanform is in play... 
            if (base.GetTitanform().IsInPlayAndNotUnderCard && storedTarget.Any())
            {
                //you may discard a card...
                List<DiscardCardAction> storedResult = new List<DiscardCardAction>();
                coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, 1, true, storedResults: storedResult);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //...to search your deck and trash for a copy of the card Immolate and play it next to that target. If you searched your deck, shuffle it.
                if (base.DidDiscardCards(storedResult))
                {
                    List<SelectCardDecision> selectedImmolate = new List<SelectCardDecision>();
                    coroutine = base.SearchForCards(base.HeroTurnTakerController, true, false, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "Immolate"), true, false, false, storedResults: selectedImmolate, shuffleAfterwards: true);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    CardController immolateController = FindCardController(selectedImmolate.FirstOrDefault().SelectedCard);
                    if (immolateController is ImmolateCardController)
                    {
                        IEnumerator play = (immolateController as ImmolateCardController).PlayBySpecifiedTarget(storedTarget.FirstOrDefault().SelectedCard, true, base.GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(play);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(play);
                        }
                    }
                }
            }
            yield break;
        }
    }
}
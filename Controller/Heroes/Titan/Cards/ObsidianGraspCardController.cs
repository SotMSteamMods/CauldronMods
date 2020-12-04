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
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.Identifier == "Immolate", "Immolate"));
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "Immolate", "Immolate"));
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
                    IEnumerable<Card> immolates = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "Immolate" && (c.IsInTrash || c.IsInDeck)));
                    coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.PlayCard, immolates, selectedImmolate, cardSource: base.GetCardSource());
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
                        coroutine = (immolateController as ImmolateCardController).PlayBySpecifiedTarget(storedTarget.FirstOrDefault().SelectedCard, true, base.GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                }
            }
            yield break;
        }
    }
}
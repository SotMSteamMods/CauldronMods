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
            if (base.GetTitanform().IsInPlayAndNotUnderCard && storedTarget.Any() && storedTarget.FirstOrDefault().SelectedCard.Location.IsInPlayAndNotUnderCard)
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
                    bool immolateInDeck = FindCardsWhere((Card c) => c.Location == TurnTaker.Deck && c.Identifier == "Immolate").Any();
                    bool immolateInTrash = FindCardsWhere((Card c) => c.Location == TurnTaker.Trash && c.Identifier == "Immolate").Any();
                    var searches = new List<Function>
                    {
                        new Function(HeroTurnTakerController, "Search deck", SelectionType.SearchDeck, () => SearchLocationAndPlayImmolateByTarget(TurnTaker.Deck, storedTarget.FirstOrDefault().SelectedCard), immolateInDeck, $"{TurnTaker.Name} searches their deck for Immolate."),
                        new Function(HeroTurnTakerController, "Search trash", SelectionType.SearchTrash, () => SearchLocationAndPlayImmolateByTarget(TurnTaker.Trash, storedTarget.FirstOrDefault().SelectedCard), immolateInTrash, $"{TurnTaker.Name} searches their trash for Immolate.")
                    };

                    var decision = new SelectFunctionDecision(GameController, HeroTurnTakerController, searches, false, noSelectableFunctionMessage: "There were no more copies of Immolate to search for.", cardSource: GetCardSource());

                    coroutine = GameController.SelectAndPerformFunction(decision);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }

        private IEnumerator SearchLocationAndPlayImmolateByTarget(Location toSearch, Card target)
        {
            IEnumerator coroutine;
            var foundImmolate = FindCardsWhere((Card c) => c.Location == toSearch && c.Identifier == "Immolate").FirstOrDefault();
            if (foundImmolate != null)
            {
                var controller = FindCardController(foundImmolate);
                if (controller is ImmolateCardController)
                {
                    coroutine = (controller as ImmolateCardController).PlayBySpecifiedTarget(target, false, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            if (toSearch.IsDeck)
            {
                coroutine = GameController.ShuffleLocation(toSearch, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}

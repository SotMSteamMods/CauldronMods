using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class NoRestForTheWickedCardController : TerminusBaseCardController
    {
        /*
         * You may put a target from the villain trash into play. If that target has more than 5HP, 
         * reduce its current HP to 5. That target deals up to 2 other targets 5 infernal damage each. 
         * If no target enters play this way, add 5 tokens to your Wrath pool.
         */
        public NoRestForTheWickedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<LocationChoice> possibleDestinations;
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
            Location selectedLocation;
            bool targetEnteredPlay = false;

            possibleDestinations = FindVillainTurnTakerControllers(true).Select((ttc) => new LocationChoice(ttc.TurnTaker.Trash)).ToList();

            coroutine = base.GameController.SelectLocation(DecisionMaker, possibleDestinations, SelectionType.SearchTrash, storedResults, true, base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectLocation(storedResults))
            {
                // You may put a target from the villain trash into play.
                selectedLocation = storedResults.FirstOrDefault().SelectedLocation.Location;
                coroutine = base.GameController.SelectCardFromLocationAndMoveIt(DecisionMaker,
                    selectedLocation,
                    new LinqCardCriteria((lcc) => lcc.IsTarget),
                    new List<MoveCardDestination> { new MoveCardDestination(selectedLocation.OwnerTurnTaker.PlayArea) },
                    storedResults: selectCardDecisions,
                    cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (DidSelectCard(selectCardDecisions))
                {
                    targetEnteredPlay = true;
                    var selectedCard = selectCardDecisions.FirstOrDefault().SelectedCard;
                    // If that target has more than 5HP, reduce its current HP to 5.
                    if (selectedCard.HitPoints > 5)
                    {
                        coroutine = base.GameController.SetHP(selectedCard, 5, cardSource: base.GetCardSource()); 
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                    }

                    // That target deals up to 2 other targets 5 infernal damage each. 
                    coroutine = base.GameController.SelectCardsAndDoAction(DecisionMaker, 
                        new LinqCardCriteria((lcc) => lcc.IsTarget && lcc.IsInPlayAndHasGameText && lcc != selectedCard), 
                        SelectionType.DealDamage, 
                        (card) => ActionWithCardResponse(selectedCard, card), 
                        2, 
                        false, 
                        0, 
                        cardSource: base.GetCardSource());
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

            if (!targetEnteredPlay)
            {
                // If no target enters play this way, add 5 tokens to your Wrath pool.
                coroutine = base.AddWrathTokens(5);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        private IEnumerator ActionWithCardResponse(Card sourceCard, Card targetCard)
        {
            IEnumerator coroutine;

            // That target deals up to 2 other targets 5 infernal damage each.
            coroutine = base.DealDamage(sourceCard, (card) => targetCard == card, 5, DamageType.Infernal);
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

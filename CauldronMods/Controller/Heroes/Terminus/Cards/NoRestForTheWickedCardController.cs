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

            Func<TurnTakerController, List<LocationChoice>> allVillainTrashesWithTargets = delegate (TurnTakerController ttc)
            {
                var trashes = new List<LocationChoice>();
                if(GameController.IsLocationVisibleToSource(ttc.TurnTaker.Trash, GetCardSource()) && ttc.TurnTaker.Trash.Cards.Any((Card c) => c.IsTarget && GameController.CanPlayCard(FindCardController(c), true, destinationLocation: ttc.TurnTaker.PlayArea) == CanPlayCardResult.CanPlay))
                {
                    trashes.Add(new LocationChoice(ttc.TurnTaker.Trash));
                }
                foreach (Location subtrash in ttc.TurnTaker.SubTrashes)
                {
                    if (subtrash.IsRealTrash && GameController.IsLocationVisibleToSource(subtrash, GetCardSource()) && subtrash.Cards.Any((Card c) => c.IsTarget && GameController.CanPlayCard(FindCardController(c), true, destinationLocation: ttc.TurnTaker.PlayArea) == CanPlayCardResult.CanPlay))
                    {
                        trashes.Add(new LocationChoice(subtrash));
                    }
                }
                return trashes;
            };
            possibleDestinations = FindVillainTurnTakerControllers(true).Where(ttc => GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource())).SelectMany((TurnTakerController ttc) => allVillainTrashesWithTargets(ttc)).ToList();

            Location chosenLocation = null;
            if(possibleDestinations.Count() == 0)
            {
                coroutine = GameController.SendMessageAction($"There were no villain targets {Card.Title} could return from a trash.", Priority.Medium, GetCardSource());
            }
            else if (possibleDestinations.Count() == 1)
            {
                chosenLocation = possibleDestinations.FirstOrDefault().Location;
            }
            else
            {
                coroutine = base.GameController.SelectLocation(DecisionMaker, possibleDestinations, SelectionType.SearchTrash, storedResults, false, base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(DidSelectLocation(storedResults))
                {
                    chosenLocation = GetSelectedLocation(storedResults);
                }
            }

            if (chosenLocation != null)
            {
                var storedResultsMove = new List<MoveCardAction>();
                // You may put a target from the villain trash into play.
                selectedLocation = chosenLocation;
                coroutine = base.GameController.SelectCardFromLocationAndMoveIt(DecisionMaker,
                    selectedLocation,
                    new LinqCardCriteria((lcc) => lcc.IsTarget, "target"),
                    new List<MoveCardDestination> { new MoveCardDestination(selectedLocation.OwnerTurnTaker.PlayArea) },
                    optional: true,
                    storedResultsMove: storedResultsMove,
                    storedResults: selectCardDecisions,
                    isPutIntoPlay: true,
                    cardSource: base.GetCardSource()) ;
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                Card selectedCard = null;
                var moveAction = storedResultsMove.FirstOrDefault();
                if(moveAction != null && moveAction.Destination.IsInPlayAndNotUnderCard && moveAction.IsSuccessful)
                {
                    targetEnteredPlay = true;
                    selectedCard = selectCardDecisions.FirstOrDefault().SelectedCard;
                }
                if (selectedCard != null && selectedCard.IsInPlayAndHasGameText)
                {
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

                    coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, selectedCard), 5, DamageType.Infernal, 2, false, 0, additionalCriteria: (Card c) => c != selectedCard, cardSource: GetCardSource());
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

    }
}

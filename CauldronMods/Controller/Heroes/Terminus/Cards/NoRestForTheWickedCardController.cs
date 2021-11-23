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
            IEnumerable<TurnTaker> villainTurnTakers = Game.TurnTakers.Where(tt => !tt.IsIncapacitatedOrOutOfGame && IsVillain(tt));
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
            Location selectedLocation;
            bool targetEnteredPlay = false;

            List<Location> possibleLocationsList = new List<Location>();
            foreach (TurnTaker tt in villainTurnTakers)
            {
                if (TrashCondition(tt.Trash))
                {
                    possibleLocationsList.Add(tt.Trash);
                }

                foreach (Location subtrash in tt.SubTrashes)
                {
                    if (subtrash.IsRealTrash && TrashCondition(subtrash))
                    {
                        possibleLocationsList.Add(subtrash);
                    }
                }
            }

            List<LocationChoice> possibleDestinations = possibleLocationsList.Select(loc => new LocationChoice(loc)).ToList();
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

        private bool TrashCondition(Location loc)
        {
            return GameController.IsLocationVisibleToSource(loc, GetCardSource()) && loc.Cards.Any((Card c) => c.IsTarget && GameController.CanPlayCard(FindCardController(c), true, destinationLocation: loc.OwnerTurnTaker.PlayArea) == CanPlayCardResult.CanPlay);
        }
    }
}

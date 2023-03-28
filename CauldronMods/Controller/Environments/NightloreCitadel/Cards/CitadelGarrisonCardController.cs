using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class CitadelGarrisonCardController : NightloreCitadelUtilityCardController
    {
        public CitadelGarrisonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            AddDealDamageAtStartOfTurnTrigger(TurnTaker, Card, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, Game.H + 1, DamageType.Radiant, highestLowestRanking: 2);
            //Then, if Starlight of Oros and Aethium Cannon are in play, discard 2 cards from beneath Aethium Cannon.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DiscardCardResponse, TriggerType.MoveCard, (PhaseChangeAction pca) => IsStarlightOfOrosInPlay() && IsAethiumCannonInPlay());
        }

        private IEnumerator DiscardCardResponse(PhaseChangeAction pca)
        {
            Card cannon = FindAethiumCannonInPlay();
            IEnumerator coroutine;
            if(cannon.UnderLocation.NumberOfCards == 0)
            {
                coroutine = GameController.SendMessageAction("There are no cards under " + cannon.Title + " for " + Card.Title + " to discard.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            } else if(cannon.UnderLocation.NumberOfCards <= 2)
            {
                coroutine = GameController.MoveCards(TurnTakerController, cannon.UnderLocation.Cards, (Card c) => new MoveCardDestination(c.Owner.Trash), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            } else
            {
                //select 2 cards under cannon then move
                 List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
                coroutine = this.GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.MoveCard, (Card c) => cannon.UnderLocation.HasCard(c), 2, storedResults, false, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
                if(DidSelectCards(storedResults, 2))
                {
                    IEnumerable<Card> selectedCards = GetSelectedCards(storedResults);
                    coroutine = GameController.MoveCards(TurnTakerController, selectedCards, (Card c) => new MoveCardDestination(c.Owner.Trash), cardSource: GetCardSource());
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
    }
}

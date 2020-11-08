using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller.TheChairman;

namespace Cauldron.Necro
{
    public class GrandSummonCardController : NecroCardController
    {
        public GrandSummonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override IEnumerator Play()
        {
            //Reveal cards from the top of your deck until you reveal 2 Undead cards. 
            List<RevealCardsAction> revealedCardActions = new List<RevealCardsAction>();
            IEnumerator coroutine = base.GameController.RevealCards(base.HeroTurnTakerController, base.TurnTaker.Deck, (Card c) => this.IsUndead(c), 2, revealedCardActions, cardSource: this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<Card> undeadCards = GetRevealedCards(revealedCardActions).Where(c => IsUndead(c)).Take(2).ToList();
            List<Card> otherCards = GetRevealedCards(revealedCardActions).Where(c => !undeadCards.Contains(c)).ToList();
            if (undeadCards.Any())
            {
                //Put 1 into play 
                coroutine = base.GameController.SelectAndPlayCard(this.DecisionMaker, undeadCards, isPutIntoPlay: true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                Card nonSelectedCard = undeadCards.FirstOrDefault(c => c.Location.IsRevealed);
                if (nonSelectedCard != null)
                {
                    //and put 1 into the trash.
                    coroutine = base.GameController.MoveCard(this.DecisionMaker, nonSelectedCard, base.FindCardController(nonSelectedCard).GetTrashDestination(), showMessage: true, cardSource: base.GetCardSource());
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
            if (otherCards.Any())
            {
                //Put the remaining revealed cards back on the deck and shuffle
                coroutine = base.GameController.MoveCards(this.DecisionMaker, otherCards, this.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = this.ShuffleDeck(this.DecisionMaker, this.TurnTaker.Deck);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}

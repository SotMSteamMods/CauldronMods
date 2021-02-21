using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class TintedStagCardController : DendronBaseCardController
    {
        //==============================================================
        // At the end of the villain turn, shuffle the villain trash
        // and reveal cards until you reveal a Tattoo.
        // Put that card into play.
        // Put the rest of the cards back into the villain trash.
        //==============================================================

        public static readonly string Identifier = "TintedStag";

        public TintedStagCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => IsTattoo(c), "tattoo"));
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, this.RevealTattooResponse, new[]
            {
                TriggerType.ShuffleDeck,
                TriggerType.RevealCard,
                TriggerType.MoveCard
            });
        }

        private IEnumerator RevealTattooResponse(PhaseChangeAction pca)
        {
            // Shuffle villain trash
            IEnumerator shuffleTrashRoutine = base.GameController.ShuffleLocation(TurnTaker.Trash, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleTrashRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleTrashRoutine);
            }

            // Reveal cards from trash until tattoo is revealed
            List<RevealCardsAction> revealedCardList = new List<RevealCardsAction>();
            IEnumerator revealCardRoutine = base.GameController.RevealCards(this.TurnTakerController, this.TurnTaker.Trash, IsTattoo, 1, revealedCardList, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardRoutine);
            }

            List<Card> tattooCards = GetRevealedCards(revealedCardList).Where(IsTattoo).Take(1).ToList();
            List<Card> otherCards = GetRevealedCards(revealedCardList).Where(c => !tattooCards.Contains(c)).ToList();

            if (tattooCards.Any())
            {
                // Put tattoo into play
                IEnumerator playCardRoutine = base.GameController.PlayCards(this.DecisionMaker, c => tattooCards.Contains(c), false, true, 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCardRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCardRoutine);
                }
            }

            if (!otherCards.Any())
            {
                yield break;
            }

            // Put remaining revealed cards back into the villain trash
            IEnumerator returnCardsToTrashRoutine = base.GameController.MoveCards(this.DecisionMaker, otherCards, this.TurnTaker.Trash, cardSource: base.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(returnCardsToTrashRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(returnCardsToTrashRoutine);
            }
        }
    }
}
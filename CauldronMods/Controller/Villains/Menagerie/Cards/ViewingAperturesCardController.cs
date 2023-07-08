using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class ViewingAperturesCardController : MenagerieCardController
    {
        public ViewingAperturesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Play the top card of the villain deck.
            IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Select 1 face down card beneath each Enclosure. Flip those cards face up.
            var actedEnclosures = new List<Card>();
            var criteria = new LinqCardCriteria(c => IsEnclosure(c) && !actedEnclosures.Contains(c) && c.IsInPlayAndHasGameText && c.UnderLocation.HasCards && HasFaceDownCards(c), "enclosure");
            if (GameController.FindCardsWhere(criteria).Any())
            {
                coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, criteria, SelectionType.None, c => FlipEnclosedCardResponse(c, actedEnclosures), cardSource: GetCardSource());
            }
            else
            {
                coroutine = GameController.SendMessageAction($"There are no enclosures with facedown cards for {Card.Title} to flip face up.", Priority.Medium, GetCardSource(), null, true);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Menagerie} deals each hero, environment, and Specimen target 1 psychic damage.
            coroutine = base.DealDamage(base.CharacterCard, (Card c) => IsHero(c) || c.IsEnvironmentTarget || base.IsSpecimen(c), 1, DamageType.Psychic);
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

        private IEnumerator FlipEnclosedCardResponse(Card card, List<Card> actedEnclosures)
        {
            List<SelectCardDecision> result = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.FlipCardFaceUp, new LinqCardCriteria((Card c) => c.IsFlipped && c.Location == card.UnderLocation), result, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            actedEnclosures.Add(card);

            if (DidSelectCard(result))
            {
                var selectedCard = GetSelectedCard(result);
                coroutine = base.GameController.FlipCard(base.FindCardController(selectedCard), cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SendMessageAction($"{Card.Title} flips {selectedCard.Title} face up.", Priority.Medium, GetCardSource(), new[] { selectedCard }, true);
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

        private bool HasFaceDownCards(Card c)
        {
            foreach (Card underCard in c.UnderLocation.Cards)
            {
                if (underCard.IsFlipped)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
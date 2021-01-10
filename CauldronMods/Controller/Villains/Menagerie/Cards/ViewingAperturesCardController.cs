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
            IEnumerable<Card> enclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && base.IsEnclosure(c) && c.UnderLocation.HasCards && this.HasFaceDownCards(c)));
            var actedEnclosures = new List<Card>();
            foreach (Card enclosure in enclosures)
            {
                IEnumerable<Card> choices = base.FindCardsWhere((new LinqCardCriteria((Card c) => enclosures.Contains(c) && !actedEnclosures.Contains(c) && this.HasFaceDownCards(c))));
                if (choices.Any())
                {
                    SelectCardDecision cardDecision = new SelectCardDecision(base.GameController, this.DecisionMaker, SelectionType.FlipCardFaceUp, choices, cardSource: base.GetCardSource());
                    coroutine = base.GameController.SelectCardAndDoAction(cardDecision, (SelectCardDecision decision) => this.FlipEnclosedCardResponse(decision));
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    actedEnclosures.Add(cardDecision.SelectedCard);
                }
            }

            //{Menagerie} deals each hero, environment, and Specimen target 1 psychic damage.
            coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero || c.IsEnvironmentTarget || base.IsSpecimen(c), 1, DamageType.Psychic);
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

        private IEnumerator FlipEnclosedCardResponse(SelectCardDecision decision)
        {
            List<SelectCardDecision> cardDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.FlipCardFaceUp, new LinqCardCriteria((Card c) => !c.IsFaceUp && decision.SelectedCard.UnderLocation.Cards.Contains(c)), cardDecision, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.GameController.FlipCard(base.FindCardController(cardDecision.FirstOrDefault().SelectedCard), cardSource: base.GetCardSource());
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

        private bool HasFaceDownCards(Card c)
        {
            foreach (Card underCard in c.UnderLocation.Cards)
            {
                if (!underCard.IsFaceUp)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
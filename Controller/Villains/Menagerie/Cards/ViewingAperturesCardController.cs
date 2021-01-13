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

        private List<Card> actedEnclosures;

        public override IEnumerator Play()
        {
            //Play the top card of the villain deck.
            IEnumerator coroutine = base.PlayCardFromLocation(base.TurnTaker.Deck, "villain deck");
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Select 1 face down card beneath each Enclosure. Flip those cards face up.
            IEnumerable<Card> enclosures = base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsEnclosure(c) && c.UnderLocation.HasCards));
            foreach (Card enclosure in enclosures)
            {
                Card[] choices = base.FindCardsWhere((new LinqCardCriteria((Card c) => enclosures.Contains(c) && !actedEnclosures.Contains(c)))).ToArray();
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
                    actedEnclosures.Add(enclosure);
                }
            }

            //{Menagerie} deals each hero, environment, and Specimen target 1 psychic damage.
            coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero || c.IsEnvironment || base.IsSpecimen(c), 1, DamageType.Psychic);
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
            IEnumerator coroutine = base.GameController.SelectAndFlipCards(this.DecisionMaker, new LinqCardCriteria((Card c) => decision.SelectedCard.UnderLocation.Cards.Contains(c)), toFaceDown: false, cardSource: base.GetCardSource());
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
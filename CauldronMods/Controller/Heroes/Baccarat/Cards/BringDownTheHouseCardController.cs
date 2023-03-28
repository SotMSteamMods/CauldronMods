using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class BringDownTheHouseCardController : CardController
    {
        public BringDownTheHouseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => TwoOrMoreCopiesInTrash(c) && c.IsInTrash, "pairs of"));
            X = 0;
        }

        private int X;

        public override IEnumerator Play()
        {
            int maxNumberOfPairs = (from c in base.TurnTaker.Trash.Cards
                                    where TwoOrMoreCopiesInTrash(c)
                                    select c).Count<Card>() / 2;

            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            SelectCardsDecision selectCardsDecision = new SelectCardsDecision(base.GameController, base.HeroTurnTakerController, (Card c) => TwoOrMoreCopiesInTrash(c) && c.IsInTrash, SelectionType.ShuffleCardFromTrashIntoDeck, numberOfCards: maxNumberOfPairs, requiredDecisions: new int?(0), eliminateOptions: true, cardSource: base.GetCardSource(null));
            //Pick first Card
            IEnumerator coroutine = base.GameController.SelectCardsAndDoAction(selectCardsDecision, this.ShufflePairIntoDeckResponse, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may destroy up to X ongoing or environment cards, where X is the number of pairs you shuffled this way.
            coroutine = base.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsOngoing(c) || c.IsEnvironment), X, requiredDecisions: new int?(0), responsibleCard: this.CharacterCard, cardSource: base.GetCardSource());
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

        private IEnumerator ShufflePairIntoDeckResponse(SelectCardDecision decision)
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            //Pick second card
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ShuffleCardFromTrashIntoDeck, new LinqCardCriteria((Card c) => c.Identifier == decision.SelectedCard.Identifier && c.InstanceIndex != decision.SelectedCard.InstanceIndex && c.IsInTrash, "two cards with the same name"), storedResults, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Actually shuffle cards into deck
            coroutine = base.GameController.ShuffleCardsIntoLocation(base.HeroTurnTakerController, new Card[] { decision.SelectedCard, storedResults.FirstOrDefault().SelectedCard }, base.TurnTaker.Deck, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            X++;
            yield break;
        }

        private bool TwoOrMoreCopiesInTrash(Card c)
        {
            return (from card in base.TurnTaker.Trash.Cards
                    where card.Identifier == c.Identifier
                    select card).Count<Card>() >= 2;
        }
    }
}
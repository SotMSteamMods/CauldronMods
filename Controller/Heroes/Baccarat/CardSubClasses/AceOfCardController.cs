using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Baccarat
{
    public abstract class AceOfCardController : CardController
    {
        public AceOfCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => TwoOrMoreCopiesInTrash(c) && c.IsInTrash));
        }

        protected abstract ITrigger EffectTrigger();

        public override void AddTriggers()
        {
            base.AddTrigger(EffectTrigger());

            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck or this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.ShuffleTwoSameCardFromTrashOrDestroyResponse), new TriggerType[] { TriggerType.ShuffleCardIntoDeck, TriggerType.DestroySelf });
        }

        private IEnumerator ShuffleTwoSameCardFromTrashOrDestroyResponse(PhaseChangeAction phaseChange)
        {
            MoveCardDestination obj = new MoveCardDestination(base.TurnTaker.Deck, true);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

            //...shuffle 2 cards with the same name from your trash into your deck...
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ShuffleCardIntoDeck, new LinqCardCriteria((Card c) => TwoOrMoreCopiesInTrash(c) && c.IsInTrash, "two cards with the same name"), storedResults, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
            if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
            {
                //Move second card
                coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ShuffleCardIntoDeck, new LinqCardCriteria((Card c) => c.Identifier == selectCardDecision.SelectedCard.Identifier && c.InstanceIndex != selectCardDecision.SelectedCard.InstanceIndex && c.IsInTrash, "two cards with the same name"), storedResults, false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //actually shuffle cards into deck
                IEnumerable<Card> cards = new Card[] { storedResults.FirstOrDefault().SelectedCard, storedResults.LastOrDefault().SelectedCard };
                coroutine = base.GameController.ShuffleLocation(this.TurnTaker.Deck);
                coroutine = base.GameController.ShuffleCardsIntoLocation(HeroTurnTakerController, cards, base.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }//...or this card is destroyed.
            else
            {
                coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card);
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

        private bool TwoOrMoreCopiesInTrash(Card c)
        {
            int num = (from card in base.TurnTaker.Trash.Cards
                       where card.Identifier == c.Identifier
                       select card).Count<Card>();
            return num >= 2;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class YourDarkestSecretsCardController : MythosUtilityCardController
    {
        public YourDarkestSecretsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override string DeckIdentifier
        {
            get
            {
                return MythosMindDeckIdentifier;
            }
        }

        public override IEnumerator Play()
        {
            //Discard the top card of each hero deck.
            List<MoveCardAction> discardedCards = new List<MoveCardAction>();
            IEnumerator coroutine = base.GameController.DiscardTopCardsOfDecks(this.DecisionMaker, (Location deck) => deck.IsHero, 1, storedResultsMove: discardedCards, showCards: (Card c) => true, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            foreach (MoveCardAction cardAction in discardedCards)
            {
                //{Mythos} deals each hero 1 infernal damage for each card in their hand that shares a keyword with the card discarded from their deck.
                Card card = cardAction.CardToMove;
                foreach (Card handCard in card.Owner.ToHero().Hand.Cards)
                {
                    if (base.GameController.GetAllKeywords(handCard).Intersect(base.GameController.GetAllKeywords(card)).Any())
                    {
                        Card target = card.Owner.CharacterCard;
                        if (!target.IsRealCard)
                        {
                            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                            coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.DealDamage, new LinqCardCriteria((Card c) => c.Owner == target.Owner && c.IsHeroCharacterCard), storedResults, false, cardSource: base.GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(coroutine);
                            }
                            target = storedResults.FirstOrDefault().SelectedCard;
                        }

                        coroutine = base.DealDamage(base.CharacterCard, target, 1, DamageType.Infernal, cardSource: base.GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                }
            }
            yield break;
        }
    }
}

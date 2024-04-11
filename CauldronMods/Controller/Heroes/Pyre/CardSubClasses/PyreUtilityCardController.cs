using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Pyre
{
    public abstract class PyreUtilityCardController : CardController
    {
        protected enum CustomMode
        {
            CardToIrradiate,
            PlayerToIrradiate,
            Unique
        }

        protected CustomMode CurrentMode;

        protected PyreUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected IEnumerator SelectAndIrradiateCardsInHand(HeroTurnTakerController decisionMaker, TurnTaker playerWithHand, int maxCards, int? minCards = null, List<SelectCardDecision> storedResults = null, Func<Card, bool> additionalCriteria = null)
        {
            
            if(additionalCriteria == null)
            {
                additionalCriteria = (Card c) => true;
            }
            Func<Card, bool> handCriteria = (Card c) => c != null && c.IsInHand;
            if(playerWithHand != null)
            {
                handCriteria = (Card c) => c != null && c.Location == playerWithHand.ToHero().Hand;
            }

            var fullCriteria = new LinqCardCriteria((Card c) => handCriteria(c) && !c.IsIrradiated() && additionalCriteria(c), $"non-{PyreExtensionMethods.Irradiated}");
            if(storedResults == null)
            {
                storedResults = new List<SelectCardDecision>();
            }
            if(minCards == null)
            {
                minCards = maxCards;
            }
            var oldMode = CurrentMode;
            CurrentMode = CustomMode.CardToIrradiate;
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(decisionMaker, fullCriteria, SelectionType.Custom, c => this.IrradiateCard(c), maxCards, false, minCards, storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            CurrentMode = oldMode;
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            string radIcon = "{{Rad}}";
            if (CurrentMode is CustomMode.CardToIrradiate)
            {
                return new CustomDecisionText($"Select a card to {radIcon}", $"deciding which card to {radIcon}.", $"Vote for which card to {radIcon}", $"card to {radIcon}");
            }
            else if (CurrentMode is CustomMode.PlayerToIrradiate)
            {
                return new CustomDecisionText($"Select a player to {radIcon} a card in their hand.", $"deciding whose hand to {radIcon} cards in.", $"Vote for which player's hand to {radIcon} cards from.", $"player's hand to {radIcon} cards from");
            }
            else if (CurrentMode is CustomMode.Unique)
            {
                return new CustomDecisionText($"Select a player to {radIcon} a card in their hand.", $"deciding whose hand to {radIcon} cards in.", $"Vote for which player's hand to {radIcon} cards from.", $"player's hand to {radIcon} cards from");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}

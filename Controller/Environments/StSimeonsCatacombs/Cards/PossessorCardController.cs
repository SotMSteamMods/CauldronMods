using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class PossessorCardController : GhostCardController
    {
        #region Constructors

        public PossessorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "TortureChamber")
        {

        }



        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //That hero may not play cards or use powers.
            base.CannotPlayCards((TurnTakerController ttc) => ttc != null && ttc.TurnTaker == base.GetCardThisCardIsNextTo().Owner, (Card c) => c.Owner == base.GetCardThisCardIsNextTo().Owner);
            base.CannotUsePowers((TurnTakerController ttc) => ttc != null && ttc.TurnTaker == base.GetCardThisCardIsNextTo().Owner);

            //At the start of that hero's turn, put 2 cards from their hand into play at random.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.GetCardThisCardIsNextTo().Owner, new Func<PhaseChangeAction, IEnumerator>(this.StartOfHeroTurnResponse), TriggerType.PutIntoPlay);
        }

        private IEnumerator StartOfHeroTurnResponse(PhaseChangeAction pca)
        {
            //put 2 cards from the hero this card is next to's hand into play at random.
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController,base.GetCardThisCardIsNextTo().Owner.ToHero().Hand, false, true, false, new LinqCardCriteria((Card c) => true), new int?(2), shuffleBeforehand: true);
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

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the most cards in hand.

            //find hero with the most cards in hand
            List<TurnTaker> storedMostCards = new List<TurnTaker>();
            IEnumerator coroutine = base.FindHeroWithMostCardsInHand(storedMostCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //select the hero with the most cards in hand, resolving ties
            IEnumerator coroutine2 = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => storedMostCards.Contains(c.Owner) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero target with the most cards in hand"), storedResults, isPutIntoPlay, decisionSources);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            yield break;
        }

        #endregion Methods
    }
}
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class TemporalReversalCardController : CardController
    {
        #region Constructors

        public TemporalReversalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //When this card enters play, place 1 card in play from each other deck back on top of that deck.
            IEnumerator coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController turnTakerController) => turnTakerController.IsHero || turnTakerController.IsVillain, MoveCardToDeckResponse);
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

        private IEnumerator MoveCardToDeckResponse(TurnTakerController turnTakerController)
        {
            IEnumerator coroutine = base.GameController.SelectAndReturnCards(this.DecisionMaker, new int?(1), new LinqCardCriteria((Card c) => !c.IsCharacter && c.Owner == turnTakerController.TurnTaker, "card in play"), false, true, false, null, cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        #endregion Methods
    }
}
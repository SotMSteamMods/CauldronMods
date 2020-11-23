using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class LivingGeometryCardController : CardController
    {
        #region Constructors

        public LivingGeometryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //At the end of the environment turn, play the top card of the environment deck and destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator> (this.EndOfTurnResponse), new TriggerType[]
            {
                TriggerType.PlayCard,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //play the top card of the environment deck and destroy this card.
            IEnumerator play = base.PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(pca);
            IEnumerator destroy = base.DestroyThisCardResponse(pca);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(play);
                yield return this.GameController.StartCoroutine(destroy);
            }
            else
            {
                base.GameController.ExhaustCoroutine(play);
                base.GameController.ExhaustCoroutine(destroy);
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, destroy a room card. 

            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsRoom, "room"), false, responsibleCard: base.Card, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Its replacement is selected randomly from the 5 room cards, not chosen by the players.


            IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.FindCard("StSimeonsCatacombs").UnderLocation, false, true, false, new LinqCardCriteria((Card c) => this.IsDefinitionRoom(c), "room"), new int?(1), shuffleBeforehand: true);
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

        private bool IsDefinitionRoom(Card card)
        {
            return card != null && card.Definition.Keywords.Contains("room");
        }

        #endregion Methods
    }
}
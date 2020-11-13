using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.BlackwoodForest
{
    public class OldBonesCardController : CardController
    {
        //==============================================================
        // When this card enters play, shuffle each trash pile other than the environment
        // and reveal a card from it. Put the revealed cards on top of each deck.
        // At the start of the environment turn destroy this card.
        //==============================================================

        public static string Identifier = "OldBones";

        public OldBonesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker,
                new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse),
                TriggerType.DestroySelf, null, false);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Shuffle each trash pile other than environment
            IEnumerator shuffleRoutine
                = base.DoActionToEachTurnTakerInTurnOrder(
                    turnTakerController => !turnTakerController.TurnTaker.IsEnvironment,
                    ShuffleTrashResponse);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleRoutine);
            }
        }

        private IEnumerator ShuffleTrashResponse(TurnTakerController turnTakerController)
        {
            TurnTaker turnTaker = turnTakerController.TurnTaker;
            HeroTurnTakerController decisionMaker = turnTaker.IsHero ? turnTakerController.ToHero() : this.DecisionMaker;


            // Shuffle trash pile
            IEnumerator shuffleTrashRoutine = base.GameController.ShuffleLocation(turnTaker.Trash);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleTrashRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleTrashRoutine);
            }

            // Reveal top card and place on TT's deck
            List<RevealCardsAction> revealCardsActions = new List<RevealCardsAction>();
            IEnumerator revealCardsRoutine = base.GameController.RevealCards(turnTakerController, turnTaker.Trash, 
                card => true, 1, revealCardsActions, RevealedCardDisplay.ShowRevealedCards);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            /*
            IEnumerator selectCardsFromLocationRoutine = base.GameController.SelectCardsFromLocationAndMoveThem(decisionMaker, turnTaker.Trash,
                1, CardsToMoveFromTrash,
                new LinqCardCriteria(c => c.Location == turnTaker.Trash, "trash"),
                list, shuffleAfterwards: false, cardSource: base.GetCardSource());

            List<MoveCardDestination> list = new List<MoveCardDestination>
            {
                new MoveCardDestination(turnTaker.Deck)
            };
            */

            yield break;
        }
    }
}
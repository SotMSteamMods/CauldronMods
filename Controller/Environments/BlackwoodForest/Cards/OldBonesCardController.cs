using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


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

        private const int CardsToMove = 1;

        public OldBonesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Destroy self at start of next env. turn
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker,
                base.DestroyThisCardResponse,
                TriggerType.DestroySelf);

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
            //HeroTurnTakerController decisionMaker = turnTaker.IsHero ? turnTakerController.ToHero() : this.DecisionMaker;

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

            IEnumerator revealCardsRoutine = base.GameController.MoveCards(this.TurnTakerController, 
                turnTaker.Trash, turnTaker.Deck, CardsToMove);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }
        }
    }
}
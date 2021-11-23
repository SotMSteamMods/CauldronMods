using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class InfernalElegyCardController : TheInfernalChoirUtilityCardController
    {
        public InfernalElegyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(ghostCardPlay, $"{Card.Title} has played a random ghost from the trash this turn.", $"{Card.Title} has not played a random ghost from the trash this turn.");
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsGhost(c), "ghost"));
        }

        private readonly string ghostCardPlay = "GhostCardPlay";

        public override IEnumerator Play()
        {
            var coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddTrigger<DestroyCardAction>(dca => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsEnvironment && !HasBeenSetToTrueThisTurn(ghostCardPlay), PlayRandomGhost, TriggerType.PutIntoPlay, TriggerTiming.After);
        }

        private IEnumerator PlayRandomGhost(GameAction action)
        {
            SetCardPropertyToTrueIfRealAction(ghostCardPlay);
            var ghostsInTrash = base.FindCardsWhere(c => c.Location == TurnTaker.Trash && IsGhost(c));
            if(ghostsInTrash.Count() == 0)
            {
                yield break;
            }
            var cardToPlay = ghostsInTrash.TakeRandomFirstOrDefault(Game.RNG);
            if (cardToPlay != null)
            {
                var coroutine = GameController.PlayCard(TurnTakerController, cardToPlay, true, actionSource: action, cardSource: GetCardSource());
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
}

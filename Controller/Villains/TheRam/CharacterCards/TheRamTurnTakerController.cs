using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheRam
{
    public class TheRamTurnTakerController : TurnTakerController
    {
        public TheRamTurnTakerController(TurnTaker tt, GameController gameController) : base(tt, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            if (CharacterCardController is TheRamCharacterCardController)
            {
                IEnumerator coroutine = HandleWinters(banish: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = MoveUpCloseToTrash();
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayCard(this, FindCardsWhere((Card c) => c.Identifier == "GrapplingClaw").FirstOrDefault(), cardSource: new CardSource(CharacterCardController));
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                var shuffleAction = new ShuffleCardsAction(new CardSource(CharacterCardController), this.TurnTaker.Deck);
                coroutine = GameController.DoAction(shuffleAction);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        private IEnumerator MoveUpCloseToTrash()
        {
            BulkMoveCardsAction upCloseToTrash = new BulkMoveCardsAction(GameController, FindCardsWhere((Card c) => c.Identifier == "UpClose"), TurnTaker.Trash, false, TurnTaker, false, false);
            return GameController.DoAction(upCloseToTrash);
        }

        private IEnumerator HandleWinters(bool banish = true)
        {
            yield break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;
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
                //"At the start of the game, put {TheRam}'s villain character cards into play, “Mechanical Juggernaut” side up.",
                IEnumerator coroutine;

                //"Search the villain deck for all copies of Up Close and put them in the trash. 
                coroutine = MoveUpCloseToTrash();
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Put Grappling Claw into play. Shuffle the villain deck."
                coroutine = GameController.PlayCard(this, FindCardsWhere((Card c) => c.Identifier == "GrapplingClaw").FirstOrDefault(), isPutIntoPlay: true, cardSource: new CardSource(CharacterCardController));
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
            else
            {
                //"At the start of the game, put {TheRam} and {AdmiralWinters}' villain character cards into play, “Amphibious Dreadnought” and “Dreadnought Pilot” sides up. 
                IEnumerator coroutine;

                //Search the villain deck for all copies of Up Close and put them in the trash. 
                coroutine = MoveUpCloseToTrash();
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Put 2 copies of Remote Mortar into play. Shuffle the villain deck."
                coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.Identifier == "RemoteMortar"), 2);
                if (UseUnityCoroutines)
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

        public bool IsAdmiralWintersHandled { get; private set; } = false;
        public IEnumerator HandleWintersEarly(bool banish = true)
        {
            IsAdmiralWintersHandled = true;
            Card winters = TurnTaker.FindCard("AdmiralWintersCharacter", true);
            if (winters == null || winters.Location != TurnTaker.OffToTheSide)
            {
                Log.Debug("Failed to find Admiral Winters");
                yield return GameController.DoNothing();
            }
            else
            {
                IEnumerator coroutine;
                if (banish)
                {
                    coroutine = GameController.MoveCard(this, winters, TurnTaker.InTheBox, cardSource: FindCardController(winters).GetCardSource());
                }
                else
                {
                    coroutine = GameController.PlayCard(this, winters, isPutIntoPlay: true, cardSource: FindCardController(winters).GetCardSource());
                }

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

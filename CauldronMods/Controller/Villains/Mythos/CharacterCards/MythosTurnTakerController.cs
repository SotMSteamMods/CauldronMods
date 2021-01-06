using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System.Linq;

namespace Cauldron.Mythos
{
    public class MythosTurnTakerController : TurnTakerController
    {
        public MythosTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //At the start of the game, put {Mythos}'s villain character cards into play, 'Insubstantial Rumor' side up.
            IEnumerator coroutine = base.GameController.BulkMoveCards(this, base.FindCardsWhere((Card c) => c.Owner == base.TurnTaker && c != base.CharacterCard), base.TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Search the villain deck for Dangerous Investigations and put it into play. Shuffle the villain deck.
            coroutine = base.PutCardIntoPlay("DangerousInvestigation");
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
    }
}

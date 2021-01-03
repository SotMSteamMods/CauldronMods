using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class OriphelTurnTakerController : TurnTakerController
    {
        public OriphelTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //"At the start of the game, put {Oriphel}'s villain character cards into play, 'Shardwalker' side up.",

            //"Reveal cards from the top of the villain deck until {H - 2} guardians are revealed and put them into play. 
            //Shuffle the villain deck."
            IEnumerator coroutine = PutCardsIntoPlay(new LinqCardCriteria(IsGuardian, "guardian"), H - 2, true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        protected bool IsGuardian(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "guardian");
        }
    }
}
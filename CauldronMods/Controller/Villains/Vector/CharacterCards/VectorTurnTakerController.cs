﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class VectorTurnTakerController : TurnTakerController
    {
        private const int StartingHp = 40;

        public VectorTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            if (!(base.CharacterCardController is VectorCharacterCardController))
            {
                yield break;
            }

            // At the start of the game, put {Vector}'s villain character cards into play, "Asymptomatic Carrier" side up, with 40 HP.
            IEnumerator routine = base.GameController.SetHP(this.CharacterCard, StartingHp, cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if(GameController.Game.IsChallenge)
            {
                //At the start of the game, put Supervirus under Vector. It does not count as being in play.
                Card virus = TurnTaker.FindCard("Supervirus");
                routine = GameController.MoveCard(this, virus, CharacterCard.UnderLocation, playCardIfMovingToPlayArea: false, cardSource: CharacterCardController.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
        }
    }
}

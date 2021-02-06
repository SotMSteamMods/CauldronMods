using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
    public class LadyOfTheWoodTurnTakerController : HeroTurnTakerController
    {
        public LadyOfTheWoodTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }



        public override IEnumerator StartGame()
        {
            TokenPool unlucky = TurnTaker.CharacterCard.TokenPools[0];
            TokenPool element = TurnTaker.CharacterCard.TokenPools[1];
            TurnTaker.CharacterCard.TokenPools[0] = element;
            TurnTaker.CharacterCard.TokenPools[1] = unlucky;
            yield break;
        }


    }
}
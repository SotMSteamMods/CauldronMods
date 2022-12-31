using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.TheStranger
{
    public class WastelandRoninTheStrangerPromoCardUnlockController : PromoCardUnlockController
    {

        public WastelandRoninTheStrangerPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.TheStranger", "WastelandRoninTheStrangerCharacter")
        {

        }

        private readonly string rogueFissionCascadeIdentifier = "RogueFissionCascade";
        private readonly string heartOfWandererIdentifier = "HeartOfTheWanderer";

        public override bool IsUnlockPossibleThisGame()
        {
            return AreInGame(new string[]
            {
                "Pyre",
                "Impact",
                "TheStranger",
                "Gargoyle",
                "Cricket",
                "FSCContinuanceWanderer"
            });
        }
       
        public override bool CheckForUnlock(GameAction action)
        {
            if (!IsCardEnterPlay(action))
                return IsUnlocked;

            bool condition1 = ((CardEntersPlayAction)action).CardEnteringPlay.Identifier == rogueFissionCascadeIdentifier;
            bool condition2 = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == heartOfWandererIdentifier).Any();

            if (condition1 && condition2)
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }

        private bool IsCardEnterPlay(GameAction gameAction)
        {
            if (gameAction.IsSuccessful)
            {
                return gameAction is CardEntersPlayAction;
            }
            return false;
        }



    }
}
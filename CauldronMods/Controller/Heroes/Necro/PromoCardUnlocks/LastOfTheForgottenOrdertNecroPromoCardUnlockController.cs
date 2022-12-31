using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Necro
{
    public class LastOfTheForgottenOrderNecroPromoCardUnlockController : PromoCardUnlockController
    {

        public LastOfTheForgottenOrderNecroPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Necro", "LastOfTheForgottenOrderNecroCharacter")
        {

        }

        public const string LastOfTheForgottenOrderNecroUnlockCondition1 = "LastOfTheForgottenOrderNecroUnlockCondition1";


        public override bool IsUnlockPossibleThisGame()
        {
            return AreInGame(new string[]
            {
                "Mythos",
                "Drift",
                "Terminus"
            }, new Dictionary<string, string>
            {
                {
                    "Drift",
                    "AllInGoodTimeDriftCharacter"
                }
            }) && !IsInGame("Necro") && HasFlagBeenSetToTrue(LastOfTheForgottenOrderNecroUnlockCondition1);
        }

        public override bool IsFlagPossibleThisGame()
        {
            return !HasFlagBeenSetToTrue(LastOfTheForgottenOrderNecroUnlockCondition1) && AreInGame(new string[]
            {
                "Necro",
                "Vanish",
                "TheKnight",
                "TangoOne",
                "LaComodora",
                "CatchwaterHarbor"
            }, new Dictionary<string, string>
            {
                {
                    "Necro",
                    "PastNecroCharacter"
                },
                {
                    "Vanish",
                    "PastVanishCharacter"
                },
                {
                    "TheKnight",
                    "PastTheKnightCharacter"
                },
                {
                    "TangoOne",
                    "PastTangoOneCharacter"
                }
            });
        }

        public override void CheckForFlags(GameAction action)
        {
         
            if (IsGameOverDefeat(action))
            {
                GameOverAction gameOver = action as GameOverAction;
                if(gameOver.EndingResult == EndingResult.EnvironmentDefeat && 
                    gameOver.CardSource.Card.Identifier == "LeftBehind" &&
                    WasLeftBehindOnNecro())
                {
                    SetFlag(LastOfTheForgottenOrderNecroUnlockCondition1, value: true);
                    ContinueCheckingForFlags = false;
                }
            }
        }

        private bool WasLeftBehindOnNecro()
        {
            Card leftBehind = FindCard("LeftBehind");
            if (leftBehind is null)
                return false;

            return leftBehind.Location.IsNextToCard && leftBehind.Location.OwnerCard.Identifier == "NecroCharacter";
        }

        public override bool CheckForUnlock(GameAction action)
        {

            if (!IsGameOver(action))
                return IsUnlocked;

            bool condition1 = (action as GameOverAction).ResultIsVictory;
            bool condition2 = Journal.DoesMostRecentDealDamageEntryMatchCriteria((DealDamageJournalEntry dd) => dd.SourceCard != null && dd.SourceCard.Identifier == "TerminusCharacter" && dd.TargetCard.Identifier == "MythosCharacter");
       
            if (condition1 && condition2)
            {
                IsUnlocked = true;

            }

            return IsUnlocked;
        }


    }
}
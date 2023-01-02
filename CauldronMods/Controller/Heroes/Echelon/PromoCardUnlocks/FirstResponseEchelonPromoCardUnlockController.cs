using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Echelon
{
    public class FirstResponseEchelonPromoCardUnlockController : PromoCardUnlockController
    {

        public FirstResponseEchelonPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Echelon", "FirstResponseEchelonCharacter")
        {

        }

        public const string FirstResponseEchelonUnlockCondition1 = "FirstResponseEchelonUnlockCondition1";
        public const string FirstResponseEchelonUnlockCondition2 = "FirstResponseEchelonUnlockCondition2";
        public const string FirstResponseEchelonUnlockCondition3 = "FirstResponseEchelonUnlockCondition3";
        public const string FirstResponseEchelonUnlockCondition4 = "FirstResponseEchelonUnlockCondition4";
        public const string FirstResponseEchelonUnlockCondition5 = "FirstResponseEchelonUnlockCondition5";

        private bool DidEchelonPlayFirstResponderWhileKestrelWasOut { get; set; } = false;

        Dictionary<string, string> conditionEnvironmentDictionary = new Dictionary<string, string>()
        {
            { FirstResponseEchelonUnlockCondition1, "WindmillCity" },
            { FirstResponseEchelonUnlockCondition2, "SuperstormAkela" },
            { FirstResponseEchelonUnlockCondition3, "Megalopolis" },
            { FirstResponseEchelonUnlockCondition4, "RookCity" },
            { FirstResponseEchelonUnlockCondition5, "Mordengrad" }
        };


        public override bool IsUnlockPossibleThisGame()
        {
            string missingCondition = AreFourConditionsMet();
            if (missingCondition is null)
                return false;
            return AreInGame(new string[]
            {
                "Vanish",
                "Echelon",
                "DocHavoc",
                "Cricket",
                "Cypher",
                conditionEnvironmentDictionary[missingCondition]
            });
        }

        private string AreFourConditionsMet()
        {
            Dictionary<string, bool> conditions = new Dictionary<string, bool>();
            foreach(string condition in conditionEnvironmentDictionary.Keys)
            {
                conditions.Add(condition, HasFlagBeenSetToTrue(condition));
            }

            IEnumerable<string> incompleteConditions = conditions.Where(c => c.Value == false).Select(c => c.Key);
            if (!incompleteConditions.Any() || incompleteConditions.Count() > 1)
                return null;

            return incompleteConditions.First();


        }

        public override bool IsFlagPossibleThisGame()
        {
            return AreInGame(new string[]
            {
                "Vanish",
                "Echelon",
                "DocHavoc",
                "Cricket",
                "Cypher"
            }) && conditionEnvironmentDictionary.Any(c => IsInGame(c.Value));
        }

        public override void CheckForFlags(GameAction action)
        {
            if (!DidEchelonPlayFirstResponderWhileKestrelWasOut)
            {
                CheckIfEchelonPlayedFirstResponderWhileKestrelIsOut(action);
            }


            if (IsGameOverVictory(action))
            {
                bool condition1 = DidEchelonPlayFirstResponderWhileKestrelWasOut;
                if (condition1)
                {
                    string environmentIdentifier = GameController.Game.EnvironmentTurnTakers.First().Identifier;
                    string unlockConditionKey = conditionEnvironmentDictionary.First(c => c.Value == environmentIdentifier).Key;
                    SetFlag(unlockConditionKey, value: true);
                    ContinueCheckingForFlags = false;
                    Log.Debug("Flag for " + environmentIdentifier + " has been achieved");
                }
            }
        }

       

        public override bool CheckForUnlock(GameAction action)
        {
            if(!DidEchelonPlayFirstResponderWhileKestrelWasOut)
            {
                CheckIfEchelonPlayedFirstResponderWhileKestrelIsOut(action);
            }

            if (!IsGameOver(action))
                return IsUnlocked;

            bool condition1 = (action as GameOverAction).ResultIsVictory;
            bool condition2 = DidEchelonPlayFirstResponderWhileKestrelWasOut;


            if (condition1 && condition2)
            {
                IsUnlocked = true;

            }

            return IsUnlocked;
        }

        private void CheckIfEchelonPlayedFirstResponderWhileKestrelIsOut(GameAction action)
        {
            if (action is PlayCardAction)
            {
                PlayCardAction playCardAction = action as PlayCardAction;
                if (playCardAction.WasCardPlayed && (playCardAction.CardToPlay.Identifier == "FirstResponder" && AreAnyInPlay((Card c) => c.Identifier == "TheKestrelMarkII")))
                {
                    Log.Debug("FirstResponseEchelonPromoUnlock play card condition has been met!");
                    DidEchelonPlayFirstResponderWhileKestrelWasOut = true;
                }
            }
        }



    }
}
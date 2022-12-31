using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System;

namespace Cauldron.Cricket
{
    public class RenegadeCricketPromoCardUnlockController : PromoCardUnlockController
    {

        public RenegadeCricketPromoCardUnlockController(GameController gameController) : base(gameController, "Cauldron.Cricket", "RenegadeCricketCharacter")
        {

        }

        private readonly string cricketIdentifier = "CricketCharacter";
        private readonly string pythonIdentifier = "Python";

        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame("Dynamo") && IsInGame("Cricket") && IsInGame("WindmillCity") && FindHeroPosition("Cricket") == 1;
        }
       
        public override bool CheckForUnlock(GameAction action)
        { 
            if (!IsGameOver(action))
                return IsUnlocked;

            EndingResult[] defeatResults = new EndingResult[] { EndingResult.HeroesDestroyedDefeat, EndingResult.EnvironmentDefeat, EndingResult.AlternateDefeat };
            bool condition1 = defeatResults.Contains((action as GameOverAction).EndingResult);
            bool condition2 = CheckCricketPythonReduction();
            bool condition3 = DidDealFinalDamage((Card c) => IsResponder(c), cricketIdentifier);

            if (condition1 && condition2 && condition3)
            {
                IsUnlocked = true;
            }

            return IsUnlocked;
        }

        private bool CheckCricketPythonReduction()
        {
            IEnumerable<ReduceDamageJournalEntry> reduceDamageJournalEntries = from rde in base.Journal.ReduceDamageEntries(rde => true)
                                                             where rde.CardSource.Identifier == pythonIdentifier && rde.DamageSource.Identifier == cricketIdentifier
                                                             select rde;
            return reduceDamageJournalEntries.Any();
            
        }

        public static readonly string ResponderKeyword = "responder";
        protected bool IsResponder(Card card)
        {
            return card.DoKeywordsContain(ResponderKeyword);
        }

    }
}
using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using Cauldron;
using System.Collections.Generic;

namespace CauldronTests.Random
{
    [TestFixture()]
    public class BaccaratRandomTests : RandomGameTest
    {
        [Test]
        public void TestBaccarat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestBaccarat_RandomOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSorrowsBaccarat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSorrowsBaccarat_RandomOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSwords_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSwordsBaccarat_RandomOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastBaccarat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/PastBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastBaccarat_RandomOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/PastBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestBaccarat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestBaccarat_ReasonableOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSorrowsBaccarat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSorrowsBaccarat_ReasonableOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSwords_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAceOfSwordsBaccarat_ReasonableOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastBaccarat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/PastBaccaratCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastBaccarat_ReasonableOblivaeon()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Baccarat/PastBaccaratCharacter" });
            RunGame(gameController);
        }

    }
}
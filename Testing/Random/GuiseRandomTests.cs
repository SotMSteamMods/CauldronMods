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
    public class GuiseRandomTests : RandomGameTest
    {
        #region Just Guise
        [Test]
        public void TestGuise_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuise_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuise_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuise_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Baccarat
        [Test]
        public void TestGuiseAndBaccarat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndBaccarat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndBaccarat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndBaccarat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Baccarat" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Cricket
        [Test]
        public void TestGuiseAndCricket_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Cricket" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndCricket_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Cricket" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndCricket_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Cricket" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndCricket_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Cricket" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Cypher
        [Test]
        public void TestGuiseAndCypher_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Cypher" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndCypher_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Cypher" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndCypher_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Cypher" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndCypher_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Cypher" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + DocHavoc
        [Test]
        public void TestGuiseAndDocHavoc_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.DocHavoc" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndDocHavoc_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.DocHavoc" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndDocHavoc_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.DocHavoc" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndDocHavoc_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.DocHavoc" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Drift
        [Test]
        public void TestGuiseAndDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Drift" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Drift" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Drift" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Drift" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Echelon
        [Test]
        public void TestGuiseAndEchelon_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Echelon" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndEchelon_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Echelon" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndEchelon_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Echelon" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndEchelon_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Echelon" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Gargoyle
        [Test]
        public void TestGuiseAndGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Gargoyle" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Gargoyle" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Gargoyle" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Gargoyle" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Gyrosaur
        [Test]
        public void TestGuiseAndGyrosaur_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Gyrosaur" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndGyrosaur_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Gyrosaur" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndGyrosaur_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Gyrosaur" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndGyrosaur_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Gyrosaur" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Impact
        [Test]
        public void TestGuiseAndImpact_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Impact" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndImpact_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Impact" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndImpact_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Impact" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndImpact_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Impact" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + LadyOfTheWood
        [Test]
        public void TestGuiseAndLadyOfTheWood_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.LadyOfTheWood" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndLadyOfTheWood_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.LadyOfTheWood" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndLadyOfTheWood_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.LadyOfTheWood" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndLadyOfTheWood_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.LadyOfTheWood" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + MagnificentMara
        [Test]
        public void TestGuiseAndMagnificentMara_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.MagnificentMara" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndMagnificentMara_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.MagnificentMara" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndMagnificentMara_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.MagnificentMara" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndMagnificentMara_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.MagnificentMara" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Malichae
        [Test]
        public void TestGuiseAndMalichae_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Malichae" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndMalichae_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Malichae" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndMalichae_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Malichae" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndMalichae_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Malichae" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Necro
        [Test]
        public void TestGuiseAndNecro_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Necro" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndNecro_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Necro" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndNecro_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Necro" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndNecro_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Necro" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Pyre
        [Test]
        public void TestGuiseAndPyre_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Pyre" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndPyre_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Pyre" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndPyre_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Pyre" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndPyre_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Pyre" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Quicksilver
        [Test]
        public void TestGuiseAndQuicksilver_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Quicksilver" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndQuicksilver_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Quicksilver" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndQuicksilver_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Quicksilver" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndQuicksilver_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Quicksilver" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Starlight
        [Test]
        public void TestGuiseAndStarlight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Starlight" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndStarlight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Starlight" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndStarlight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Starlight" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndStarlight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Starlight" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + TangoOne
        [Test]
        public void TestGuiseAndTangoOne_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.TangoOne" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndTangoOne_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.TangoOne" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTangoOne_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.TangoOne" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTangoOne_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.TangoOne" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Terminus
        [Test]
        public void TestGuiseAndTerminus_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Terminus" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndTerminus_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Terminus" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTerminus_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Terminus" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTerminus_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Terminus" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + TheKnight
        [Test]
        public void TestGuiseAndTheKnight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.TheKnight" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndTheKnight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.TheKnight" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTheKnight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.TheKnight" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTheKnight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.TheKnight" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + TheStranger
        [Test]
        public void TestGuiseAndTheStranger_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.TheStranger" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndTheStranger_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.TheStranger" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTheStranger_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.TheStranger" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTheStranger_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.TheStranger" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Titan
        [Test]
        public void TestGuiseAndTitan_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Titan" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndTitan_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Titan" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTitan_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Titan" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndTitan_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Titan" });
            RunGame(gameController);
        }

        #endregion

        #region Guise + Vanish
        [Test]
        public void TestGuiseAndVanish_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Vanish" });
            RunGame(gameController);
        }

        [Test]
        public void TestGuiseAndVanish_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise", "Cauldron.Vanish" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndVanish_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Vanish" });
            RunGame(gameController);
        }

        [Test]
        public void TestCompletionistGuiseAndVanish_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Guise/CompletionistGuiseCharacter", "Cauldron.Vanish" });
            RunGame(gameController);
        }

        #endregion
    }
}
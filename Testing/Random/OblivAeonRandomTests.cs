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
    public class OblivAeonRandomTests : RandomGameTest
    {
        #region Just a Random OblivAeon Game with all Cauldron
        [Test]
        public void TestOblivAeon_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.BlackwoodForest" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeon_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments);
            RunGame(gameController);
        }
        #endregion
        #region OblivAeon + BlackwoodForest
        [Test]
        public void TestOblivAeonBlackwoodForest_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.BlackwoodForest" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonBlackwoodForest_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.BlackwoodForest" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + CatchwaterHarbor
        [Test]
        public void TestOblivAeonCatchwaterHarbor_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.CatchwaterHarbor" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonCatchwaterHarbor_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.CatchwaterHarbor" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + DungeonsOfTerror
        [Test]
        public void TestOblivAeonDungeonsOfTerror_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.DungeonsOfTerror" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonDungeonsOfTerror_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.DungeonsOfTerror" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + FSCContinuanceWanderer
        [Test]
        public void TestOblivAeonFSCContinuanceWanderer_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.FSCContinuanceWanderer" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonFSCContinuanceWanderer_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.FSCContinuanceWanderer" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + HalberdExperimentalResearchCenter
        [Test]
        public void TestOblivAeonHalberdExperimentalResearchCenter_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.HalberdExperimentalResearchCenter" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonHalberdExperimentalResearchCenter_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.HalberdExperimentalResearchCenter" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + NightloreCitadel
        [Test]
        public void TestOblivAeonNightloreCitadel_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.NightloreCitadel" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonNightloreCitadel_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.NightloreCitadel" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + Northspar
        [Test]
        public void TestOblivAeonNorthspar_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.Northspar" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonNorthspar_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.Northspar" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + OblaskCrater
        [Test]
        public void TestOblivAeonOblaskCrater_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.OblaskCrater" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonOblaskCrater_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.OblaskCrater" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + StSimeonsCatacombs
        [Test]
        public void TestOblivAeonStSimeonsCatacombs_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.StSimeonsCatacombs" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonStSimeonsCatacombs_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.StSimeonsCatacombs" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + SuperstormAkela
        [Test]
        public void TestOblivAeonSuperstormAkela_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.SuperstormAkela" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonSuperstormAkela_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.SuperstormAkela" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + TheChasmOfAThousandNights
        [Test]
        public void TestOblivAeonTheChasmOfAThousandNights_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.TheChasmOfAThousandNights" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonTheChasmOfAThousandNights_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.TheChasmOfAThousandNights" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + TheCybersphere
        [Test]
        public void TestOblivAeonTheCybersphere_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.TheCybersphere" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonTheCybersphere_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.TheCybersphere" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + TheWanderingIsle
        [Test]
        public void TestOblivAeonTheWanderingIsle_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.TheWanderingIsle" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonTheWanderingIsle_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.TheWanderingIsle" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + VaultFive
        [Test]
        public void TestOblivAeonVaultFive_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.VaultFive" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonVaultFive_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.VaultFive" });
            RunGame(gameController);
        }

        #endregion
        #region OblivAeon + WindmillCity
        [Test]
        public void TestOblivAeonWindmillCity_Random()
        {
            GameController gameController = SetupRandomOblivAeonGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.WindmillCity" });
            RunGame(gameController);
        }

        [Test]
        public void TestOblivAeonWindmillCity_Reasonable()
        {
            GameController gameController = SetupRandomOblivAeonGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useEnvironments: new List<string> { "Cauldron.WindmillCity" });
            RunGame(gameController);
        }

        #endregion
    }
}
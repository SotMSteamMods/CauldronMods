using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Starlight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class NightloreCouncilStarlightTests : BaseTest
    {
        #region StarlightHelperFunctions
        protected HeroTurnTakerController starlight { get { return FindHero("Starlight"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(starlight.CharacterCard, 1);
            DealDamage(villain, starlight, 2, DamageType.Melee);
        }

        protected Card terra { get { return GetCard("StarlightOfTerraCharacter"); } }
        protected Card asheron { get { return GetCard("StarlightOfAsheronCharacter"); } }
        protected Card cryos { get { return GetCard("StarlightOfCryosFourCharacter"); } }

        private CardController MakeCardWithActivator(Type baseType, Card baseCard, TurnTakerController ttc)
        {
            var newObj = Activator.CreateInstance(baseType, baseCard, ttc);
            if (newObj is CardController)
            {
                return (CardController)newObj;
            }
            return null;
        }
        #endregion
        [Test()]
        public void TestGenesisLoads()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "Megalopolis" }, false, promoDict);

            Assert.AreEqual(4, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(starlight);
            Assert.IsInstanceOf(typeof(GenesisStarlightCharacterCardController), starlight.CharacterCardController);

            Assert.AreEqual(29, starlight.CharacterCard.HitPoints);
            Assert.AreEqual("Starlight: Genesis", starlight.CharacterCard.Title);

            Assert.AreEqual((starlight.IncapacitationCardController).Card.Title, "Starlight: Genesis");
        }
        [Test()]
        public void TestGuiseCanGetOtherStarlight()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            promoDict["Guise"] = "CompletionistGuiseCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Guise", "Legacy", "Megalopolis" }, false, promoDict);
            StartGame();

            Assert.AreEqual(guise.CharacterCard.Title, "Completionist Guise");
            UsePower(guise);

        }
        [Test()]
        public void TestNightloreCouncilLoads()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            nightloreDict["Legacy"] = "GreatestLegacyCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "Megalopolis" }, false, nightloreDict);

            Assert.AreEqual(4, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(starlight);
            Card starlightInstructions = starlight.TurnTaker.FindCard("StarlightCharacter", realCardsOnly: false);
            Assert.IsNotNull(starlightInstructions);
            Assert.IsInstanceOf(typeof(NightloreCouncilStarlightCharacterCardController), FindCardController(starlightInstructions));

            Assert.IsFalse(starlightInstructions.IsRealCard);
            AssertNumberOfCardsInPlay(starlight, 3);
        }
        [Test()]
        public void TestNightloreCouncilIndividualsLoad()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            var starlightInstructions = starlight.TurnTaker.FindCard("StarlightCharacter", realCardsOnly: false);

            Assert.IsNotNull(starlightInstructions);
            Assert.AreEqual(starlightInstructions.PromoIdentifierOrIdentifier, "NightloreCouncilStarlightCharacter");
            StartGame();

            AssertNotIncapacitatedOrOutOfGame(starlight);

            Assert.AreEqual(13, terra.HitPoints);
            Assert.AreEqual(12, asheron.HitPoints);
            Assert.AreEqual(15, cryos.HitPoints);

            Assert.AreEqual(terra.Title, "Starlight of Terra");
            Assert.AreEqual(asheron.Title, "Starlight of Asheron");
            Assert.AreEqual(cryos.Title, "Starlight of Cryos-4");

            Assert.IsNull(starlight.CharacterCard);
            AssertNumberOfCardsInPlay(starlight, 3);
        }
        [Test()]
        public void TestNightloreCouncilTerraTrigger()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();

            Card constellation = GetCard("AncientConstellationA");
            DecisionSelectCard = terra;
            PlayCard(constellation);

            AssertNextToCard(constellation, terra);
            AssertNumberOfCardsNextToCard(terra, 1);

            DecisionSelectCard = null;
            SetHitPoints(terra, 10);
            SetHitPoints(asheron, 10);
            SetHitPoints(cryos, 10);

            QuickHPStorage(terra, asheron, cryos);

            GoToStartOfTurn(starlight);
            QuickHPCheck(1, 1, 1);

            DestroyCard(constellation);

            GoToStartOfTurn(starlight);
            QuickHPCheck(0, 0, 0);
        }
        [Test()]
        public void TestNightloreCouncilAsheronTrigger()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            var m = DamageType.Melee;
            Card constellation = GetCard("AncientConstellationA");
            Card mdp = GetMobileDefensePlatform().Card;
            DecisionSelectCard = asheron;
            PlayCard(constellation);
            DecisionSelectCard = terra;
            PlayCard("AncientConstellationB");
            DecisionSelectCard = idealist;
            PlayCard("AncientConstellationC");
            QuickHPStorage(mdp, asheron, idealist);
            DealDamage(asheron, mdp, 1, m);
            DealDamage(asheron, asheron, 1, m);
            DealDamage(asheron, idealist, 1, m);
            QuickHPCheck(-2, -1, -1);
            DealDamage(terra, mdp, 1, m);
            DealDamage(terra, asheron, 1, m);
            DealDamage(terra, idealist, 1, m);
            QuickHPCheck(-1, -2, -1); //nemesis bonus!
            DestroyCard(constellation);
            DealDamage(asheron, mdp, 1, m);
            DealDamage(asheron, asheron, 1, m);
            DealDamage(asheron, idealist, 1, m);
            QuickHPCheck(-1, -1, -1);
        }
        [Test()]
        public void TestNightloreCouncilCryosTrigger()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();

            DiscardAllCards(starlight); //so we don't accidentally play a card in hand
            DecisionSelectCards = new List<Card> { mainstay, terra, baron.CharacterCard, cryos };
            QuickHandStorage(starlight);
            PlayCard("AncientConstellationA");
            PlayCard("AncientConstellationB");
            PlayCard("AncientConstellationC");
            QuickHandCheck(0);
            PlayCard("AncientConstellationD");
            QuickHandCheck(1);
            
        }
    }
}
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
    public class StarlightVariantTests : CauldronBaseTest
    {
        #region StarlightHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(starlight.CharacterCard, 1);
            DealDamage(villain, starlight, 2, DamageType.Melee);
        }

        protected List<Card> EachStarlight { get { return new List<Card> { terra, asheron, cryos }; } }
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

            Assert.Ignore("The game can't get mod heroes out of the box until Handelabra makes it possible.");
            //this doesn't work yet, seems like a Handelabra issue

        }
        [Test()]
        public void TestNightloreCouncilLoads()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            nightloreDict["Legacy"] = "AmericasGreatestLegacyCharacter";
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
        [Test()]
        public void TestNightloreCouncilCelestialAuraTriggersOnAnyStarlightDamage()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            var m = DamageType.Melee;

            var targets = new List<Card> { mainstay, terra, mdp, idealist };
            DecisionSelectCards = targets;

            SetHitPoints(mainstay, 2);
            SetHitPoints(terra, 2);
            SetHitPoints(mdp, 5);

            PlayCard("AncientConstellationA");
            PlayCard("AncientConstellationB");
            PlayCard("AncientConstellationC");
            PlayCard("CelestialAura");

            QuickHPStorage(mainstay, terra, mdp, idealist);
            foreach (Card source in EachStarlight)
            {
                foreach (Card target in targets)
                {
                    DealDamage(source, target, 1, m);
                }
                QuickHPCheck(1, source == terra ? 1 : 2, -1, -1);
            }
        }
        [Test()]
        public void TestNightloreCouncilCelestialAuraPowerUsableWithAny()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            PlayCard("CelestialAura");
            QuickHPStorage(mdp);
            QuickHandStorage(starlight);

            foreach (Card character in EachStarlight)
            {
                DecisionSelectCards = new List<Card> { character, mdp };
                foreach (Card otherchar in EachStarlight)
                {
                    if (otherchar != character)
                    {
                        DecisionSelectCardsIndex = 0;
                        AssertNotDamageSource(otherchar);
                        UsePower("CelestialAura");
                    }
                }
                QuickHPCheck(-2);
                QuickHandCheck(2);
            }
        }
        [Test()]
        public void TestNightloreCouncilRetreatPlaysNextToAStarlight()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card retreat = GetCard("RetreatIntoTheNebula");
            DecisionSelectCard = terra;
            PlayCard(retreat);
            AssertNextToCard(retreat, terra);
        }
        [Test()]
        public void TestNightloreCouncilRetreatProtectsOnlyStarlightItIsNextTo()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card retreat = GetCard("RetreatIntoTheNebula");
            DecisionSelectCard = terra;
            PlayCard(retreat);

            QuickHPStorage(terra, asheron, mainstay);
            DealDamage(baron, terra, 3, DamageType.Melee);
            DealDamage(baron, asheron, 3, DamageType.Melee);
            DealDamage(baron, mainstay, 3, DamageType.Melee);
            QuickHPCheck(-1, -3, -3);

            DecisionSelectCard = asheron;
            DestroyCard(retreat);
            PlayCard(retreat);

            DealDamage(baron, terra, 3, DamageType.Melee);
            DealDamage(baron, asheron, 3, DamageType.Melee);
            DealDamage(baron, mainstay, 3, DamageType.Melee);
            QuickHPCheck(-3, -1, -3);
        }
        [Test]
        public void TestNightloreCouncilRetreatProtectsCorrectlyWithShenanigans()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Guise", "TheSentinels", "FreedomTower" }, false, nightloreDict);

            StartGame();
            Card retreat = GetCard("RetreatIntoTheNebula");
            DecisionSelectCard = terra;
            PlayCard(retreat);
            PlayCard("CaspitsPlayground");
            DecisionSelectCard = retreat;
            PlayCard("LemmeSeeThat");

            //now that Guise is stealing Retreat Into the Nebula's effect, let's see if it's protecting who it should
            DecisionSelectCard = null;
            List<Card> targets = new List<Card> { terra, asheron, guise.CharacterCard, mainstay };
            QuickHPStorage(terra, asheron, guise.CharacterCard, mainstay);
            foreach (Card target in targets)
            {
                DealDamage(baron, target, 3, DamageType.Melee);
            }
            QuickHPCheck(-3, -3, -1, -3);
        }
        //possible fail case: Caspit's Playground out, Guise has LemmeSeeThat on Retreat, damage goes to Mainstay
        [Test()]
        public void TestNightloreCouncilRetreatPicksCharacterWhenPlayedOddly()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card retreat = GetCard("RetreatIntoTheNebula");
            DecisionSelectCard = terra;

            PutInHand(legacy, retreat);
            PlayCardFromHand(legacy, "RetreatIntoTheNebula");
            AssertNextToCard(retreat, terra);

            PutInHand(sentinels, retreat);
            PlayCardFromHand(sentinels, "RetreatIntoTheNebula");
            AssertNextToCard(retreat, terra);
        }
        [Test()]
        public void TestNightloreCouncilIncapsOnlyWhenAllThreeCharactersDo()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();

            foreach (Card charCard in EachStarlight)
            {
                AssertNotIncapacitatedOrOutOfGame(starlight);
                DealDamage(baron, charCard, 40, DamageType.Melee);
            }

            AssertIncapacitated(starlight);
        }
        [Test]
        public void TestNightloreCouncilAsheronPowerDamage()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //don't want a stray select-constellation-to-play to mess things up
            DiscardAllCards(starlight);

            QuickHPStorage(mdp);

            foreach (Card character in EachStarlight)
            {
                DecisionSelectCards = new List<Card> { character, mdp };
                foreach (Card otherchar in EachStarlight)
                {
                    if (otherchar != character)
                    {
                        DecisionSelectCardsIndex = 0;
                        AssertNotDamageSource(otherchar);
                        UsePower(asheron);
                    }
                }
                QuickHPCheck(-2);
            }
        }
        [Test]
        public void TestNightloreCouncilAsheronPowerPlay()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            //avoid stray constellations messing up the test
            DiscardAllCards(starlight);

            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");
            PutInHand(new Card[2] { constA, constB });

            DecisionSelectCards = new List<Card> { constA, asheron, asheron, mdp };
            QuickHPStorage(mdp);
            QuickHandStorage(starlight);
            UsePower(asheron);
            //constellation play should happen before the damage, giving Asheron +1
            QuickHPCheck(-2);
            QuickHandCheck(-1);

            DecisionSelectCards = new List<Card> { baron.CharacterCard, terra, mdp };
            DecisionSelectCardsIndex = 0;
            AssertMaxNumberOfDecisions(3);
            //should not have a choice whether to play next constellation, only where
            //then damage source and damage target
            UsePower(asheron);
            QuickHPCheck(-1);
            QuickHandCheck(-1);
        }
        [Test]
        public void TestNightloreCouncilAsheronPowerIsActualPlay()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            //avoid stray constellations messing up the test
            DiscardAllCards(starlight);

            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");
            PutInHand(new Card[2] { constA, constB });

            PutIntoPlay("HostageSituation");

            QuickHandStorage(starlight);
            UsePower(asheron);
            QuickHandCheck(0);
        }
        [Test]
        public void TestNightloreCouncilTerraPowerBasic()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            //just in case we start with all copies of Exodus in hand
            MoveCards(starlight, (Card c) => c.Identifier == "Exodus", starlight.TurnTaker.Deck);

            int startingHand = starlight.NumberOfCardsInHand;

            AssertMaxNumberOfDecisions(1);
            QuickHandStorage(starlight);
            AssertNumberOfChoicesInNextDecision(startingHand);
            QuickShuffleStorage(starlight);

            UsePower(terra);
            QuickHandCheck(0); //-1 discard, +1 exodus-in-hand
            try
            {
                GetCardFromHand("Exodus");
            }
            catch
            {
                Assert.Fail("Could not find an Exodus in Starlight's hand.");
            }
            QuickShuffleCheck(1);
        }
        [Test]
        public void TestNightloreCouncilTerraPowerNoDiscardOrNoFind()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            //just in case we start with all copies of Exodus in hand
            MoveCards(starlight, (Card c) => c.Identifier == "Exodus", starlight.TurnTaker.Deck);
            DiscardAllCards(starlight);

            int startingTrashSize = starlight.TurnTaker.Trash.Cards.Count();
            AssertNumberOfCardsInHand(starlight, 0);
            QuickShuffleStorage(starlight);

            //at all times we should have an auto-decided outcome
            AssertMaxNumberOfDecisions(0);

            for (int i = 0; i < 4; i++)
            {
                UsePower(terra);
                //first use should not discard, as there are no cards in hand at that time
                AssertNumberOfCardsInTrash(starlight, startingTrashSize + i);
                AssertNumberOfCardsInHand(starlight, 1);
                QuickShuffleCheck(1);
            }
            UsePower(terra);
            AssertNumberOfCardsInHand(starlight, 0);
            AssertNumberOfCardsInTrash(starlight, startingTrashSize + 4);
            QuickShuffleCheck(1);
        }
        [Test]
        public void TestNightloreCouncilCryosPowerBasic()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");
            PutInTrash(new Card[2] { constA, constB });
            DecisionSelectCards = new List<Card> { constA, mainstay, idealist };

            QuickHPStorage(cryos);
            UsePower(cryos);
            QuickHPCheck(-2);
            AssertNumberOfCardsInTrash(starlight, 0);
            AssertIsInPlay(constA, constB);
        }
        [Test]
        public void TestNightloreCouncilCryosPowerNotEnoughConstellations()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");

            DecisionSelectCards = new List<Card> { mainstay, idealist };

            QuickHPStorage(cryos);
            AssertNextMessages("There were no playable constellations in the trash.", "There were no more playable constellations in the trash.");
            UsePower(cryos);
            QuickHPCheck(-2);
            PutInTrash(constA);
            UsePower(cryos);
            QuickHPCheck(-2);
            AssertIsInPlay(constA);
        }
        [Test()]
        public void TestNightloreCouncilCryosPowerIsActualPlay()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();

            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");
            PutInTrash(new Card[2] { constA, constB });

            PutIntoPlay("HostageSituation");

            UsePower(cryos);
            AssertNumberOfCardsInTrash(starlight, 2);
        }
        [Test]
        public void TestNightloreCouncilCryosPowerRequiresSelfDamage()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();

            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");
            PutInTrash(new Card[2] { constA, constB });

            PutIntoPlay("AlchemicalRedirection");
            UsePower(cryos);
            AssertNumberOfCardsInTrash(starlight, 2);
        }
        [Test]
        public void TestNightloreCouncilIncap1()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            foreach (Card character in EachStarlight)
            {
                DealDamage(baron, character, 20, DamageType.Melee);
            }

            AssertIncapacitated(starlight);
            PutInHand("FleshToIron");

            AssertIncapLetsHeroPlayCard(starlight, 0, scholar, "FleshToIron");
        }
        [Test]
        public void TestNightloreCouncilIncap2()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "Megalopolis" }, false, nightloreDict);

            StartGame();
            foreach (Card character in EachStarlight)
            {
                DealDamage(baron, character, 20, DamageType.Melee);
            }

            AssertIncapacitated(starlight);

            AssertIncapLetsHeroUsePower(starlight, 1, scholar);
        }
        [Test]
        public void TestNightloreCouncilIncap3()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "TheScholar", "TheSentinels", "TheCourtOfBlood" }, false, nightloreDict);

            StartGame();
            foreach (Card character in EachStarlight)
            {
                DealDamage(baron, character, 20, DamageType.Melee);
            }

            AssertIncapacitated(starlight);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card vamp = PlayCard("BloodCountessBathory");

            UseIncapacitatedAbility(starlight, 2);
            QuickHPStorage(vamp);

            DecisionSelectDamageType = DamageType.Infernal;
            DealDamage(scholar, vamp, 1, DamageType.Radiant);
            QuickHPCheck(-1);
            DealDamage(scholar, vamp, 1, DamageType.Radiant);
            QuickHPCheck(-2);

            PlayCard("UnhallowedHalls");
            UseIncapacitatedAbility(starlight, 2);
            DealDamage(scholar, vamp, 1, DamageType.Radiant);
            QuickHPCheck(-2);
            DealDamage(scholar, vamp, 1, DamageType.Radiant);
            QuickHPCheck(0);

        }
        [Test]
        public void TestGenesisPowerBasic()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            Card birds = PlayCard("HuginnAndMuninn");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card traffic = PlayCard("TrafficPileup");
            QuickHPStorage(starlight.CharacterCard, legacy.CharacterCard, harpy.CharacterCard, birds, mdp, traffic);

            UsePower(starlight);

            QuickHPCheck(-1, -1, -1, -1, 0, 0);
            AssertNumberOfCardsInPlay(starlight, 3);
        }
        [Test]
        public void TestGenesisPowerDoesNotRequireDamage()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            Card birds = PlayCard("HuginnAndMuninn");
            PlayCard("AppliedNumerology");
            DecisionYesNo = true;
            QuickHPStorage(starlight.CharacterCard, legacy.CharacterCard, harpy.CharacterCard, birds);

            UsePower(starlight);

            QuickHPCheck(0, 0, 0, 0);
            AssertNumberOfCardsInPlay(starlight, 3);
        }
        [Test]
        public void TestGenesisPowerPlayIsOptional()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            UsePower(starlight);
            AssertNumberOfCardsInPlay(starlight, 1);
        }
        [Test]
        public void TestGenesisPowerNoConstellationsInDeck()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            MoveCards(starlight, (Card c) => GameController.DoesCardContainKeyword(c, "constellation"), starlight.HeroTurnTaker.Hand);

            UsePower(starlight);
            AssertNumberOfCardsInPlay(starlight, 1);
        }
        [Test()]
        public void TestGenesisPowerIsActualPlay()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            PlayCard("HostageSituation");

            UsePower(starlight);
            AssertNumberOfCardsInPlay(starlight, 1);
        }
        [Test]
        public void TestGenesisIncap1Basic()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            SetupIncap(baron);

            Card takedown = PutInTrash("TakeDown");
            QuickHandStorage(legacy, harpy);

            UseIncapacitatedAbility(starlight, 0);
            QuickHandCheck(1, 0);
            AssertInHand(takedown);
        }
        [Test]
        public void TestGenesisIncap1OnlyOneCard()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            SetupIncap(baron);

            PutInTrash("TakeDown", "AppliedNumerology", "Fortitude", "HarpyHex");

            QuickHandStorage(legacy, harpy);
            DecisionSelectTurnTakers = new List<TurnTaker> { legacy.TurnTaker, harpy.TurnTaker };

            UseIncapacitatedAbility(starlight, 0);
            QuickHandCheck(1, 0);
            AssertNumberOfCardsInTrash(legacy, 1);

            UseIncapacitatedAbility(starlight, 0);
            QuickHandCheck(0, 1);
            AssertNumberOfCardsInTrash(harpy, 1);
        }
        [Test]
        public void TestGenesisIncap2()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            SetupIncap(baron);

            AssertIncapLetsHeroUsePower(starlight, 1, legacy);
        }
        [Test]
        public void TestGenesisIncap3PicksAnyDeck()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            SetupIncap(baron);

            AssertNextDecisionChoices(new List<Location> { legacy.TurnTaker.Deck, harpy.TurnTaker.Deck, baron.TurnTaker.Deck, FindEnvironment().TurnTaker.Deck }.Select((Location deck) => new LocationChoice(deck)));
        }
        [Test]
        public void TestGenesisIncap3Replace()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            SetupIncap(baron);

            Card takedown = PutOnDeck("TakeDown");
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Deck);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);

            UseIncapacitatedAbility(starlight, 2);

            AssertOnTopOfDeck(legacy, takedown);
        }
        [Test]
        public void TestGenesisIncap3Discard()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "TheHarpy", "Megalopolis" }, false, promoDict);

            StartGame();
            SetupIncap(baron);

            Card takedown = PutOnDeck("TakeDown");
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Trash);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);

            UseIncapacitatedAbility(starlight, 2);

            AssertNotOnTopOfDeck(legacy, takedown);
            AssertInTrash(takedown);
        }




        [Test()]
        [Order(0)]
        public void Area51StarlightLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight/Area51StarlightCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(starlight);
            Assert.IsInstanceOf(typeof(Area51StarlightCharacterCardController), starlight.CharacterCardController);

            foreach (var card in starlight.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(29, starlight.CharacterCard.HitPoints);
        }


        [Test]
        public void Area51StarlightInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight/Area51StarlightCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            DiscardAllCards(starlight);
            //load the hand with some stuff
            var c = PutInHand("EventHorizon");
            PutInHand("NightloreArmor");
            PutInHand("Redshift");
            PutInHand("StellarWind");
            var pillar = GetCard("PillarsOfCreation");

            DecisionSelectCards = new[] { c, pillar };

            GoToUsePowerPhase(starlight);

            QuickHandStorage(starlight);
            UsePower(starlight);
            AssertInPlayArea(starlight, pillar);
            AssertInTrash(c);
            QuickHandCheck(-1);
        }


        [Test]
        public void Area51StarlightInnatePower_SplitLocations()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight/Area51StarlightCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            MoveAllCardsFromHandToDeck(starlight);
            //load the hand with some stuff
            var c = PutInHand("EventHorizon");
            PutInHand("NightloreArmor");
            PutInHand("Redshift");
            PutInHand("StellarWind");
            var pillar1 = GetCard("PillarsOfCreation", 0);
            var pillar2 = PutInTrash("PillarsOfCreation", 1);

            GoToUsePowerPhase(starlight);

            DecisionSelectCards = new[] { c, pillar1 };
            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            AssertInPlayArea(starlight, pillar2);
            AssertInTrash(c);
            QuickHandCheck(-1);
        }

        [Test]
        public void Area51StarlightIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight/Area51StarlightCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(starlight);

            GoToUseIncapacitatedAbilityPhase(starlight);
            AssertIncapLetsHeroDrawCard(starlight, 0, haka, 1);
        }

        [Test]
        public void Area51StarlightIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight/Area51StarlightCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(starlight);

            GoToUseIncapacitatedAbilityPhase(starlight);
            AssertIncapLetsHeroUsePower(starlight, 1, haka);
        }

        [Test]
        public void Area51StarlightIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight/Area51StarlightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapacitated(starlight);

            var c1 = PlayCard("ImbuedFire");
            var c2 = PlayCard("FleshOfTheSunGod");
            var c3 = PlayCard("LivingForceField");

            DecisionSelectCards = new[] { c1, null, c3 };
            UseIncapacitatedAbility(starlight, 2);
            AssertInTrash(c1);
            AssertInPlayArea(ra, c2);
            AssertInTrash(c3);
        }
        
        [Test()]
        public void TestAsheronAsRepresentativeOfEarth()
        {
            SetupGameController(new string[] { "BaronBlade", "Legacy", "Haka", "Tachyon", "TheCelestialTribunal" });
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Cauldron.StarlightOfAsheronCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Starlight";

            Card representative = PlayCard("RepresentativeOfEarth");

            PrintJournal();
        }

    }
}

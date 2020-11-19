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
    public class StarlightVariantTests : BaseTest
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

        protected List<Card> EachStarlight { get { return new List<Card> { terra, asheron, cryos }; } }

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

            foreach(Card character in EachStarlight)
            {
                DecisionSelectCards = new List<Card> { character, mdp };
                foreach(Card otherchar in EachStarlight)
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

            for(int i = 0; i < 4; i++)
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
    }
}
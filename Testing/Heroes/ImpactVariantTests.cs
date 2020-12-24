using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Impact;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class ImpactVariantTests : BaseTest
    {
        #region ImpactHelperFunctions
        protected HeroTurnTakerController impact { get { return FindHero("Impact"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(impact.CharacterCard, 1);
            DealDamage(villain, impact, 2, DamageType.Melee);
        }
        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        private DamageType DTM = DamageType.Melee;

        #endregion
        [Test]
        public void TestRenegadeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(impact);
            Assert.IsInstanceOf(typeof(RenegadeImpactCharacterCardController), impact.CharacterCardController);

            Assert.AreEqual(27, impact.CharacterCard.HitPoints);
        }
        [Test]
        public void TestRenegadePowerNoDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card orb = PutInHand("GraviticOrb");
            Card hurl = PutInHand("HurledObstruction");

            DecisionSelectCard = hurl;
            UsePower(impact);
            AssertInHand(orb, hurl);
        }
        [Test]
        public void TestRenegadePowerDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card orb = PlayCard("GraviticOrb");
            Card pull = PlayCard("InescapablePull");

            Card hurl = PutInHand("HurledObstruction");
            Card orbit = PutInHand("DecayingOrbit");

            Card velocity = PutOnDeck("EscapeVelocity");
            
            DecisionSelectCards = new Card[] { orb, hurl, null };
            UsePower(impact);
            AssertInHand(orbit, velocity);
            AssertIsInPlay(hurl, pull);
            AssertInTrash(orb);
        }
        [Test]
        public void TestRenegadeIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);

            PlayCard("LivingForceField");

            QuickHPStorage(baron);
            DecisionSelectCard = haka.CharacterCard;
            UseIncapacitatedAbility(impact, 0);

            //not active yet
            DealDamage(haka, baron, 2, DTM);
            QuickHPCheck(-1);

            //GoToStartOfTurn(haka);
            GoToPlayCardPhase(haka);
            DealDamage(haka, baron, 2, DTM);
            QuickHPCheck(-2);

            GoToStartOfTurn(bunker);
            DealDamage(haka, baron, 2, DTM);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestRenegadeIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);

            AssertIncapLetsHeroUsePower(impact, 1, haka);
        }
        [Test]
        public void TestRenegadeIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);

            QuickHPStorage(haka, bunker);
            DecisionSelectTarget = bunker.CharacterCard;

            UseIncapacitatedAbility(impact, 2);
            QuickHPCheck(0, -2);
        }
        [Test]
        public void TestWastelandRoninLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(impact);
            Assert.IsInstanceOf(typeof(WastelandRoninImpactCharacterCardController), impact.CharacterCardController);

            Assert.AreEqual(26, impact.CharacterCard.HitPoints);
        }
        [Test]
        public void TestWastelandRoninPowerGainHP()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionSelectFunction = 0;

            SetHitPoints(impact, 20);
            QuickHPStorage(impact);
            UsePower(impact);
            QuickHPCheck(1);
        }
        [Test]
        public void TestWastelandRoninReplayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card pull = PlayCard("InescapablePull");

            DecisionSelectFunction = 1;

            QuickHandStorage(impact);
            UsePower(impact);
            AssertNumberOfStatusEffectsInPlay(1);
            DestroyCard(pull);
            AssertIsInPlay(pull);
            QuickHandCheck(1);

            //make sure it's only the next time
            AssertNumberOfStatusEffectsInPlay(0);
            DestroyCard(pull);
            AssertInTrash(pull);
        }
        [Test]
        public void TestWastelandRoninIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);

            AssertIncapLetsHeroUsePower(impact, 0, haka);
        }
        [Test]
        public void TestWastelandRoninIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            UseIncapacitatedAbility(impact, 1);

            GoToStartOfTurn(bunker);
            Card police = PlayCard("PoliceBackup");
            AssertIsInPlay(police);

            GoToNextTurn();
            Card traffic = PlayCard("TrafficPileup");
            AssertNotInPlay(traffic);

            GoToNextTurn();
            PlayCard(traffic);
            AssertIsInPlay(traffic);

        }
        [Test]
        public void TestWastelandRoninIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Haka", "Bunker", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);

            DecisionSelectCard = bunker.CharacterCard;

            UseIncapacitatedAbility(impact, 2);

            PlayCard("LivingForceField");
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DTM);
            QuickHPCheck(-2);
            DealDamage(bunker, baron, 2, DTM);
            QuickHPCheck(-1);
        }
    }
}
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheKnight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheKnightVariantTests : BaseTest
    {
        #region HelperFunctions
        protected string HeroNamespace = "Cauldron.TheKnight";

        protected HeroTurnTakerController knight { get { return FindHero("TheKnight"); } }

        protected Card youngKnight { get { return GetCard("TheYoungKnightCharacter"); } }
        protected Card oldKnight { get { return GetCard("TheOldKnightCharacter"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(knight, 1);
            DealDamage(villain, knight, 2, DamageType.Melee);
        }

        private readonly DamageType DTM = DamageType.Melee;

        private readonly string MessageTerminator = "There should have been no other messages.";
        #endregion


        [Test]
        public void TestBerserkerKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(BerserkerTheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(27, knight.CharacterCard.HitPoints);

            StartGame();
            Assert.AreEqual(knight.TurnTaker.InTheBox, youngKnight.Location);
            Assert.AreEqual(knight.TurnTaker.InTheBox, oldKnight.Location);
        }
        [Test]
        public void TestBerserkerKnightPowerSimple()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card mail = PlayCard("PlateMail");
            QuickHPStorage(baron, knight, wraith);
            UsePower(knight);
            QuickHPCheck(-6, 0, 0);
            AssertInTrash(mail);
        }
        [Test]
        public void TestBerserkerKnightPowerNoArmorToDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            AssertNextMessages("There are no equipment targets.", MessageTerminator);
            QuickHPStorage(baron, knight, wraith);
            UsePower(knight);
            QuickHPCheck(-0, 0, 0);
        }
        [Test]
        public void TestBerserkerKnightPowerDamageVariesWithArmorHP()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            PlayCard("PlateHelm");
            QuickHPStorage(baron, knight, wraith);
            UsePower(knight);
            QuickHPCheck(-4, 0, 0);

            Card helm = PlayCard("PlateHelm");
            SetHitPoints(helm, 1);
            UsePower(knight);
            QuickHPCheck(-2, 0, 0);
        }
        [Test]
        public void TestBerserkerKnightPowerCanBorrowAnimatedEquipment()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "RealmOfDiscord");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            PlayCard("ImbuedVitality");
            Card bolt = PlayCard("StunBolt");

            QuickHPStorage(baron, knight, wraith);
            UsePower(knight);
            QuickHPCheck(-7, 0, 0);
            AssertInTrash(bolt);
        }
        [Test]
        public void TestBerserkerKnightPowerWhenIndestructible()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "TimeCataclysm");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            PlayCard("FixedPoint");
            Card helm = PlayCard("PlateHelm");

            QuickHPStorage(baron, knight, wraith);
            UsePower(knight);
            QuickHPCheck(-4, 0, 0);
            AssertIsInPlay(helm);
        }
        [Test]
        public void TestBerserkerKnightPowerWhenDestroyReplaced()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "ChronoRanger", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card helm = PlayCard("PlateHelm");
            PlayCard("NoExecutions");

            QuickHPStorage(baron, knight, wraith);
            UsePower(knight);
            QuickHPCheck(-4, 0, 0);
            AssertNotInTrash(helm);
        }
        [Test]
        public void TestBerserkerKnightIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapLetsHeroDrawCard(knight, 0, ra, 1);
        }
        [Test]
        public void TestBerserkerKnightIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            Card traffic = PlayCard("TrafficPileup");

            UseIncapacitatedAbility(knight, 1);

            QuickHPStorage(ra, baron, wraith);
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(0, -2, 0);
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(-2, 0, 0);
            DealDamage(traffic, ra, 1, DTM);
            QuickHPCheck(-2, 0, 0);

            GoToStartOfTurn(knight);
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(-1, 0, 0);
        }
        [Test]
        public void TestBerserkerKnightIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerserkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);

            DecisionSelectTargets = new Card[] { wraith.CharacterCard, baron.CharacterCard, ra.CharacterCard, wraith.CharacterCard };
            PlayCard("StunBolt");
            UsePower("StunBolt");
            PlayCard("TheStaffOfRa");

            QuickHPStorage(baron, ra, wraith);

            for (int i = 0; i < 3; i++)
            {
                UseIncapacitatedAbility(knight, 2);
            }
            //Blade deals himself normal damage, Ra has +1 from Staff, Wraith has -1 from Stun Bolt
            QuickHPCheck(-1, -2, 0);
        }
        [Test]
        public void TestFairKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/FairTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(FairTheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(25, knight.CharacterCard.HitPoints);

            StartGame();
            Assert.AreEqual(knight.TurnTaker.InTheBox, youngKnight.Location);
            Assert.AreEqual(knight.TurnTaker.InTheBox, oldKnight.Location);
        }
        [Test]
        public void TestFairKnightPower()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/FairTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card helm = PlayCard("PlateHelm");
            Card mail = PlayCard("PlateMail");

            SetHitPoints(new Card[] { knight.CharacterCard, helm, mail, wraith.CharacterCard }, 2);
            QuickHPStorage(knight.CharacterCard, helm, mail, wraith.CharacterCard);

            AssertNoDecision();
            UsePower(knight);
            QuickHPCheck(2, 1, 2, 0);
        }
        [Test]
        public void TestPastKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(PastTheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(27, knight.CharacterCard.HitPoints);

            StartGame();
            Assert.AreEqual(knight.TurnTaker.InTheBox, youngKnight.Location);
            Assert.AreEqual(knight.TurnTaker.InTheBox, oldKnight.Location);
        }
        [Test]
        public void TestRoninKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Card knightInstructions = knight.TurnTaker.FindCard("TheKnightCharacter", realCardsOnly: false);
            Assert.IsNotNull(knightInstructions);
            Assert.IsFalse(knightInstructions.IsRealCard);
            Assert.IsInstanceOf(typeof(WastelandRoninTheKnightCharacterCardController), FindCardController(knightInstructions));

            StartGame();
            AssertIsInPlay(youngKnight);
            AssertIsInPlay(oldKnight);
            AssertNumberOfCardsInPlay(knight, 2);
            Assert.AreEqual(15, youngKnight.HitPoints);
            Assert.AreEqual(18, oldKnight.HitPoints);
        }
    }
}

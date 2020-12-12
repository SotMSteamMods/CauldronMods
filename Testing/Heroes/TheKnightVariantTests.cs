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
        public void TestFairKnightIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/FairTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            PutInHand("StunBolt");
            AssertIncapLetsHeroPlayCard(knight, 0, wraith, "StunBolt");
        }
        [Test]
        public void TestFairKnightIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/FairTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            AssertIncapLetsHeroDrawCard(knight, 1, ra, 1);
        }
        [Test]
        public void TestFairKnightIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/FairTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");

            QuickHPStorage(baron, ra, wraith);

            UseIncapacitatedAbility(knight, 2);
            
            //reduce damage from any environment to hero
            DealDamage(traffic, ra, 3, DTM);
            DealDamage(police, wraith, 4, DTM);
            //not from environment to villain
            DealDamage(traffic, baron, 1, DTM);
            QuickHPCheck(-1, -1, -2);

            //not from hero or villain to hero
            DealDamage(baron, ra, 1, DTM);
            DealDamage(ra, wraith, 1, DTM);

            QuickHPCheck(0, -1, -1);

            //only until next turn
            GoToStartOfTurn(knight);
            DealDamage(traffic, ra, 1, DTM);
            QuickHPCheck(0, -1, 0);
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
        public void TestPastKnightPowerSimple()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            Card helm = PutInHand("PlateHelm");
            Card mail = PutInHand("PlateMail");
            DecisionSelectCards = new Card[] { helm, ra.CharacterCard };
            QuickHandStorage(knight, ra);

            UsePower(knight);
            AssertInPlayArea(ra, helm);
            //replaces "knight" but not "you" - Knight plays card + draws card, Ra stands pat
            QuickHandCheck(0, 0);

            QuickHPStorage(knight.CharacterCard, ra.CharacterCard, helm);
            DecisionYesNo = true;
            //should be able to redirect damage from Ra
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(0, 0, -1);

            //should not be able to redirect damage from Knight
            DealDamage(baron, knight, 1, DTM);
            QuickHPCheck(-1, 0, 0);
        }
        [Test]
        public void TestPastKnightPowerDoesNotAllowUnplayableCards()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            Card helm1 = PlayCard("PlateHelm");
            PutOnDeck(knight, knight.HeroTurnTaker.Hand.Cards);
            Card helm2 = PutInHand("PlateHelm");

            AssertNoDecision();
            UsePower(knight);
            AssertInHand(helm2);
        }
        [Test]
        public void TestPastKnightPowerEndsWhenTargetLeavesPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            Card helm = PutInHand("PlateHelm");
            Card mail = PutInHand("PlateMail");
            DecisionSelectCards = new Card[] { helm, ra.CharacterCard };

            UsePower(knight);
            QuickHPStorage(knight.CharacterCard, ra.CharacterCard, helm);
            DecisionYesNo = true;
            //should be able to redirect damage from Ra
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(0, 0, -1);

            DestroyCard(helm);
            PlayCard(helm);
            QuickHPStorage(knight.CharacterCard, ra.CharacterCard, helm);

            //Ra should no longer have the Helm's protection
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(0, -1, 0);

            DealDamage(baron, knight, 1, DTM);
            QuickHPCheck(0, 0, -1);

        }
        [Test]
        public void TestPastKnightImbuedVitality()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "RealmOfDiscord");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card helm = PutInHand("PlateHelm");
            Card mail = PutInHand("PlateMail");
            Card sword = PutInHand("ShortSword");
            DecisionSelectCards = new Card[] { sword, ra.CharacterCard, knight.CharacterCard, baron.CharacterCard };
            PlayCard("ImbuedVitality");
            
            UsePower(knight);
            //short sword should be in play by Ra
            AssertInPlayArea(ra, sword);

            //sword's damage should be done by Ra, so we make the Knight unable to deal any
            PlayCard("ThroatJab");
            QuickHPStorage(baron);
            UsePower(sword);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestPastKnightIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapLetsHeroDrawCard(knight, 0, ra, 1);
        }
        [Test]
        public void TestPastKnightIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            Card topCard = GetTopCardOfDeck(baron);
            AssertNextMessages($"The Knight 1929 reveals {topCard.Title}", MessageTerminator);
            UseIncapacitatedAbility(knight, 1);
            AssertOnTopOfDeck(topCard);
            AssertNumberOfCardsAtLocation(baron.TurnTaker.Revealed, 0);
        }
        [Test]
        public void TestPastKnightIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            DecisionSelectTarget = baron.CharacterCard;
            UseIncapacitatedAbility(knight, 2);

            QuickHPStorage(baron, ra);
            //increase damage dealt by 2, but only once
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(0, -3);
            DealDamage(baron, ra, 1, DTM);
            QuickHPCheck(0, -1);

            //increase damage taken by 2, but only once
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-3, 0);
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-1, 0);
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
        [Test]
        public void TestRoninKnightAssignsArmor([Values("PlateHelm", "PlateMail")] string armorName)
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DecisionSelectCard = youngKnight;
            Card armor = PlayCard(armorName);
            AssertNextToCard(armor, youngKnight);

            QuickHPStorage(oldKnight, youngKnight, armor);
            DecisionYesNo = true;

            //armor should protect Young Knight and not Old Knight
            DealDamage(baron, oldKnight, 1, DTM);
            QuickHPCheck(-1, 0, 0);
            DealDamage(baron, youngKnight, 1, DTM);
            QuickHPCheck(0, 0, -1);

            DestroyCard(armor);
            DecisionSelectCard = oldKnight;
            PlayCard(armor);

            //now the other way around
            QuickHPStorage(oldKnight, youngKnight, armor);
            DealDamage(baron, oldKnight, 1, DTM);
            QuickHPCheck(0, 0, -1);
            DealDamage(baron, youngKnight, 1, DTM);
            QuickHPCheck(0, -1, 0);
        }
        [Test]
        public void TestRoninKnightAssignsWhetstone()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionSelectCard = youngKnight;
            Card stone = PlayCard("Whetstone");
            AssertNextToCard(stone, youngKnight);

            QuickHPStorage(baron);

            //should not be affected by Whetstone
            DealDamage(oldKnight, baron, 1, DTM);
            DealDamage(oldKnight, baron, 1, DamageType.Lightning);
            QuickHPCheck(-2);

            //should get +1 on the melee damage from Whetstone
            DealDamage(youngKnight, baron, 1, DTM);
            DealDamage(youngKnight, baron, 1, DamageType.Toxic);
            QuickHPCheck(-3);

            DecisionSelectCard = oldKnight;
            DestroyCard(stone);
            PlayCard(stone);

            //should now have Whetstone
            DealDamage(oldKnight, baron, 1, DTM);
            DealDamage(oldKnight, baron, 1, DamageType.Lightning);
            QuickHPCheck(-3);

            //should no longer have the +1 damage from Whetstone
            DealDamage(youngKnight, baron, 1, DTM);
            DealDamage(youngKnight, baron, 1, DamageType.Toxic);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestRoninKnightAssignsShortSword()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionSelectCard = youngKnight;
            Card sword = PlayCard("ShortSword");
            //this one does *not* go in the NextToLocation, as that breaks the power-use UI
            AssertAtLocation(sword, knight.TurnTaker.PlayArea);

            //should have the damage boost only on Young Knight
            QuickHPStorage(baron.CharacterCard, youngKnight, oldKnight);
            DealDamage(oldKnight, baron, 1, DTM);
            QuickHPCheck(-1, 0, 0);
            DealDamage(youngKnight, baron, 1, DTM);
            QuickHPCheck(-2, 0, 0);

            //power should NOT check which knight to use, but automatically pick YoungKnight
            AssertNextDecisionChoices(included: new Card[] { baron.CharacterCard });
            PlayCard("BacklashField");
            UsePower(sword);
            QuickHPCheck(-3, -3, 0);
        }
        [Test]
        public void TestRoninKnightAssignsStalwartShield()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionSelectCard = youngKnight;
            Card mail = PlayCard("PlateMail");

            DecisionSelectCard = oldKnight;
            Card helm = PlayCard("PlateHelm");
            Card shield = PlayCard("StalwartShield");

            QuickHPStorage(oldKnight, helm, youngKnight, mail);
            DecisionYesNo = false;

            //should protect Old Knight and his Plate Helm, should not protect Young Knight or her Plate Mail
            DealDamage(baron, oldKnight, 2, DTM);
            DealDamage(baron, helm, 2, DTM);
            DealDamage(baron, youngKnight, 2, DTM);
            DealDamage(baron, mail, 2, DTM);
            QuickHPCheck(-1, -1, -2, -2);
        }
    }
}

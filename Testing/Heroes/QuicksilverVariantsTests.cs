using Cauldron.Quicksilver;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class QuicksilverVariantsTests : CauldronBaseTest
    {

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        [Test()]
        public void TestUncannyQuicksilverLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(quicksilver);
            Assert.IsInstanceOf(typeof(UncannyQuicksilverCharacterCardController), quicksilver.CharacterCardController);

            Assert.AreEqual(26, quicksilver.CharacterCard.HitPoints);
        }

        [Test]
        public void TestUncannyQuicksilverPower()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            QuickHPStorage(apostate);
            UsePower(quicksilver);
            //{Quicksilver} deals 1 target 2 melee damage.
            QuickHPCheck(-2);
        }

        [Test]
        public void TestUncannyQuicksilverIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            DiscardAllCards(ra);

            //One player may draw until they have 4 cards in hand.
            QuickHandStorage(ra);
            UseIncapacitatedAbility(quicksilver, 0);
            QuickHandCheck(4);
        }


        [Test]
        public void TestUncannyQuicksilverIncap1_SoftLock()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "NightMist", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            SetupIncap(baron, ra.CharacterCard);
            PlayCard("MistForm");

            DrawCard(mist, 3);

            //One player may draw until they have 4 cards in hand.
            QuickHandStorage(mist);
            UseIncapacitatedAbility(quicksilver, 0);
            QuickHandCheck(0);
        }

        [Test]
        public void TestUncannyQuicksilverIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(apostate, quicksilver.CharacterCard);

            //One player may use a power now.
            AssertIncapLetsHeroUsePower(quicksilver, 1, ra);
        }

        [Test]
        public void TestUncannyQuicksilverIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            SetHitPoints(ra, 10);
            SetHitPoints(wraith, 10);
            Card knives = PutInHand("ThrowingKnives");
            Card comp = PutInHand("MicroTargetingComputer");

            Card staff = PutInHand("TheStaffOfRa");
            Card blaze = PutInHand("BlazingTornado");
            Card exc = PutInHand("Excavation");

            //One player may discard any number of cards to regain X HP, where X is the number of cards discarded.
            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, wraith.TurnTaker };
            DecisionSelectCards = new Card[] { staff, blaze, exc, null, knives, comp, null };

            QuickHPStorage(ra);
            UseIncapacitatedAbility(quicksilver, 2);
            QuickHPCheck(3);

            QuickHPStorage(wraith);
            UseIncapacitatedAbility(quicksilver, 2);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestRenegadeQuicksilverLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(quicksilver);
            Assert.IsInstanceOf(typeof(RenegadeQuicksilverCharacterCardController), quicksilver.CharacterCardController);

            Assert.AreEqual(27, quicksilver.CharacterCard.HitPoints);
        }

        [Test]
        public void TestRenegadeQuicksilverPower()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DiscardAllCards(quicksilver);

            PrintSpecialStringsForCard(quicksilver.CharacterCard);

            Card retort = PutInDeck("IronRetort");
            Card needle = PutInHand("ForestOfNeedles");

            DecisionSelectCard = retort;
            DecisionDiscardCard = needle;

            QuickHandStorage(quicksilver);
            UsePower(quicksilver);
            //Discard a card. Search your deck for Iron Retort and put it into your hand. Shuffle your deck.
            QuickHandCheckZero();
            AssertInHand(retort);
            AssertInTrash(needle);

            PrintSpecialStringsForCard(quicksilver.CharacterCard);

        }

        [Test]
        public void TestRenegadeQuicksilverIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);

            //One player may draw a card now.
            AssertIncapLetsHeroDrawCard(quicksilver, 0, ra, 1);
        }

        [Test]
        public void TestRenegadeQuicksilverIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(apostate, quicksilver.CharacterCard);
            Card staff = PutInHand("TheStaffOfRa");
            Card blaze = PutInHand("BlazingTornado");
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { staff, blaze };

            //The next time a hero is dealt damage, they may play a card.
            UseIncapacitatedAbility(quicksilver, 1);
            AssertInHand(new Card[] { staff, blaze });
            DealDamage(apostate, ra, 2, DamageType.Melee);
            PrintSeparator("Second instance of damage");
            DealDamage(apostate, ra, 2, DamageType.Melee);
            AssertIsInPlay(staff);
            AssertInHand(blaze);
        }

        [Test]
        public void TestRenegadeQuicksilverIncap2_NoDamageDealt()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(apostate, quicksilver.CharacterCard);
            Card staff = PutInHand("TheStaffOfRa");
            Card blaze = PutInHand("BlazingTornado");
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { staff, blaze };

            //The next time a hero is dealt damage, they may play a card.
            UseIncapacitatedAbility(quicksilver, 1);
            AssertInHand(new Card[] { staff, blaze });

            AddImmuneToNextDamageEffect(ra, false, true);
            DealDamage(apostate, ra, 2, DamageType.Melee);
            AssertInHand(new Card[] { staff, blaze });


        }

        [Test]
        public void TestRenegadeQuicksilverIncap2UsedTwice()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(apostate, quicksilver.CharacterCard);
            Card staff = PutInHand("TheStaffOfRa");
            Card blaze = PutInHand("BlazingTornado");
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { staff, blaze };

            //The next time a hero is dealt damage, they may play a card.
            UseIncapacitatedAbility(quicksilver, 1);
            UseIncapacitatedAbility(quicksilver, 1);
            AssertInHand(new Card[] { staff, blaze });
            DealDamage(apostate, ra, 2, DamageType.Melee);
            AssertIsInPlay(new Card[] { staff, blaze });
        }

        [Test]
        public void TestRenegadeQuicksilverIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/RenegadeQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(apostate, quicksilver.CharacterCard);

            //Increase all damage dealt by 1 until the start of your next turn.
            UseIncapacitatedAbility(quicksilver, 2);

            QuickHPStorage(ra);
            DealDamage(apostate, ra, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(apostate);
            DealDamage(wraith, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);

            GoToStartOfTurn(quicksilver);

            QuickHPStorage(ra);
            DealDamage(apostate, ra, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(apostate);
            DealDamage(wraith, apostate, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestHarbingerQuicksilverLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(quicksilver);
            Assert.IsInstanceOf(typeof(HarbingerQuicksilverCharacterCardController), quicksilver.CharacterCardController);

            Assert.AreEqual(26, quicksilver.CharacterCard.HitPoints);
        }
        [Test]
        public void TestHarbingerPowerAutoDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            AssertMaxNumberOfDecisions(0);
            Card spear = PutOnDeck("CoalescingSpear");
            Card storm = PutOnDeck("AlloyStorm");
            Card iron = PutOnDeck("IronRetort");

            UsePower(quicksilver);
            AssertInTrash(iron, storm);
            AssertOnTopOfDeck(spear);
        }
        [Test]
        public void TestHarbingerPowerReturn()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            Card spear = PutInTrash("CoalescingSpear");
            Card retort = PutInTrash("IronRetort");
            Card storm = PutOnDeck("AlloyStorm");
            DecisionSelectFunction = 1;
            DecisionSelectCard = spear;

            UsePower(quicksilver);
            AssertInHand(spear);
            AssertInTrash(retort);
            AssertOnTopOfDeck(storm);
        }
        [Test]
        public void TestHarbingerIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DealDamage(baron, quicksilver, 50, DamageType.Melee);

            PutInHand("TheStaffOfRa");
            AssertIncapLetsHeroPlayCard(quicksilver, 0, ra, "TheStaffOfRa");
        }
        [Test]
        public void TestHarbingerIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DealDamage(baron, quicksilver, 50, DamageType.Melee);

            Card traffic = PutIntoPlay("TrafficPileup");
            Card battalion = PutIntoPlay("BladeBattalion");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card redist = PutIntoPlay("ElementalRedistributor");

            SetHitPoints(battalion, 1);
            SetHitPoints(mdp, 3);
            SetHitPoints(traffic, 3);
            DecisionSelectCard = mdp;
            AssertNextDecisionChoices(new Card[] { mdp, traffic }, new Card[] { battalion, redist });

            UseIncapacitatedAbility(quicksilver, 1);
            AssertInTrash(mdp);
            AssertIsInPlay(traffic);
        }
        [Test]
        public void TestHarbingerIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DealDamage(baron, quicksilver, 50, DamageType.Melee);

            Card blast = PutInHand("FireBlast");
            Card summon = PutInHand("SummonStaff");

            DecisionSelectCard = blast;
            QuickHandStorage(ra, wraith);

            UseIncapacitatedAbility(quicksilver, 2);
            QuickHandCheck(0, 1);
            AssertInTrash(blast);
        }
        [Test]
        public void TestHarbingerIncap3NoDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/HarbingerQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DealDamage(baron, quicksilver, 50, DamageType.Melee);

            Card blast = PutInHand("FireBlast");
            Card summon = PutInHand("SummonStaff");

            DecisionSelectCards = new Card[] { null, blast };
            QuickHandStorage(ra, wraith);

            UseIncapacitatedAbility(quicksilver, 2);
            QuickHandCheck(0, 0);
            AssertInHand(blast);
        }
    }
}

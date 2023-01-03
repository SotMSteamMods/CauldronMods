using Cauldron.Quicksilver;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    class QuicksilverTests : CauldronBaseTest
    {

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        [Test()]
        public void TestQuicksilverLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(quicksilver);
            Assert.IsInstanceOf(typeof(QuicksilverCharacterCardController), quicksilver.CharacterCardController);

            Assert.AreEqual(25, quicksilver.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestQuicksilverDeckList()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");

            Card storm = GetCard("AlloyStorm");
            Assert.IsTrue(storm.DoKeywordsContain("one-shot"));
            Assert.IsTrue(storm.DoKeywordsContain("combo"));

            Card spear = GetCard("CoalescingSpear");
            Assert.IsTrue(storm.DoKeywordsContain("one-shot"));
            Assert.IsTrue(storm.DoKeywordsContain("combo"));

            Card chain = GetCard("ComboChain");
            Assert.IsTrue(chain.DoKeywordsContain("ongoing"));
            Assert.IsTrue(chain.DoKeywordsContain("limited"));

            Card forest = GetCard("ForestOfNeedles");
            Assert.IsTrue(forest.DoKeywordsContain("one-shot"));
            Assert.IsTrue(forest.DoKeywordsContain("finisher"));

            Card frenzy = GetCard("FrenziedMelee");
            Assert.IsTrue(frenzy.DoKeywordsContain("ongoing"));
            Assert.IsTrue(frenzy.DoKeywordsContain("limited"));

            Card breaker = GetCard("GuardBreaker");
            Assert.IsTrue(breaker.DoKeywordsContain("one-shot"));
            Assert.IsTrue(breaker.DoKeywordsContain("finisher"));

            Card retort = GetCard("IronRetort");
            Assert.IsTrue(retort.DoKeywordsContain("ongoing"));

            Card liquid = GetCard("LiquidMetal");
            Assert.IsTrue(liquid.DoKeywordsContain("one-shot"));

            Card armor = GetCard("MalleableArmor");
            Assert.IsTrue(armor.DoKeywordsContain("ongoing"));
            Assert.IsTrue(armor.DoKeywordsContain("limited"));

            Card strike = GetCard("MercuryStrike");
            Assert.IsTrue(strike.DoKeywordsContain("one-shot"));
            Assert.IsTrue(strike.DoKeywordsContain("combo"));

            Card shard = GetCard("MirrorShard");
            Assert.IsTrue(shard.DoKeywordsContain("ongoing"));
            Assert.IsTrue(shard.DoKeywordsContain("limited"));

            Card stress = GetCard("StressHardening");
            Assert.IsTrue(stress.DoKeywordsContain("ongoing"));
            Assert.IsTrue(stress.DoKeywordsContain("limited"));

            Card subject = GetCard("TestSubjectHalberd");
            Assert.IsTrue(subject.DoKeywordsContain("one-shot"));

            Card memories = GetCard("ViciousMemories");
            Assert.IsTrue(memories.DoKeywordsContain("ongoing"));
            Assert.IsTrue(memories.DoKeywordsContain("limited"));

            Card steel = GetCard("WhisperingSteel");
            Assert.IsTrue(steel.DoKeywordsContain("one-shot"));
            Assert.IsTrue(steel.DoKeywordsContain("combo"));
        }
        [Test]
        public void TestQuicksilverPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            QuickHandStorage(quicksilver);
            UsePower(quicksilver);
            //-1 for discard, +2 for draw
            QuickHandCheck(1);
            AssertNumberOfCardsInTrash(quicksilver, 1);

            DiscardAllCards(quicksilver);
            QuickHandStorage(quicksilver);
            UsePower(quicksilver);
            //with nothing to discard, shouldn't draw
            QuickHandCheck(0);
        }
        [Test]
        public void TestQuicksilverIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            PutInHand("TheStaffOfRa");

            //"One player may play a card now.",
            AssertIncapLetsHeroPlayCard(quicksilver, 0, ra, "TheStaffOfRa");
        }
        [Test]
        public void TestQuicksilverIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            Card living = PlayCard("LivingForceField");

            //"Destroy 1 ongoing card.",
            UseIncapacitatedAbility(quicksilver, 1);
            AssertInTrash(living);
        }
        [Test]
        public void TestQuicksilverIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(ra.CharacterCard, mdp, traffic);

            //"Until the start of your next turn increase melee damage dealt by hero targets by 1."
            UseIncapacitatedAbility(quicksilver, 2);
            DealDamage(ra, mdp, 1, DamageType.Melee);
            DealDamage(ra, traffic, 1, DamageType.Melee);
            DealDamage(mdp, ra, 1, DamageType.Melee);
            DealDamage(traffic, ra, 1, DamageType.Melee);
            QuickHPCheck(-2, -2, -2);

            DealDamage(ra, mdp, 1, DamageType.Fire);
            DealDamage(ra, traffic, 1, DamageType.Fire);
            QuickHPCheck(0, -1, -1);

            GoToStartOfTurn(quicksilver);
            DealDamage(ra, mdp, 1, DamageType.Melee);
            DealDamage(ra, traffic, 1, DamageType.Melee);
            QuickHPCheck(0, -1, -1);
        }

        [Test()]
        public void TestComboCombo()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");
            PlayCard(battalion);
            Card storm = GetCard("AlloyStorm", 1);
            DiscardAllCards(quicksilver);
            PutInHand(storm);

            DecisionSelectFunction = 1;
            DecisionSelectCard = storm;
            //{Quicksilver} may deal herself 2 melee damage and play a Combo.
            QuickHPStorage(battalion, mdp, ra.CharacterCard, quicksilver.CharacterCard);
            PlayCard("AlloyStorm", 0);
            //Playing Alloy Storm twice pickinmg continue combo twice deals all non-hero -1x2 and Quicksilver -2x2
            QuickHPCheck(-2, -2, 0, -4);
        }

        [Test()]
        public void TestComboCombo_noCombosInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card storm = GetCard("AlloyStorm", 1);
            DiscardAllCards(quicksilver);
            PutInHand(storm);

            DecisionSelectFunction = 1;
            DecisionSelectCard = storm;
            //{Quicksilver} may deal herself 2 melee damage and play a Combo.
            QuickHPStorage(baron, ra, quicksilver, wraith);
            PlayCard(storm);
            //Playing Alloy Storm twice pickinmg continue combo twice deals all non-hero -1x2 and Quicksilver -2x2
            QuickHPCheck(-1, 0, -2, 0);
        }

        [Test()]
        public void TestComboFinisher()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");
            PlayCard(battalion);
            Card forest = GetCard("ForestOfNeedles", 1);
            DiscardAllCards(quicksilver);
            PutInHand(forest);

            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { forest, battalion };
            //You may play a Finisher
            QuickHPStorage(battalion, mdp, ra.CharacterCard, quicksilver.CharacterCard);
            PlayCard("AlloyStorm", 0);
            //Playing Alloy Storm twice pickinmg continue finish deals all non-hero -1 and Blade Battalion -3
            QuickHPCheck(-4, -1, 0, 0);
        }

        [Test()]
        public void TestAlloyStormSkipCombo()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");
            PlayCard(battalion);

            DecisionDoNotSelectFunction = true;
            //{Quicksilver} deals each non-hero target 1 projectile damage.
            QuickHPStorage(battalion, mdp, ra.CharacterCard);
            PlayCard("AlloyStorm");
            QuickHPCheck(-1, -1, 0);
        }

        [Test()]
        public void TestCoalescingSpearSkipCombo()
        {
            SetupGameController("Spite", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DecisionDoNotSelectFunction = true;
            //{Quicksilver} deals 1 target 3 projectile damage.
            QuickHPStorage(spite);
            PlayCard("CoalescingSpear");
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestComboChainPreventDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            DiscardAllCards(quicksilver);
            DecisionSelectFunction = 1;

            //The first time each turn that {Quicksilver} would deal herself damage to play a Combo card, prevent that damage.
            QuickHPStorage(quicksilver);
            PlayCard("ComboChain");
            PlayCard("AlloyStorm");
            QuickHPCheck(0);
            PlayCard("AlloyStorm");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestForestOfNeedlesSkipCombo()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            DiscardAllCards(quicksilver);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectFunction = 1;
            DecisionSelectTarget = mdp;
            //{Quicksilver} may deal 6 melee damage to a target with more than 8HP, or 3 melee damage to a target with 8 or fewer HP.
            QuickHPStorage(mdp);
            PlayCard("ForestOfNeedles");
            QuickHPCheck(-6);
            //mdp HP is 4 after the first damage
            QuickHPStorage(mdp);
            PlayCard("ForestOfNeedles");
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestForestOfNeedlesDamageNotChangeAfterBeingDealt()
        {
            SetupGameController("KaargraWarfang", "Cauldron.Quicksilver", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            var heroFavor = FindTokenPool("CrowdsFavor", "HeroFavorPool");
            Card dymkharn = PlayCard("DymkharnTheFearless");
            
            QuickTokenPoolStorage(heroFavor);
            QuickHPStorage(dymkharn);
            DecisionSelectTarget = dymkharn;
            //10 max HP, so he should take 6 damage and trigger a favor point for the heroes
            //however he is left at 4 HP, so the 'damage readout' afterwards might fool it.
            PlayCard("ForestOfNeedles");
            QuickHPCheck(-6);
            QuickTokenPoolCheck(1);
        }
        [Test()]
        public void TestForestOfNeedlesCannotDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "TheWraith", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectCards = new List<Card> { quicksilver.CharacterCard, baron.CharacterCard };

            QuickHPStorage(baron, quicksilver);
            PlayCard("ThroatJab");
            PlayCard("ForestOfNeedles");
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestFrenziedMeleeYesRedirect()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            GoToPlayCardPhase(apostate);

            Card tony = PlayCard("TonyTaurus");
            PlayCard("FrenziedMelee");

            //Increase all damage dealt by 1.
            QuickHPStorage(apostate);
            DealDamage(ra, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(apostate);
            DealDamage(apostate, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(ra);
            DealDamage(ra, ra, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //The first time a hero target would be dealt damage by a non-hero target during the villain turn, you may redirect that damage to {Quicksilver}.
            DecisionYesNo = true;
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -3);

            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(apostate, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
            GoToPlayCardPhase(apostate);

            //The first time a hero target would be dealt damage by a non-hero target during the villain turn, you may redirect that damage to {Quicksilver}.
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -3);

            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(apostate, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestFrenziedMeleeNoRedirect()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            GoToPlayCardPhase(apostate);

            Card tony = PlayCard("TonyTaurus");
            PlayCard("FrenziedMelee");

            //Increase all damage dealt by 1.
            QuickHPStorage(apostate);
            DealDamage(ra, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(apostate);
            DealDamage(apostate, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(ra);
            DealDamage(ra, ra, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //The first time a hero target would be dealt damage by a non-hero target during the villain turn, you may redirect that damage to {Quicksilver}.
            DecisionYesNo = false;
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestFrenziedMeleeSkip()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            GoToPlayCardPhase(apostate);

            Card tony = PlayCard("TonyTaurus");
            PlayCard("FrenziedMelee");

            //The first time a hero target would be dealt damage by a non-hero target during the villain turn, you may redirect that damage to {Quicksilver}.
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }
        [Test()]
        public void TestFrenziedMeleeRedirectOnlyOnVillainTurn()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            GoToPlayCardPhase(apostate);

            Card tony = PlayCard("TonyTaurus");
            PlayCard("FrenziedMelee");

            GoToStartOfTurn(quicksilver);

            AssertNoDecision();
            //During a hero turn, the redirect shouldn't apply.
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);

            GoToStartOfTurn(FindEnvironment());
            //And during environment turn.
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestFrenziedMeleePower()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            Card melee = PlayCard("FrenziedMelee");

            //Destroy this card.
            AssertIsInPlay(melee);
            UsePower(melee);
            AssertInTrash(melee);
        }

        [Test()]
        public void TestGuardBreakerDestroy()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            Card imp = PlayCard("ImpPilferer");

            //make sure it's not destroying through damage
            AssertNotDamageSource(quicksilver.CharacterCard);
            //so a stray combo doesn't mess things up
            DiscardAllCards(quicksilver);

            //Destroy a target with 3 or fewer HP, or deal 1 target 3 irreducible melee damage.
            PlayCard("GuardBreaker");
            AssertInTrash(imp);
        }

        [Test()]
        public void TestGuardBreakerDealDamage()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            DecisionSelectFunction = 1;
            Card sword = GetCardInPlay("Condemnation");
            DecisionSelectTarget = sword;

            //Destroy a target with 3 or fewer HP, or deal 1 target 3 irreducible melee damage.
            QuickHPStorage(sword);
            PlayCard("GuardBreaker");
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIronRetort()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            DealDamage(apostate, quicksilver, 10, DamageType.Cold);

            //When this card enters play, draw a card and {Quicksilver} regains 2HP.
            QuickHandStorage(quicksilver);
            QuickHPStorage(quicksilver);
            Card retort = PlayCard("IronRetort");
            QuickHandCheck(1);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestIronRetortDoNotDestroy()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            DealDamage(apostate, quicksilver, 10, DamageType.Cold);
            Card melee = PutInHand("FrenziedMelee");
            Card retort = PlayCard("IronRetort");

            DecisionSelectCardToPlay = melee;
            DecisionYesNo = false;
            DealDamage(apostate, quicksilver, 2, DamageType.Cold);
            AssertIsInPlay(retort);
            AssertInHand(quicksilver, melee); //card wasn't played
        }

        [Test()]
        public void TestIronRetortDestroy()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            DealDamage(apostate, quicksilver, 10, DamageType.Cold);
            Card melee = PutInHand("FrenziedMelee");
            Card retort = PlayCard("IronRetort");

            //When {Quicksilver} is dealt damage, you may destroy this card. If you do, you may play a card.
            DecisionSelectCardToPlay = melee;
            DecisionYesNo = true;
            DealDamage(apostate, quicksilver, 2, DamageType.Cold);
            AssertInTrash(retort);
            AssertIsInPlay(melee);
        }

        [Test()]
        public void TestIronRetortDestroy_UhYeahImThatGuy()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "Guise", "RookCity");
            StartGame();

            DealDamage(apostate, quicksilver, 10, DamageType.Cold);
            Card totalBeefcake = PutInHand("TotalBeefcake");
            Card retort = PlayCard("IronRetort");
            Card uhYeah = PlayCard("UhYeahImThatGuy");

            //When {Quicksilver} is dealt damage, you may destroy this card. If you do, you may play a card.
            DecisionSelectCardToPlay = totalBeefcake;
            DecisionYesNo = true;
            DealDamage(apostate, guise, 2, DamageType.Cold);
            AssertInTrash(uhYeah);
            AssertIsInPlay(totalBeefcake);
        }

        [Test()]
        public void TestIronRetortDestroy_Timing()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            SetHitPoints(quicksilver.CharacterCard, 16);

            Card gauntletOfPerdition = PlayCard("GauntletOfPerdition");

            Card stressHardening = PutInHand("StressHardening");
            Card ironRetort = PlayCard("IronRetort");

            //When {Quicksilver} is dealt damage, you may destroy this card. If you do, you may play a card.
            DecisionSelectCardToPlay = stressHardening;
            DecisionYesNo = true;

            DealDamage(apostate, quicksilver, 2, DamageType.Melee);

            string gauntletMessage = "Gauntlet of Perdition dealt 1 Infernal damage to Quicksilver.";
            string ironRetortMessage = "Iron Retort was destroyed by Iron Retort and moved from Quicksilver's play area.";

            var journal = GameController.Game.Journal.Entries.Select(e => e.ToString()).ToList();
            Assert.Greater(journal.IndexOf(gauntletMessage), journal.IndexOf(ironRetortMessage), "Iron Retort triggered after Gauntlet of Perdition.");
        }

        [Test()]
        public void TestIronRetortDestroyIndestructible()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            DealDamage(apostate, quicksilver, 10, DamageType.Cold);
            Card melee = PutInHand("FrenziedMelee");
            Card retort = PlayCard("IronRetort");
            AssertInPlayArea(quicksilver, retort);

            var effect = new MakeIndestructibleStatusEffect();
            effect.CardSource = quicksilver.CharacterCard;
            effect.UntilEndOfNextTurn(quicksilver.TurnTaker);
            effect.CardsToMakeIndestructible.IsSpecificCard = retort;
            base.RunCoroutine(GameController.AddStatusEffect(effect, false, GetCardController(quicksilver.CharacterCard).GetCardSource()));

            //When {Quicksilver} is dealt damage, you may destroy this card. If you do, you may play a card.
            //we want to destroy, but it fails, so no card is played
            DecisionSelectCardToPlay = melee;
            DecisionYesNo = true;
            DealDamage(apostate, quicksilver, 2, DamageType.Cold);
            AssertInPlayArea(quicksilver, retort);
            AssertInHand(quicksilver, melee);
        }

        [Test()]
        public void TestLiquidMetalSkipCombo1Finisher()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            //Reveal cards from the top of your deck until you reveal a Combo and a Finisher and put them into your hand. Shuffle the other revealed cards back into your deck.

            Card spear = PutOnDeck("CoalescingSpear");
            Card retort = PutOnDeck("IronRetort");
            Card forest = PutOnDeck("ForestOfNeedles");
            Card breaker = PutOnDeck("GuardBreaker");
            //AlloyStorm
            DecisionDoNotSelectFunction = true;

            PlayCard("LiquidMetal");
            AssertInHand(breaker, spear);
            AssertInDeck(quicksilver, new Card[] { retort, forest });
        }

        [Test()]
        public void TestLiquidMetalAddCardsToHand()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            //Reveal cards from the top of your deck until you reveal a Combo and a Finisher and put them into your hand. Shuffle the other revealed cards back into your deck.

            Card forest = PutOnDeck("ForestOfNeedles");
            Card retort = PutOnDeck("IronRetort");
            Card storm = PutOnDeck("AlloyStorm");
            Card spear = PutOnDeck("CoalescingSpear");
            DecisionYesNo = false;

            PlayCard("LiquidMetal");
            AssertInHand(forest, spear);
            AssertInDeck(quicksilver, new Card[] { retort, storm });
        }

        [Test()]
        public void TestLiquidMetalPlayCard()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            //Reveal cards from the top of your deck until you reveal a Combo and a Finisher and put them into your hand. Shuffle the other revealed cards back into your deck.

            Card forest = PutOnDeck("ForestOfNeedles");
            Card retort = PutOnDeck("IronRetort");
            Card storm = PutOnDeck("AlloyStorm");
            Card spear = PutOnDeck("CoalescingSpear");
            DecisionYesNo = true;
            DecisionSelectCard = spear;
            DecisionDoNotSelectFunction = true;

            QuickHPStorage(apostate, quicksilver);
            PlayCard("LiquidMetal");
            QuickHPCheck(-3, -2);
        }
        [Test]
        public void TestLiquidMetalNoCombosInDeck()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            MoveAllCards(quicksilver, quicksilver.TurnTaker.Deck, quicksilver.TurnTaker.Trash);
            PutOnDeck("ForestOfNeedles");
            PutOnDeck("ViciousMemories");

            PlayCard("LiquidMetal");
        }
        [Test]
        public void TestLiquidMetalNoFinishersInDeck()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            MoveAllCards(quicksilver, quicksilver.TurnTaker.Deck, quicksilver.TurnTaker.Trash);
            PutOnDeck("CoalescingSpear");
            PutOnDeck("ViciousMemories");

            PlayCard("LiquidMetal");
        }
        [Test]
        public void TestLiquidMetalNoSearchablesInDeck()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            MoveAllCards(quicksilver, quicksilver.TurnTaker.Deck, quicksilver.TurnTaker.Trash);
            PutOnDeck("ViciousMemories");

            PlayCard("LiquidMetal");
        }
        [Test()]
        public void TestMalleableArmor()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            //If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP, restore her to 1HP.
            PlayCard("MalleableArmor");

            //Starting HP: 2 - Target HP: 0
            SetHitPoints(quicksilver.CharacterCard, 2);
            QuickHPStorage(quicksilver);
            DealDamage(apostate, quicksilver, 2, DamageType.Melee);
            AssertHitPoints(quicksilver, 1);

            //Starting HP: 2 - Target HP: -1
            SetHitPoints(quicksilver.CharacterCard, 2);
            QuickHPStorage(quicksilver);
            DealDamage(apostate, quicksilver, 3, DamageType.Melee);
            AssertHitPoints(quicksilver, 1);

            //Starting HP: 1 - Target HP: -2
            DealDamage(apostate, quicksilver, 3, DamageType.Melee);
            AssertIncapacitated(quicksilver);
        }

        [Test()]
        public void TestMalleableArmorFromMaxHP()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            //If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP, restore her to 1HP.
            PlayCard("MalleableArmor");

            DealDamage(apostate, quicksilver, 30, DamageType.Infernal);
            AssertHitPoints(quicksilver.CharacterCard, 1);
        }
        [Test]
        public void TestMalleableArmorWithRedirect()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "TheScholar", "RookCity");
            StartGame();

            //If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP, restore her to 1HP.
            PlayCard("MalleableArmor");

            PlayCard("AlchemicalRedirection");

            SetHitPoints(quicksilver, 5);

            DealDamage(apostate, quicksilver, 10, DamageType.Infernal);
            AssertHitPoints(quicksilver.CharacterCard, 5);
        }

        [Test]
        public void TestMalleableArmorDoesNotModifyDamageAmount()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            //If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP, restore her to 1HP.
            PlayCard("MalleableArmor");

            //assuming, for the moment, that this card works
            PlayCard("MirrorShard");

            DecisionYesNo = true;
            SetHitPoints(quicksilver, 2);
            QuickHPStorage(apostate);
            DealDamage(ra, apostate, 4, DamageType.Melee);
            //Quicksilver takes 4 and responds by punching for 5
            QuickHPCheck(-5);
        }
        [Test]
        public void TestMalleableArmorPower()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            PlayCard("MalleableArmor");

            SetHitPoints(quicksilver, 10);
            QuickHPStorage(quicksilver);

            //Power: "If {Quicksilver} has not dealt damage this turn, she regains 3HP."
            UsePower("MalleableArmor");
            QuickHPCheck(3);
        }
        [Test]
        public void TestMalleableArmorPowerHealOnlyIfNoDamage()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            PlayCard("MalleableArmor");

            SetHitPoints(quicksilver, 10);
            QuickHPStorage(quicksilver);

            DealDamage(quicksilver, apostate, 2, DamageType.Melee);
            //Power: "If {Quicksilver} has not dealt damage this turn, she regains 3HP."
            UsePower("MalleableArmor");
            QuickHPCheck(0);
        }
        [Test()]
        public void TestMercuryStrikeSkipCombo()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            Card sword = GetCardInPlay("Condemnation");
            DecisionSelectTargets = new Card[] { sword, apostate.CharacterCard };
            DecisionDoNotSelectFunction = true;

            //{Quicksilver} deals 1 target 2 melee damage, then 1 target 1 melee damage.
            QuickHPStorage(apostate.CharacterCard, sword);
            PlayCard("MercuryStrike");
            QuickHPCheck(-1, -1);

            //Same Target
            DecisionSelectTargets = new Card[] { apostate.CharacterCard, apostate.CharacterCard };
            QuickHPStorage(apostate.CharacterCard, sword);
            PlayCard("MercuryStrike");
            QuickHPCheck(-3, 0);
        }
        [Test]
        public void TestMirrorShard()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            Card shard = PutInHand("MirrorShard");

            //"When [Mirror Shard] enters play, draw a card."
            QuickHandStorage(quicksilver);
            PlayCard(shard);
            //net 0, play + draw
            QuickHandCheck(0);

            DecisionYesNo = true;
            DecisionSelectTarget = apostate.CharacterCard;

            QuickHPStorage(quicksilver, apostate);
            DealDamage(ra, apostate, 2, DamageType.Melee);

            //should be redirected to Quicksilver, who then punches Apostate for 3
            QuickHPCheck(-2, -3);
        }
        [Test]
        public void TestMirrorShardOptional()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            PlayCard("MirrorShard");

            DecisionYesNo = false;
            DecisionSelectTarget = apostate.CharacterCard;

            QuickHPStorage(quicksilver, apostate);
            DealDamage(ra, apostate, 2, DamageType.Melee);

            //should go straight to Apostate
            QuickHPCheck(0, -2);
        }
        [Test]
        public void TestMirrorShardKeepsDamageType()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "TheCourtOfBlood");
            StartGame();

            PlayCard("MirrorShard");

            DecisionYesNo = true;
            DecisionSelectTarget = apostate.CharacterCard;

            PlayCard("UnhallowedHalls");

            QuickHPStorage(quicksilver, apostate);
            DealDamage(ra, apostate, 2, DamageType.Infernal);

            //should be redirected to Quicksilver, given +1 for 3 damage
            //the punch to Apostate should be 4 infernal increased to 5
            QuickHPCheck(-3, -5);
        }
        [Test]
        public void TestMirrorShardOnlyRespondsIfFinalTarget()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "TheScholar", "TheCourtOfBlood");
            StartGame();

            PlayCard("MirrorShard");
            PlayCard("AlchemicalRedirection");

            DecisionYesNo = true;
            DecisionSelectTarget = apostate.CharacterCard;

            QuickHPStorage(apostate, scholar);
            DealDamage(legacy, scholar, 2, DamageType.Melee);
            QuickHPCheck(0, -2);
        }
        [Test()]
        public void TestMirrorShardNoSameTargetRedirect()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "TheScholar", "TheCourtOfBlood");
            StartGame();

            PlayCard("MirrorShard");

            DecisionYesNo = true;
            DecisionSelectTarget = apostate.CharacterCard;

            QuickHPStorage(apostate, quicksilver);
            DealDamage(legacy, quicksilver, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

            PlayCard("AlchemicalRedirection");
            DealDamage(legacy, quicksilver, 2, DamageType.Melee);
            QuickHPCheck(-3, -2);
        }

        [Test()]
        public void TestStressHardening()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            PlayCard("StressHardening");
            //No damage no increase
            QuickHPStorage(apostate, ra);
            DealDamage(quicksilver, apostate, 2, DamageType.Melee);
            DealDamage(quicksilver, ra, 2, DamageType.Melee);
            QuickHPCheck(-2, -2);

            //If {Quicksilver} currently has less than her max HP, increase damage she deals to non-hero targets by 1.
            DealDamage(apostate, quicksilver, 3, DamageType.Melee);
            QuickHPStorage(apostate, ra);
            DealDamage(quicksilver, apostate, 2, DamageType.Melee);
            DealDamage(quicksilver, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, -2);

            //If {Quicksilver} has 10 or fewer HP, increase damage she deals to non-hero targets by an additional 1.
            SetHitPoints(quicksilver, 10);
            QuickHPStorage(apostate, ra);
            DealDamage(quicksilver, apostate, 2, DamageType.Melee);
            DealDamage(quicksilver, ra, 2, DamageType.Melee);
            QuickHPCheck(-4, -2);
        }

        [Test()]
        public void TestTestSubjectHalberd()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            DealDamage(apostate, quicksilver, 15, DamageType.Melee);
            GoToPlayCardPhase(quicksilver);

            QuickHPStorage(quicksilver);
            PlayCard("TestSubjectHalberd");
            //{Quicksilver} regains 6HP.
            QuickHPCheck(6);
            //Immediately end your turn.
            AssertPhaseActionCount(-1);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(quicksilver, Phase.End);
        }

        [Test()]
        public void TestViciousMemories()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            GoToPlayCardPhase(quicksilver);
            PlayCard("ViciousMemories");
            GoToDrawCardPhase(quicksilver);
            //You may draw an extra card during your draw phase.
            AssertPhaseActionCount(2);

            //Don't play or power then get 3rd draw
            GoToDrawCardPhase(quicksilver);
            //You may draw an extra card during your draw phase.
            AssertPhaseActionCount(3);
        }

        [Test()]
        public void TestWhisperingSteelSkipCombo()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();

            Card sword = GetCardInPlay("Condemnation");
            DecisionSelectTarget = sword;
            DecisionDoNotSelectFunction = true;
            //{Quicksilver} deals 1 target 2 irreducible melee damage.
            QuickHPStorage(sword);
            PlayCard("WhisperingSteel");
            QuickHPCheck(-2);
        }
    }
}

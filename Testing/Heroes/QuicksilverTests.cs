using Cauldron.Quicksilver;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class QuicksilverTests : BaseTest
    {
        protected HeroTurnTakerController quicksilver { get { return FindHero("Quicksilver"); } }

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
        public void TestFrenziedMelee()
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
            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(tony, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -3);

            QuickHPStorage(ra.CharacterCard, quicksilver.CharacterCard);
            DealDamage(apostate, ra, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);

            GoToDrawCardPhase(ra);

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
    }
}

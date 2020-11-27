using Cauldron.SwarmEater;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class SwarmEaterTests : BaseTest
    {
        protected TurnTakerController swarm { get { return FindVillain("SwarmEater"); } }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        protected void AddCannotDealNextDamageTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.NumberOfUses = 1;
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        [Test()]
        public void TestSwarmEaterLoads()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(swarm);
            Assert.IsInstanceOf(typeof(SwarmEaterCharacterCardController), swarm.CharacterCardController);

            Assert.AreEqual(80, swarm.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestSwarmEaterDecklist()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");

            AssertHasKeyword("nanomutant", new string[] {
                "BladeAug",
                "FireAug",
                "JumperAug",
                "LasherAug",
                "SpeedAug",
                "StalkerAug",
                "SubterranAug",
                "VenomAug"
            });
            AssertHasKeyword("one-shot", new string[] {
                "FollowTheScreams",
                "InsatiableCharge",
                "NaniteCorruption",
                "UpdatedPriorities"
            });
            AssertHasKeyword("ongoing", new string[] { "BehindYou", "HuntingGrounds", "SingleMindedPursuit" });
            AssertHasKeyword("trait", new string[] { "AbsorbedNanites", "ConvertedBiomass" });

            AssertHitPoints(GetCard("BladeAug"), 4);
            AssertHitPoints(GetCard("FireAug"), 14);
            AssertHitPoints(GetCard("JumperAug"), 13);
            AssertHitPoints(GetCard("LasherAug"), 12);
            AssertHitPoints(GetCard("SpeedAug"), 7);
            AssertHitPoints(GetCard("StalkerAug"), 16);
            AssertHitPoints(GetCard("SubterranAug"), 6);
            AssertHitPoints(GetCard("VenomAug"), 15);
        }

        [Test()]
        public void TestSwarmEaterStartGame()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            Card blade = GetCard("BladeAug");
            Card fire = GetCard("FireAug");
            AddCannotDealNextDamageTrigger(swarm, swarm.CharacterCard);
            PutOnDeck(swarm, new Card[] { blade, fire });
            StartGame();

            //2 Traits
            AssertIsInPlay(new string[] { "AbsorbedNanites", "ConvertedBiomass" });
            //H-1 Targets
            AssertIsInPlay(new Card[] { blade, fire });
            //Single Minded Pursuit ends up next to the target with the lowest HP
            AssertNextToCard(GetCard("SingleMindedPursuit"), blade);
        }

        [Test()]
        public void TestSwarmEaterFrontFlip()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
            DestroyCard("SingleMindedPursuit");
            Assert.IsTrue(swarm.CharacterCard.IsFlipped);
        }

        [Test()]
        public void TestSwarmEaterFrontStartoFturn()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            QuickHPStorage(fire);
            StartGame();
            //At the start of the villain turn, {SwarmEater} deals the pursued target 3 psychic damage.
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestMoveSingleMindedPursuit()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            AddCannotDealNextDamageTrigger(swarm, swarm.CharacterCard);
            StartGame();

            //Whenever a pursued hero deals damage to a target other than {SwarmEater}, you may move Single-Minded Pursuit next to that target.
            DecisionYesNo = true;
            AssertNextToCard(pursuit, unity.CharacterCard);
            DealDamage(unity, stalker, 2, DamageType.Melee);
            AssertNextToCard(pursuit, stalker);
        }

        [Test()]
        public void TestVillainCantMoveSingleMindedPursuit()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            AddCannotDealNextDamageTrigger(swarm, swarm.CharacterCard);
            StartGame();

            //Whenever a pursued hero deals damage to a target other than {SwarmEater}, you may move Single-Minded Pursuit next to that target.
            //Only Hero Character Cards can move it
            DecisionYesNo = true;
            AssertNextToCard(pursuit, fire);
            DealDamage(fire, stalker, 2, DamageType.Melee);
            AssertNextToCard(pursuit, fire);
        }

        [Test()]
        public void TestNonCharacterCardHeroCantMoveSingleMindedPursuit()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            AddCannotDealNextDamageTrigger(swarm, swarm.CharacterCard);
            Card swift = PlayCard("SwiftBot");
            StartGame();

            //Whenever a pursued hero deals damage to a target other than {SwarmEater}, you may move Single-Minded Pursuit next to that target.
            //Only Hero Character Cards can move it
            DecisionYesNo = true;
            AssertNextToCard(pursuit, swift);
            DealDamage(swift, stalker, 2, DamageType.Melee);
            AssertNextToCard(pursuit, swift);
        }

        [Test()]
        public void TestEnvironmentCantMoveSingleMindedPursuit()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            AddCannotDealNextDamageTrigger(swarm, swarm.CharacterCard);
            Card mono = PlayCard("PlummetingMonorail");
            StartGame();

            //Whenever a pursued hero deals damage to a target other than {SwarmEater}, you may move Single-Minded Pursuit next to that target.
            //Only Hero Character Cards can move it
            DecisionYesNo = true;
            AssertNextToCard(pursuit, mono);
            DealDamage(mono, stalker, 2, DamageType.Melee);
            AssertNextToCard(pursuit, mono);
        }

        [Test()]
        public void TestCantMoveToSwarmEaterSingleMindedPursuit()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            AddCannotDealNextDamageTrigger(swarm, swarm.CharacterCard);
            StartGame();

            //Whenever a pursued hero deals damage to a target other than {SwarmEater}, you may move Single-Minded Pursuit next to that target.
            //Can't move next to Swarm Eater
            DecisionYesNo = true;
            AssertNextToCard(pursuit, unity.CharacterCard);
            DealDamage(unity, swarm, 2, DamageType.Melee);
            AssertNextToCard(pursuit, unity.CharacterCard);
        }

        [Test()]
        public void TestAdvancedSwarmEaterFront()
        {
            SetupGameController(new string[] { "Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis" }, true);
            Card fire = GetCard("FireAug");
            Card stalker = GetCard("StalkerAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();
            Card mono = PlayCard("PlummetingMonorail");

            QuickHPStorage(mono, legacy.CharacterCard, swarm.CharacterCard);
            DealDamage(swarm, mono, 2, DamageType.Melee);
            DealDamage(swarm, legacy, 2, DamageType.Melee);
            DealDamage(swarm, swarm, 2, DamageType.Melee);
            //Advanced: Increase damage dealt by {SwarmEater} to environment targets by 1.
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-5, -4, -4);
        }

        [Test()]
        public void TestSwarmEaterBackStartOfTurn()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
            DestroyCard("SingleMindedPursuit");

            //At the start of the villain turn, {SwarmEater} deals the target other than itself with the lowest HP 2 melee damage.
            QuickHPStorage(fire);
            GoToStartOfTurn(swarm);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSwarmEaterBackFlip()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
            DestroyCard("SingleMindedPursuit");
            Assert.IsTrue(swarm.CharacterCard.IsFlipped);

            //Whenever Single-Minded Pursuit enters play, flip {SwarmEater}'s villain character cards.
            PlayCard("SingleMindedPursuit");
            Assert.IsTrue(!swarm.CharacterCard.IsFlipped);
        }

        [Test()]
        public void TestSwarmEaterBackVillainCardResponse()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
            DestroyCard("SingleMindedPursuit");
            Assert.IsTrue(swarm.CharacterCard.IsFlipped);

            //Whenever a villain card is played {SwarmEater} deals the non-hero target other than itself with the lowest HP 3 melee damage.
            QuickHPStorage(fire);
            PlayCard("BehindYou");
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestSwarmEaterBackAdvancedResponse()
        {
            SetupGameController(new string[] { "Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis" }, true);
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
            DestroyCard("SingleMindedPursuit");
            Assert.IsTrue(swarm.CharacterCard.IsFlipped);

            Card speed = GetCard("SpeedAug");
            PutOnDeck(swarm, speed);
            //Advanced: Whenever {SwarmEater} destroys a villain target, play the top card of the villain deck.
            DealDamage(swarm, fire, 75, DamageType.Melee);
            AssertIsInPlay(speed);
        }

        [Test()]
        public void TestSwarmEaterBackAdvancedOnlyRespondOnVillainTarget()
        {
            SetupGameController(new string[] { "Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis" }, true);
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
            DestroyCard("SingleMindedPursuit");
            Assert.IsTrue(swarm.CharacterCard.IsFlipped);

            Card speed = GetCard("SpeedAug");
            PutOnDeck(swarm, speed);

            Card swift = PlayCard("SwiftBot");
            Card mono = PlayCard("PlummetingMonorail");
            //Advanced: Whenever {SwarmEater} destroys a villain target, play the top card of the villain deck.
            DealDamage(swarm, swift, 75, DamageType.Melee);
            DealDamage(swarm, mono, 75, DamageType.Melee);
            AssertInDeck(speed);
        }

        [Test()]
        public void TestBehindYou()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire });
            StartGame();

            Card behind = PlayCard("BehindYou");
            Card swift = GetCard("SwiftBot");
            //Whenever a target enters play, {SwarmEater} deals that target 1 melee damage.
            QuickHPStorage(swift);
            PlayCard(swift);
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-3);

            Card speed = GetCard("SpeedAug");
            PutOnDeck(swarm, speed);
            //When this card is destroyed, play the top card of the villain deck.
            DestroyCard(behind);
            AssertIsInPlay(speed);
        }

        [Test()]
        public void TestBladeAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();
            DestroyCards(new Card[] { stalker, fire });
            PlayCard("BladeAug");

            //At the end of the villain turn, this card deals the hero target with the highest HP 2 lightning damage.
            QuickHPStorage(haka);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestConvertedBiomass()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            Card bio = GetCard("ConvertedBiomass");
            SetHitPoints(swarm, 20);

            //At the end of the villain turn, {SwarmEater} regains X times 2 HP, where X is the number of cards beneath this one.
            QuickHPStorage(swarm);
            GoToEndOfTurn(swarm);
            QuickHPCheck(0);

            //This card and cards beneath it are indestructible.
            DestroyCard(bio);
            AssertIsInPlay(bio);

            //Whenever {SwarmEater} destroys an environment target, put it beneath this card. Cards beneath this one have no game text.
            Card mono = PlayCard("PlummetingMonorail");
            DealDamage(swarm, mono, 75, DamageType.Melee);
            AssertUnderCard(bio, mono);
            AssertNoGameText(mono);

            //This card and cards beneath it are indestructible.
            DestroyCard(mono);
            AssertUnderCard(bio, mono);

            //At the end of the villain turn, {SwarmEater} regains X times 2 HP, where X is the number of cards beneath this one.
            QuickHPStorage(swarm);
            GoToEndOfTurn(swarm);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestFireAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();
            DestroyCards(new Card[] { stalker });

            //At the end of the villain turn this card deals the hero target with the second highest HP {H - 1} fire damage and each player must discard a card.
            QuickHPStorage(legacy);
            QuickHandStorage(legacy, haka, unity);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-2);
            QuickHandCheck(-1, -1, -1);
        }

        [Test()]
        public void TestFollowTheScreams()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            Card adj = PlayCard("CelestialAdjudicator");
            Card turret = PlayCard("TurretBot");

            //{SwarmEater} deals the {H} non-character targets with the lowest HP 4 irreducible projectile damage each.
            //{SwarmEater} deals the hero target with the highest HP {H} melee damage.
            QuickHPStorage(stalker, fire, adj, turret, haka.CharacterCard);
            PlayCard("FollowTheScreams");
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(0, -6, -6, -6, -5);
        }

        [Test()]
        public void TestFollowTheScreamsFirstDamageNotCharacterCards()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            //{SwarmEater} deals the {H} non-character targets with the lowest HP 4 irreducible projectile damage each.
            //{SwarmEater} deals the hero target with the highest HP {H} melee damage.
            QuickHPStorage(stalker, fire, unity.CharacterCard, haka.CharacterCard);
            PlayCard("FollowTheScreams");
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-6, -6, 0, -5);
        }
    }
}

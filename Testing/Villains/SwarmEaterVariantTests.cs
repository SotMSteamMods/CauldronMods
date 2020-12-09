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
    public class SwarmEaterVariantTests : BaseTest
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
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Bunker", "Unity", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(swarm);
            Assert.IsInstanceOf(typeof(DistributedHivemindSwarmEaterCharacterCardController), swarm.CharacterCardController);

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
        public void TestBladeAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();
            DestroyCards(new Card[] { stalker, fire });
            Card blade = PlayCard("BladeAug");

            DealDamage(swarm, blade, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), blade);
            //Absorb: at the end of the villain turn, {SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            QuickHPStorage(haka);
            GoToEndOfTurn(swarm);
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-4);
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
        public void TestFireAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();
            DestroyCards(new Card[] { stalker });

            DealDamage(swarm, fire, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), fire);
            //Absorb: at the start of the villain turn, {H - 2} players must discard a card.
            QuickHandStorage(legacy);
            GoToStartOfTurn(swarm);
            QuickHandCheck(-1);
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

        [Test()]
        public void TestHuntingGrounds()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();
            Card ex0 = GetCard("CelestialExecutioner", 0);
            Card ex1 = GetCard("CelestialExecutioner", 1);
            PutOnDeck(env, new Card[] { ex0, ex1 });
            Card adj = PlayCard("CelestialAdjudicator");

            PlayCard("HuntingGrounds");
            //Reduce damage dealt to {SwarmEater} by environment targets by 1.
            QuickHPStorage(swarm);
            DealDamage(adj, swarm, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //Increase damage dealt by {SwarmEater} to environment targets by 1.
            QuickHPStorage(adj);
            DealDamage(swarm, adj, 2, DamageType.Melee);
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            //Celestial Adjudicator reduces damage dealt to environment targets by 1
            QuickHPCheck(-4);

            //Whenever {SwarmEater} destroys a target, play the top card of the environment deck.
            DealDamage(swarm, fire, 75, DamageType.Melee);
            AssertIsInPlay(ex1);
            DealDamage(swarm, ex1, 75, DamageType.Melee);
            AssertIsInPlay(ex0);
        }

        [Test()]
        public void TestInsatiableCharge0Destroy()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            Card staff = PlayCard("TheStaffOfRa");
            Card flesh = PlayCard("FleshOfTheSunGod");
            Card adj = PlayCard("CelestialAdjudicator");

            //{SwarmEater} deals each other target 2 melee damage. For each target destroyed by {SwarmEater} this way, destroy 1 hero ongoing or equipment card.
            QuickHPStorage(swarm.CharacterCard, legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, adj, stalker, fire);
            PlayCard("InsatiableCharge");
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            //Celestial Adjudicator reduces damage dealt to environment targets by 1
            QuickHPCheck(0, -4, -4, -4, -3, -4, -4);
            AssertIsInPlay(new Card[] { staff, flesh });
        }

        [Test()]
        public void TestInsatiableCharge1Destroy()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            Card staff = PlayCard("TheStaffOfRa");
            Card flesh = PlayCard("FleshOfTheSunGod");
            SetHitPoints(stalker, 1);

            //{SwarmEater} deals each other target 2 melee damage. For each target destroyed by {SwarmEater} this way, destroy 1 hero ongoing or equipment card.
            PlayCard("InsatiableCharge");
            AssertIsInPlay(new Card[] { staff });
            AssertInTrash(ra, flesh);
        }

        [Test()]
        public void TestInsatiableCharge2Destroy()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            Card staff = PlayCard("TheStaffOfRa");
            Card flesh = PlayCard("FleshOfTheSunGod");
            Card adj = PlayCard("CelestialAdjudicator");
            SetHitPoints(stalker, 1);
            SetHitPoints(adj, 1);

            //{SwarmEater} deals each other target 2 melee damage. For each target destroyed by {SwarmEater} this way, destroy 1 hero ongoing or equipment card.
            PlayCard("InsatiableCharge");
            AssertInTrash(ra, new Card[] { staff, flesh });
        }

        [Test()]
        public void TestJumperAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });

            Card jumper = PlayCard("JumperAug");
            Card adj = PlayCard("CelestialAdjudicator");
            //Reduce damage dealt to this card by non-villain cards by 1.
            QuickHPStorage(jumper);
            DealDamage(ra, jumper, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(jumper);
            DealDamage(adj, jumper, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //At the end of the villain turn this card deals the 2 hero targets with the lowest HP {H - 2} melee damage each.
            QuickHPStorage(legacy, haka, ra);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-1, 0, -1);
        }

        [Test()]
        public void TestJumperAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { fire, stalker });
            Card jumper = PlayCard("JumperAug");

            DealDamage(swarm, jumper, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), jumper);
            //Absorb: reduce damage dealt to {SwarmEater} by 1.
            QuickHPStorage(swarm);
            DealDamage(haka, swarm, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestLasherAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });
            Card staff = PlayCard("TheStaffOfRa");
            Card flesh = PlayCard("FleshOfTheSunGod");

            Card jumper = PlayCard("LasherAug");
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage and destroys {H - 2} hero ongoing and/or equipment cards.
            QuickHPStorage(haka);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-3);
            AssertInTrash(ra, flesh);
            AssertIsInPlay(staff);
        }

        [Test()]
        public void TestLasherAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { fire, stalker });
            Card lasher = PlayCard("LasherAug");
            Card moko = PlayCard("TaMoko");
            Card ring = PlayCard("TheLegacyRing");

            DealDamage(swarm, lasher, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), lasher);
            //Absorb: at the start of the villain turn, destroy 1 hero ongoing or equipment card.
            GoToStartOfTurn(swarm);
            AssertInTrash(ring);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestNaniteCorruption()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            Card lasher = GetCard("LasherAug");
            Card venom = GetCard("VenomAug");
            Card grounds = GetCard("HuntingGrounds");
            Card updated = GetCard("UpdatedPriorities");
            PutOnDeck(swarm, new Card[] { lasher, venom, grounds, updated });

            //Reveal cards from the top of the villain deck until {H - 1} targets are revealed. Put those cards into play and discard the rest.
            //{SwarmEater} deals the {H} targets other than itself with the lowest HP 2 projectile damage each.
            QuickHPStorage(ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, lasher, fire, stalker, venom);
            PlayCard("NaniteCorruption");
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(0, 0, 0, -4, -4, 0, -4);
            AssertInTrash(new Card[] { grounds, updated });
        }

        [Test()]
        public void TestSingleMindedPursuit()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            //Play this card next to the target with the lowest HP, other than {SwarmEater}. The target next to this card is Pursued.
            AssertNextToCard(pursuit, fire);

            //Increase damage dealt by {SwarmEater} by 2.
            QuickHPStorage(haka);
            DealDamage(swarm, haka, 2, DamageType.Melee);
            QuickHPCheck(-4);

            //If the Pursued target leaves play, destroy this card.
            DestroyCard(fire);
            AssertInTrash(pursuit);
        }

        [Test()]
        public void TestSpeedAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            Card speed = PlayCard("SpeedAug");
            //Increase damage dealt by villain targets by 1.
            QuickHPStorage(haka);
            DealDamage(swarm, haka, 2, DamageType.Melee);
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-5);

            QuickHPStorage(haka);
            DealDamage(fire, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(haka);
            DealDamage(stalker, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestSpeedAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { fire, stalker });
            Card speed = PlayCard("SpeedAug");

            DealDamage(swarm, speed, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), speed);
            //Absorb: increase damage dealt by {SwarmEater} by 1.
            QuickHPStorage(haka);
            DealDamage(swarm, haka, 2, DamageType.Melee);
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-5);

            QuickHPStorage(haka);
            DealDamage(legacy, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestStalkerAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            PlayCard("TaMoko");
            DestroyCard(fire);

            //At the end of the villain turn this card deals each hero target except the hero with the lowest HP {H - 2} irreducible energy damage.
            QuickHPStorage(haka, legacy, unity);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-1, -1, 0);
        }

        [Test()]
        public void TestStalkerAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { fire });
            Card terran = PlayCard("SubterranAug");
            Card mono = PlayCard("PlummetingMonorail");
            PlayCard("TaMoko");

            DealDamage(swarm, stalker, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), stalker);
            //Absorb: At the end of the villain turn, {SwarmEater} deals each other target 1 irreducible energy damage.
            QuickHPStorage(haka.CharacterCard, legacy.CharacterCard, unity.CharacterCard, terran, mono);
            GoToEndOfTurn(swarm);
            //Single-Minded Pursuit +2 damage dealt by Swarm Eater
            QuickHPCheck(-3, -3, -3, -3, -3);
        }

        [Test()]
        public void TestSubterranAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });
            Card sub = PlayCard("SubterranAug");
            GoToEndOfTurn(env);
            //To make sure Subterran isn't destroyed
            SetHitPoints(sub, 6);

            //At the start of the villain turn, put a random target from the villain trash into play.
            GoToStartOfTurn(swarm);
            //Swarm Eater, Converted Biomass, Absorbed Nanites, Subterran Aug, 1 random
            AssertNumberOfCardsAtLocation(swarm.TurnTaker.PlayArea, 5);
        }

        [Test()]
        public void TestSubterranAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });
            Card terran = PlayCard("SubterranAug");

            DealDamage(swarm, terran, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), terran);
            //Absorb: The first time {SwarmEater} would be dealt damage each turn, reduce that damage by 1
            QuickHPStorage(swarm);
            DealDamage(haka, swarm, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(swarm);
            DealDamage(haka, swarm, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestUpdatedPriorities()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            Card speed = PutOnDeck("SpeedAug");

            DestroyCard(pursuit);

            //Search the villain deck and trash for Single-Minded Pursuit and put it into play. If you searched the deck, shuffle it.
            //Play the top card of the villain deck.
            PlayCard("UpdatedPriorities");
            AssertNextToCard(pursuit, fire);
            AssertIsInPlay(speed);
        }

        [Test()]
        public void TestVenomAug()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });

            PlayCard("VenomAug");
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage. Any target dealt damage this way deals itself 1 toxic damage.
            QuickHPStorage(haka);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestVenomAugAbsorb()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "Megalopolis");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { stalker, fire, pursuit });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });
            Card venom = PlayCard("VenomAug");

            DealDamage(swarm, venom, 75, DamageType.Melee);
            AssertUnderCard(GetCard("AbsorbedNanites"), venom);

            //Absorb: Whenever {SwarmEater} deals damage to another target, that target deals itself 1 toxic damage.

            QuickHPStorage(haka);
            DealDamage(swarm, haka, 2, DamageType.Melee);

            // 2 damage from Swarm, +2 from Pursuit, +1 Haka damaging self from Venom Aug
            QuickHPCheck(-5);
        }


        [Test()]
        public void TestVenomAugSecondaryDamageSource()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Unity", "TheCelestialTribunal");
            Card pursuit = GetCard("SingleMindedPursuit");
            Card stalker = GetCard("StalkerAug");
            Card fire = GetCard("FireAug");
            PutOnDeck(swarm, new Card[] { pursuit, stalker, fire });
            StartGame();

            DestroyCards(new Card[] { stalker, fire });

            PlayCard("PunishTheWeak");

            PlayCard("VenomAug");
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage. Any target dealt damage this way deals itself 1 toxic damage.
            QuickHPStorage(haka);
            GoToEndOfTurn(swarm);
            //Punish The Weak will reduce the damage Haka deals to himself
            QuickHPCheck(-3);
        }
    }
}

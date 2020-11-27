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
    }
}

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
    public class SwarmEaterVariantTests : CauldronBaseTest
    {

        [Test()]
        public void TestHiveSwarmEaterLoads()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Bunker", "Unity", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(swarm);
            Assert.IsInstanceOf(typeof(DistributedHivemindSwarmEaterCharacterCardController), swarm.CharacterCardController);

            Assert.AreEqual(80, swarm.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestHiveSwarmEaterStartGame()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            //Search the villain deck for Converted Biomass and Absorbed Nanites and remove them from the game.
            AssertOutOfGame(GetCards(new string[] { "AbsorbedNanites", "ConvertedBiomass" }));
        }

        [Test()]
        public void TestHiveSwarmEaterAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis" }, true);
            StartGame();

            SetHitPoints(swarm, 17);
            Card pursuit = PlayCard("SingleMindedPursuit");
            //Single-Minded Pursuit is indestructible.
            DestroyCard(pursuit);
            AssertIsInPlay(pursuit);

            //When {SwarmEater} flips to this side he regains {H - 2} HP.
            QuickHPStorage(swarm);
            PlayCard("BladeAug");
            QuickHPCheck(1);
        }

        [Test()]
        public void TestHiveSwarmEaterCharacter()
        {
            SetupGameController(new string[] { "Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis" }, true);
            StartGame();

            //At then end of the villain turn, if there are no nanomutants in play, play the top card of the villain deck.
            Card jumper = PutOnDeck("JumperAug");
            Card fire = PutOnDeck("FireAug");
            GoToEndOfTurn(swarm);
            AssertIsInPlay(fire);
            AssertUnderCard(fire, jumper);

            //Whenever a villain target would deal damage to another villain target, redirect that damage to the hero target with the highest HP.
            QuickHPStorage(haka);
            DealDamage(fire, swarm, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(haka.CharacterCard, fire);
            DealDamage(fire, fire, 2, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestBladeAbsorbFire()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            //When a villain target enters play, flip {SwarmEater}'s villain character cards.
            //When {SwarmEater} flips to [Back] side, discard cards from the top of the villain deck until a target is discarded.
            //Put the discarded target beneath the villain target that just entered play. Then flip {SwarmEater}'s character cards.
            Card under = PutOnDeck("FireAug");
            Card played = PlayCard("BladeAug");

            AssertUnderCard(played, under);
            AssertNotFlipped(swarm);

            //check triggers: 
            //Blade deals highest hero 2 damage at EOT
            //Fire absorb causes H - 2 (in this case 1) discards at start of turn
            QuickHPStorage(legacy, haka, ra);
            QuickHandStorage(legacy, haka, ra);
            GoToEndOfTurn(swarm);
            QuickHPCheck(0, -2, 0);

            GoToStartOfTurn(swarm);
            QuickHandCheck(-1, 0, 0);
        }

        [Test()]
        public void TestFireAbsorbJumper()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("JumperAug");
            Card played = PlayCard("FireAug");

            AssertUnderCard(played, under);

            //check triggers:
            //Fire causes H-1 damage to second highest hero and makes everyone discard at EOT
            //Jumper absorb reduces damage dealt to absorbing target
            QuickHPStorage(legacy, haka, ra);
            QuickHandStorage(legacy, haka, ra);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-2, 0, 0);
            QuickHandCheck(-1, -1, -1);

            QuickHPStorage(played);
            DealDamage(legacy, played, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestJumperAbsorbLasher()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("LasherAug");
            Card played = PlayCard("JumperAug");

            AssertUnderCard(played, under);

            //check triggers:
            //Jumper reduces damage dealt to it by non-villain cards by 1
            //Lasher absorb blows up an equipment/ongoing at start of turn
            QuickHPStorage(played);
            DealDamage(legacy, played, 1, DamageType.Melee);

            Card tamoko = PlayCard("TaMoko");
            GoToStartOfTurn(swarm);
            AssertInTrash(tamoko);
        }

        [Test()]
        public void TestLasherAbsorbSpeed()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("SpeedAug");
            Card played = PlayCard("LasherAug");

            AssertUnderCard(played, under);

            //check triggers: 
            //Lasher does H damage to highest HP hero at EOT and blows up H-2 ongoing/equipment
            //Speed absorb increases absorber's damage by 1
            Card staff = PlayCard("TheStaffOfRa");
            QuickHPStorage(legacy, haka, ra);
            GoToStartOfTurn(legacy);
            AssertInTrash(staff);
            QuickHPCheck(0, -4, 0);
        }

        [Test()]
        public void TestSpeedAbsorbStalker()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("StalkerAug");
            Card played = PlayCard("SpeedAug");

            AssertUnderCard(played, under);

            //check triggers: 
            //Speed boosts damage from all villains by 1
            //Stalker absorb makes absorber deal 1 irreducible damage to all other targets at EOT
            QuickHPStorage(legacy, haka, ra);
            DealDamage(swarm, ra, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2);

            PlayCard("TaMoko");
            PlayCard("Fortitude");
            //because of Swarm Eater's redirect, it should be 2 to all heroes and an additional 2 to Haka
            GoToStartOfTurn(legacy);
            QuickHPCheck(-2, -4, -2);
        }

        [Test()]
        public void TestStalkerAbsorbSubterran()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("SubterranAug");
            Card played = PlayCard("StalkerAug");

            AssertUnderCard(played, under);

            PlayCard("TaMoko");
            PlayCard("Fortitude");
            //check triggers:
            //Stalker does H-2 irreducible to all heroes but the lowest
            //Subterran absorb reduces first damage dealt to absorber per turn by 1
            QuickHPStorage(legacy, haka, ra);
            GoToStartOfTurn(legacy);
            QuickHPCheck(-1, -1, 0);

            QuickHPStorage(played);
            DealDamage(legacy, played, 1, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(legacy, played, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSubterranAbsorbVenom()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("VenomAug");
            Card played = PlayCard("SubterranAug");

            AssertUnderCard(played, under);

            //check triggers:
            //Subterran returns a random villain target to play from trash at EOT
            //Venom absorb means any time absorber damages another target, the target also deals itself 1 damage

            //get some nice safe targets for the Subterran to reanimate - no damage triggers
            Card jumper = PutInTrash("JumperAug");
            Card speed = PutOnDeck("SpeedAug");

            GoToStartOfTurn(swarm);
            AssertIsInPlay(jumper);
            AssertUnderCard(jumper, speed);

            QuickHPStorage(legacy, haka, ra);
            PlayCard("TheStaffOfRa");
            PlayCard("TaMoko");
            DealDamage(played, haka, 2, DamageType.Melee);
            DealDamage(played, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -1, -4);
        }

        [Test()]
        public void TestVenomAbsorbBlade()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("BladeAug");
            Card played = PlayCard("VenomAug");

            AssertUnderCard(played, under);

            //check triggers:
            //Venom does H to highest HP hero at EOT, who then hits themself for 1
            //Blade absorb deals 2 to highest HP hero at EOT 

            QuickHPStorage(legacy, haka, ra);
            GoToEndOfTurn(swarm);
            QuickHPCheck(-2, -4, 0);
        }
        [Test()]
        public void TestAbsorbAbilitiesPersistThroughSaveLoad()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card speed = PutOnDeck("SpeedAug");
            Card jumper = PlayCard("JumperAug");
            AssertIsInPlay(jumper);
            AssertUnderCard(jumper, speed);

            QuickHPStorage(legacy);
            DealDamage(jumper, legacy, 1, DamageType.Melee);
            QuickHPCheck(-2);

            SaveAndLoad();
            
            jumper = GetCardInPlay("JumperAug");
            QuickHPStorage(legacy);
            DealDamage(jumper, legacy, 1, DamageType.Melee);
            QuickHPCheck(-2);
        }
        [Test()]
        public void TestChallengeMode()
        {
            SetupGameController(new string[] { "Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis" }, challenge: true);
            StartGame();
            DestroyNonCharacterVillainCards();
            //Challenge: Shared Code: Activated absorb texts also apply to Swarm Eater.

            Card subterran = PutOnDeck("SubterranAug");
            Card jumper = PlayCard("JumperAug");
            AssertUnderCard(jumper, subterran);
            //Check that Subterran Aug's first time each turn is once-per-target-per-turn
            QuickHPStorage(swarm.CharacterCard, jumper);
            DealDamage(legacy, swarm, 2, DamageType.Melee);
            QuickHPCheck(-1, 0);
            DealDamage(legacy, swarm, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            DealDamage(legacy, jumper, 2, DamageType.Melee);
            QuickHPCheck(0, 0);
            DealDamage(legacy, jumper, 2, DamageType.Melee);
            QuickHPCheck(0, -1);

            DestroyCard(jumper);

            Card speed = PutOnDeck("SpeedAug");
            Card lasher = PlayCard("LasherAug");

            Card fire = PutOnDeck("FireAug");
            Card blade = PlayCard("BladeAug");

            SetHitPoints(legacy, 30);
            SetHitPoints(haka, 10);
            SetHitPoints(ra, 10);

            //check that reloading doesn't break the challenge triggers
            SaveAndLoad();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHPStorage(legacy);
            QuickHandStorage(legacy);

            
            //Lasher should deal 3 + 1 damage at EOT, Blade should deal 2

            GoToEndOfTurn();
            QuickHPCheck(-6);

            //Also Swarm Eater's damage should be boosted by the Speed Aug absorb
            DealDamage(swarm, legacy, 1, DamageType.Melee);
            QuickHPCheck(-2);

            //Fire Aug's absorb should go off twice, causing Legacy to discard two cards
            GoToStartOfTurn(swarm);
            QuickHandCheck(-2);
        }
    }
}

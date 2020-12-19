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
            Card fire = PutOnDeck("FireAug");
            GoToEndOfTurn(swarm);
            AssertIsInPlay(fire);

            //Whenever a villain target would deal damage to another villain target, redirect that damage to the hero target with the highest HP.
            QuickHPStorage(haka);
            DealDamage(fire, swarm, 2, DamageType.Melee);
            QuickHPCheck(-2);
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
        }

        [Test()]
        public void TestFireAbsorbJumper()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("JumperAug");
            Card played = PlayCard("FireAug");

            AssertUnderCard(played, under);
        }

        [Test()]
        public void TestJumperAbsorbLasher()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("LasherAug");
            Card played = PlayCard("JumperAug");

            AssertUnderCard(played, under);
        }

        [Test()]
        public void TestLasherAbsorbSpeed()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("SpeedAug");
            Card played = PlayCard("LasherAug");

            AssertUnderCard(played, under);
        }

        [Test()]
        public void TestSpeedAbsorbStalker()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("StalkerAug");
            Card played = PlayCard("SpeedAug");

            AssertUnderCard(played, under);
        }

        [Test()]
        public void TestStalkerAbsorbSubterran()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("SubterranAug");
            Card played = PlayCard("StalkerAug");

            AssertUnderCard(played, under);
        }

        [Test()]
        public void TestSubterranAbsorbVenom()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("VenomAug");
            Card played = PlayCard("SubterranAug");

            AssertUnderCard(played, under);
        }

        [Test()]
        public void TestVenomAbsorbBlade()
        {
            SetupGameController("Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card under = PutOnDeck("BladeAug");
            Card played = PlayCard("VenomAug");

            AssertUnderCard(played, under);
        }
    }
}

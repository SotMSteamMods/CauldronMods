using NUnit.Framework;
using Cauldron.PhaseVillain;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class PhaseVillainTests : BaseTest
    {
        protected TurnTakerController phase { get { return FindVillain("PhaseVillain"); } }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        [Test()]
        public void TestPhaseLoad()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(phase);
            Assert.IsInstanceOf(typeof(PhaseVillainCharacterCardController), phase.CharacterCardController);

            foreach (Card card in phase.TurnTaker.GetAllCards())
            {
                CardController cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(30, phase.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestPhaseVillainDecklist()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");

            AssertHasKeyword("device", new string[] { "DensityRegulator", "DistortionGrenade", "DistortionNet" });

            AssertHasKeyword("obstacle", new string[] { "BlockedSightline", "PrecariousRubble", "ReinforcedWall", "VaultDoor" });

            AssertHasKeyword("one-shot", new string[] { "FrequencyShift", "NowhereToGoButDown" });

            AssertHasKeyword("ongoing", new string[] { "AlmostGotHer", "AroundTheCorner", "InsubstantialMatador", "ResidualDesynchronization", "UnimpededProgress" });

            AssertMaximumHitPoints(GetCard("BlockedSightline"), 5);
            AssertMaximumHitPoints(GetCard("DensityRegulator"), 10);
            AssertMaximumHitPoints(GetCard("DistortionGrenade"), 3);
            AssertMaximumHitPoints(GetCard("DistortionNet"), 4);
            AssertMaximumHitPoints(GetCard("PrecariousRubble"), 3);
            AssertMaximumHitPoints(GetCard("ReinforcedWall"), 10);
            AssertMaximumHitPoints(GetCard("VaultDoor"), 6);
        }

        [Test()]
        public void TestPhaseStartGame()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertIsInPlay("ReinforcedWall");
        }

        [Test()]
        public void TestPhaseFront()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Phase is immune to damage dealt by environment cards.
            Card popo = PlayCard("PoliceBackup");
            QuickHPStorage(phase);
            DealDamage(popo, phase, 2, DamageType.Melee);
            QuickHPCheck(0);

            //At the end of the villain turn, play the top card of the villain deck.
            Card vault = PutOnDeck("VaultDoor");
            GoToEndOfTurn(phase);
            AssertIsInPlay(vault);

            //At the start of the villain turn, if there are 3 or more obstacles in play, flip {PhaseVillain}'s villain character cards.
            PlayCard("BlockedSightline");
            GoToStartOfTurn(phase);
            AssertFlipped(phase);
        }

        public void TestPhaseFrontIncreaseDamage()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Increase damage dealt by {PhaseVillain} by 1 for each obstacle that has been removed from the game.
            QuickHPStorage(haka);
        }
    }
}

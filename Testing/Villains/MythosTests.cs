using NUnit.Framework;
using Cauldron.Mythos;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class MythosTests : BaseTest
    {
        protected TurnTakerController mythos { get { return FindVillain("Mythos"); } }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        protected const string MythosClueDeckIdentifier = "MythosClue";
        protected const string MythosDangerDeckIdentifier = "MythosDanger";
        protected const string MythosMadnessDeckIdentifier = "MythosMadness";

        private string GetIconIdentifier(Card c)
        {
            //Temporary method to get the icon of a card until Subdecks are implemented
            string[] eyeIdentifiers = { "DangerousInvestigation", "PallidAcademic", "Revelations", "RitualSite", "RustedArtifact", "TornPage" };
            string[] fearIdentifiers = { "AclastyphWhoPeers", "FaithfulProselyte", "OtherworldlyAlignment", "PreyUponTheMind" };
            string[] mindIdentifiers = { "ClockworkRevenant", "DoktorVonFaust", "HallucinatedHorror", "WhispersAndLies", "YourDarkestSecrets" };
            string identifier = null;
            if (eyeIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosClueDeckIdentifier;
            }
            if (fearIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosDangerDeckIdentifier;
            }
            if (mindIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosMadnessDeckIdentifier;
            }
            return identifier;
            /**Remove above when Subdecks are implemented**/
            return c.ParentDeck.Identifier;
        }

        [Test()]
        public void TestMythosLoad()
        {
            SetupGameController("Cauldron.Mythos", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mythos);
            Assert.IsInstanceOf(typeof(MythosCharacterCardController), mythos.CharacterCardController);

            foreach (Card card in mythos.TurnTaker.GetAllCards())
            {
                CardController cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(30, mythos.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestMythosDecklist()
        {
            SetupGameController("Cauldron.Mythos", "Haka", "Bunker", "TheScholar", "Megalopolis");

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
        public void Test()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "TheVisionary/DarkVisionaryCharacter", "Megalopolis");
            StartGame();

            Card peer = PlayCard("AclastyphWhoPeers");
            DecisionYesNo = true;

            GoToEndOfTurn(mythos);
            var a = GameController.Game.Journal;
            var b = 1;
        }

        [Test()]
        public void Test2()
        {
            SetupGameController("OblivAeon", "Legacy", "Bunker", "TheVisionary/DarkVisionaryCharacter", "Megalopolis", "Magmaria");
            StartGame();
            var a = oblivaeon;
        }
    }
}

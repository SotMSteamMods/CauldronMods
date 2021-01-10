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

        protected const string MythosClueDeckIdentifier = "MythosClue";
        protected const string MythosDangerDeckIdentifier = "MythosDanger";
        protected const string MythosMadnessDeckIdentifier = "MythosMadness";

        protected const string AclastyphWhoPeers = "AclastyphWhoPeers";
        protected const string ClockworkRevenant = "ClockworkRevenant";
        protected const string DangerousInvestigation = "DangerousInvestigation";
        protected const string DoktorVonFaust = "DoktorVonFaust";
        protected const string FaithfulProselyte = "FaithfulProselyte";
        protected const string HallucinatedHorror = "HallucinatedHorror";
        protected const string OtherworldlyAlignment = "OtherworldlyAlignment";
        protected const string PallidAcademic = "PallidAcademic";
        protected const string PreyUponTheMind = "PreyUponTheMind";
        protected const string Revelations = "Revelations";
        protected const string RitualSite = "RitualSite";
        protected const string RustedArtifact = "RustedArtifact";
        protected const string TornPage = "TornPage";
        protected const string WhispersAndLies = "WhispersAndLies";
        protected const string YourDarkestSecrets = "YourDarkestSecrets";

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        private string GetIconIdentifier(Card c)
        {
            //Temporary method to get the icon of a card until Subdecks are implemented
            string[] clueIdentifiers = { "DangerousInvestigation", "PallidAcademic", "Revelations", "RitualSite", "RustedArtifact", "TornPage" };
            string[] dangerIdentifiers = { "AclastyphWhoPeers", "FaithfulProselyte", "OtherworldlyAlignment", "PreyUponTheMind" };
            string[] madnessIdentifiers = { "ClockworkRevenant", "DoktorVonFaust", "HallucinatedHorror", "WhispersAndLies", "YourDarkestSecrets" };

            string identifier = null;
            if (clueIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosClueDeckIdentifier;
            }
            if (dangerIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosDangerDeckIdentifier;
            }
            if (madnessIdentifiers.Contains(c.Identifier))
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

            AssertMaximumHitPoints(mythos, 48);
        }

        [Test()]
        public void TestMythosDecklist()
        {
            SetupGameController("Cauldron.Mythos", "Haka", "Bunker", "TheScholar", "Megalopolis");

            //Assert Deck Back Icon
            //Blue Clue Icon
            foreach (string id in new string[] { DangerousInvestigation, PallidAcademic, Revelations, RitualSite, RustedArtifact, TornPage })
            {
                Assert.IsTrue(GetIconIdentifier(GetCard(id)) == MythosClueDeckIdentifier);
            }
            //Red Danger Icon
            foreach (string id in new string[] { AclastyphWhoPeers, FaithfulProselyte, OtherworldlyAlignment, PreyUponTheMind })
            {
                Assert.IsTrue(GetIconIdentifier(GetCard(id)) == MythosDangerDeckIdentifier);
            }
            //Green Madness Icon
            foreach (string id in new string[] { ClockworkRevenant, DoktorVonFaust, HallucinatedHorror, WhispersAndLies, YourDarkestSecrets })
            {
                Assert.IsTrue(GetIconIdentifier(GetCard(id)) == MythosMadnessDeckIdentifier);
            }

            //Assert Keywords
            AssertHasKeyword("ancient spawn", new string[] { AclastyphWhoPeers });

            AssertHasKeyword("distortion", new string[] { HallucinatedHorror });

            AssertHasKeyword("mad scientist", new string[] { DoktorVonFaust });

            AssertHasKeyword("minion", new string[] { ClockworkRevenant, FaithfulProselyte, PallidAcademic });

            AssertHasKeyword("one-shot", new string[] { PreyUponTheMind, Revelations, YourDarkestSecrets });

            AssertHasKeyword("ongoing", new string[] { DangerousInvestigation, OtherworldlyAlignment, WhispersAndLies });

            AssertHasKeyword("structure", new string[] { RitualSite });

            AssertHasKeyword("relic", new string[] { RustedArtifact, TornPage });

            //Assert hitpoints
            AssertMaximumHitPoints(GetCard(AclastyphWhoPeers), 6);
            AssertMaximumHitPoints(GetCard(ClockworkRevenant), 10);
            AssertMaximumHitPoints(GetCard(DoktorVonFaust), 12);
            AssertMaximumHitPoints(GetCard(FaithfulProselyte), 5);
            AssertMaximumHitPoints(GetCard(HallucinatedHorror), 15);
            AssertMaximumHitPoints(GetCard(PallidAcademic), 3);
            AssertMaximumHitPoints(GetCard(RitualSite), 10);
            AssertMaximumHitPoints(GetCard(RustedArtifact), 1);
            AssertMaximumHitPoints(GetCard(TornPage), 1);
        }

        [Test()]
        public void TestMythosFront_NotDanger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck(PallidAcademic);

            //{Mythos} and Dangerous Investigation are indestructible. 
            QuickHPStorage(mythos);
            DealDamage(haka, mythos, 100, 0);
            AssertNotGameOver();
            DestroyCard(DangerousInvestigation);
            AssertIsInPlay(DangerousInvestigation);

            //{MythosDanger} {Mythos} is immune to damage.
            QuickHPCheck(-100);
        }

        [Test()]
        public void TestMythosFront_Danger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck(AclastyphWhoPeers);

            //{MythosDanger} {Mythos} is immune to damage.
            QuickHPStorage(mythos);
            DealDamage(haka, mythos, 100, 0);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_0Play()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            QuickHPCheck(-3, -3, -3, -3, -3);
        }

        [Test()]
        public void TestAclastyphWhoPeers_Danger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //At the end of the villain turn:
            //{MythosDanger} This card deals the hero target with the highest HP 2 melee damage. Discard the top card of the villain deck.
            Card peer = PlayCard(AclastyphWhoPeers);
            Card pros = PutOnDeck(FaithfulProselyte);

            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            QuickHPCheck(-2, 0, 0);
            AssertInTrash(pros);
            AssertNumberOfCardsInTrash(mythos, 1);
        }
    }
}

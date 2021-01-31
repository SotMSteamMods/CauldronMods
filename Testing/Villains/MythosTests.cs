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

        private void AddTokensToDangerousInvestigationPool(int number)
        {
            AddTokensToPool(FindCardInPlay(DangerousInvestigation).FindTokenPool(DangerousInvestigation + "Pool"), number);
        }

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
            /**TODO: Remove above when Subdecks are implemented**/
            //return c.ParentDeck.Identifier;
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
        public void TestMythosFlip_3H()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck(AclastyphWhoPeers);

            //Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
            AddTokensToDangerousInvestigationPool(3);
            AssertNotFlipped(mythos);
            GoToEndOfTurn(mythos);
            AssertFlipped(mythos);
            //When {Mythos} flips to this side, remove Dangerous Investigation from the game.
            AssertOutOfGame(GetCard(DangerousInvestigation));
        }

        [Test()]
        public void TestMythosFlip_4H()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Unity", "Megalopolis");
            StartGame();

            PutOnDeck(AclastyphWhoPeers);

            //Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
            AddTokensToDangerousInvestigationPool(3);
            AssertNotFlipped(mythos);
            GoToEndOfTurn(mythos);
            AssertNotFlipped(mythos);

            AddTokensToDangerousInvestigationPool(1);
            GoToEndOfTurn(mythos);
            AssertFlipped(mythos);
        }

        [Test()]
        public void TestMythosFlip_5H()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            PutOnDeck(AclastyphWhoPeers);

            //Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
            AddTokensToDangerousInvestigationPool(3);
            AssertNotFlipped(mythos);
            GoToEndOfTurn(mythos);
            AssertNotFlipped(mythos);

            AddTokensToDangerousInvestigationPool(1);
            GoToEndOfTurn(mythos);
            AssertNotFlipped(mythos);

            AddTokensToDangerousInvestigationPool(1);
            GoToEndOfTurn(mythos);
            AssertFlipped(mythos);
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_0Play_TestDangerousInvestigation()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. 
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation: At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            QuickHPCheck(-3, -3, -3, -3, -3);
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_1Play_TestDangerousInvestigation()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            Card acad0 = PutOnDeck(mythos, GetCard(PallidAcademic, 0));
            DecisionsYesNo = new bool[] { true, false, false, false, false };

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. 
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation: At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            QuickHPCheck(-3, -3, -3, -3, 0);
            AssertNotInDeck(acad0);
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_2Play_TestDangerousInvestigation()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            IEnumerable<Card> academic = FindCardsWhere(c => c.Owner == mythos.TurnTaker && c.Identifier == PallidAcademic).Take(2);
            Card acad0 = PutOnDeck(mythos, academic.First());
            Card acad1 = PutOnDeck(mythos, academic.Last());
            DecisionsYesNo = new bool[] { true, true, false, false, false };

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. 
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation: At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            QuickHPCheck(-3, -3, -3, 0, 0);
            foreach (Card c in new Card[] { acad0, acad1 })
            {
                AssertNotInDeck(c);
            }
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_3Play_TestDangerousInvestigation()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            //Ensures later cards don't deal end of turn damage
            PutOnDeck(Revelations);
            IEnumerable<Card> academic = FindCardsWhere(c => c.Owner == mythos.TurnTaker && c.Identifier == PallidAcademic).Take(2);
            Card acad0 = PutOnDeck(mythos, academic.First());
            Card acad1 = PutOnDeck(mythos, academic.Last());
            Card site = PutOnDeck(RitualSite);
            DecisionsYesNo = new bool[] { true, true, true, false, false };

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. 
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation: At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            QuickHPCheck(-3, -3, 0, 0, 0);
            foreach (Card c in new Card[] { acad0, acad1, site })
            {
                AssertNotInDeck(c);
            }
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_4Play_TestDangerousInvestigation()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            //Ensures later cards don't deal end of turn damage
            PutOnDeck(Revelations);
            IEnumerable<Card> academic = FindCardsWhere(c => c.Owner == mythos.TurnTaker && c.Identifier == PallidAcademic).Take(2);
            Card acad0 = PutOnDeck(mythos, academic.First());
            Card acad1 = PutOnDeck(mythos, academic.Last());
            Card site = PutOnDeck(RitualSite);
            Card prey0 = PutOnDeck(mythos, GetCard(PreyUponTheMind, 0));
            DecisionsYesNo = new bool[] { true, true, true, true, false, false };

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. 
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation: At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            QuickHPCheck(-3, 0, 0, 0, 0);
            foreach (Card c in new Card[] { acad0, acad1, site, prey0 })
            {
                AssertNotInDeck(c);
            }
        }

        [Test()]
        public void TestMythosFront_EndOfTurn_5Play_TestDangerousInvestigation()
        {
            SetupGameController("Cauldron.Mythos", "AkashThriya", "Haka", "Legacy", "Bunker", "Unity", "Megalopolis");
            StartGame();

            //Ensures later cards don't deal end of turn damage
            PutOnDeck(Revelations);
            IEnumerable<Card> academic = FindCardsWhere(c => c.Owner == mythos.TurnTaker && c.Identifier == PallidAcademic).Take(2);
            IEnumerable<Card> prey = FindCardsWhere(c => c.Owner == mythos.TurnTaker && c.Identifier == PreyUponTheMind).Take(2);

            Card acad0 = PutOnDeck(mythos, academic.First());
            Card acad1 = PutOnDeck(mythos, academic.Last());
            Card site = PutOnDeck(RitualSite);
            Card prey0 = PutOnDeck(mythos, prey.First());
            Card prey1 = PutOnDeck(mythos, prey.Last());
            DecisionsYesNo = new bool[] { true, true, true, true, true, false, false };

            //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. 
            QuickHPStorage(thriya, haka, legacy, bunker, unity);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation: At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            QuickHPCheck(0, 0, 0, 0, 0);
            foreach (Card c in new Card[] { acad0, acad1, site, prey0, prey1 })
            {
                AssertNotInDeck(c);
            }
        }

        [Test()]
        public void TestMythosFront_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            PutOnDeck(Revelations);

            //Advanced: Reduce damage dealt to {Mythos} by 2.
            QuickHPStorage(mythos);
            DealDamage(haka, mythos, 3, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestMythosBack_Danger_SkipPlay()
        {
            SetupGameController(new string[] { "Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis" });
            StartGame();

            PutOnDeck(ClockworkRevenant);
            Card peer = PutOnDeck(AclastyphWhoPeers);
            AddTokensToDangerousInvestigationPool(3);
            GoToEndOfTurn(mythos);

            //{MythosDanger} Reduce damage dealt to villain targets by 1.
            Card aca = PlayCard(PallidAcademic);
            QuickHPStorage(mythos.CharacterCard, aca, peer);
            DealDamage(haka, mythos, 2, DamageType.Melee);
            DealDamage(haka, aca, 2, DamageType.Melee);
            PlayCard(peer);
            DealDamage(haka, peer, 2, DamageType.Melee);
            //At the end of the villain turn, the players may play the top card of the villain deck.
            //This is proved because the card under Aclastyph is a Danger card, and thus doesn't trigger the damage reduction
            QuickHPCheck(-1, -1, -2);
        }

        [Test()]
        public void TestMythosBack_DontSkip_Clue()
        {
            SetupGameController(new string[] { "Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis" });
            StartGame();

            PutOnDeck(RitualSite);
            Card aca = PutOnDeck(PallidAcademic);
            Card peer = PutOnDeck(AclastyphWhoPeers);
            DecisionsYesNo = new bool[] { false, true, false };
            AddTokensToDangerousInvestigationPool(3);

            //At the end of the villain turn, the players may play the top card of the villain deck. Then:
            GoToEndOfTurn(mythos);
            AssertNotInDeck(peer);
            //{MythosClue} Play the top card of the villain deck.
            AssertNotInDeck(aca);
        }

        [Test()]
        public void TestMythosBack_Madness_SkipPlay()
        {
            SetupGameController(new string[] { "Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis" });
            StartGame();

            PutOnDeck(ClockworkRevenant);
            AddTokensToDangerousInvestigationPool(3);
            QuickHPStorage(legacy, bunker, haka);
            GoToEndOfTurn(mythos);
            //{MythosMadness} {Mythos} deals each hero target {H} infernal damage.
            QuickHPCheck(-3, -3, -3);
        }

        [Test()]
        public void TestMythosBack_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            PutOnDeck(AclastyphWhoPeers);
            AddTokensToDangerousInvestigationPool(3);
            GoToEndOfTurn(mythos);

            //Advanced: Activate all {MythosDanger} effects.
            QuickHPStorage(mythos);
            DealDamage(haka, mythos, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestAclastyphWhoPeers_Danger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //At the end of the villain turn:
            //{MythosDanger} This card deals the hero target with the highest HP 2 melee damage. Discard the top card of the villain deck.
            Card peer = PlayCard(AclastyphWhoPeers);

            //Danger
            Card pros1 = PutOnDeck(mythos, GetCard(FaithfulProselyte, 1));
            Card pros = PutOnDeck(mythos, GetCard(FaithfulProselyte, 0));

            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-5, -3, -3);
            AssertInTrash(pros);
            AssertNumberOfCardsInTrash(mythos, 1);
        }

        [Test()]
        public void TestAclastyphWhoPeers_Clue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //At the end of the villain turn:
            //{MythosClue} This card regains 2HP. Discard the top card of the villain deck.
            Card peer = PlayCard(AclastyphWhoPeers);
            SetHitPoints(peer, 1);

            //Clue
            Card aca1 = PutOnDeck(mythos, GetCard(PallidAcademic, 1));
            Card aca = PutOnDeck(mythos, GetCard(PallidAcademic, 0));

            QuickHPStorage(peer);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(2);
            AssertInTrash(aca);
            AssertNumberOfCardsInTrash(mythos, 1);
        }

        [Test()]
        public void TestAclastyphWhoPeers_Madness()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Madness
            PutOnDeck(ClockworkRevenant);

            //At the end of the villain turn:
            //{MythosMadness} This card deals each other target 1 psychic damage.
            Card peer = PlayCard(AclastyphWhoPeers);

            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestClockworkRevenant()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Clue
            PutOnDeck(PallidAcademic);

            PlayCard(ClockworkRevenant);
            //At the end of the villain turn, this card deals the hero target with the highest HP 2 melee damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-5, -3, -3);
        }

        [Test()]
        public void TestClockworkRevenant_Danger_0Damage()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Danger
            PutOnDeck(AclastyphWhoPeers);

            PlayCard(ClockworkRevenant);
            //At the end of the villain turn, this card deals the hero target with the highest HP 2 melee damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-5, -3, -3);
        }

        [Test()]
        public void TestClockworkRevenant_Danger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Danger
            PutOnDeck(AclastyphWhoPeers);

            Card clock = PlayCard(ClockworkRevenant);
            //{MythosDanger} Increase damage dealt by this card by X, where X is 10 minus its current HP.

            //+1
            SetHitPoints(clock, 9);
            QuickHPStorage(haka);
            DealDamage(clock, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //+3
            SetHitPoints(clock, 7);
            QuickHPStorage(haka);
            DealDamage(clock, haka, 2, DamageType.Melee);
            QuickHPCheck(-5);

            //+7
            SetHitPoints(clock, 3);
            QuickHPStorage(haka);
            DealDamage(clock, haka, 2, DamageType.Melee);
            QuickHPCheck(-9);
        }

        [Test()]
        public void TestDangerousInvestigation_NotClue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Danger
            PutOnDeck(AclastyphWhoPeers);

            //{MythosClue} At the end of the villain turn, the players may play the top card of the villain deck to add a token to this card.
            GoToEndOfTurn(mythos);
            Assert.IsTrue(FindCardInPlay(DangerousInvestigation).FindTokenPool(DangerousInvestigation + "Pool").CurrentValue == 0);
        }

        [Test()]
        public void TestDangerousInvestigation_Clue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Clue
            Card aca = PutOnDeck(PallidAcademic);

            DecisionsYesNo = new bool[] { false, true };

            //{MythosClue} At the end of the villain turn, the players may play the top card of the villain deck to add a token to this card.
            GoToEndOfTurn(mythos);
            Assert.IsTrue(FindCardInPlay(DangerousInvestigation).FindTokenPool(DangerousInvestigation + "Pool").CurrentValue == 1);
            AssertIsInPlay(aca);
        }

        [Test()]
        public void TestDoktorVonFaust_Revenant_NotClue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Danger
            PutOnDeck(AclastyphWhoPeers);

            Card dok = PlayCard(DoktorVonFaust);

            //At the end of the villain turn, search the villain deck for a Clockwork Revenant and put it into play. Shuffle the villain deck. If no card was put into play this way, this card deals each non-villain target 3 lightning damage.
            FindCardInPlay(ClockworkRevenant);

            //{MythosClue} Reduce damage dealt to this card by 2.
            QuickHPStorage(dok);
            DealDamage(haka, dok, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoktorVonFaust_NoRevenant_Clue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Clue
            PutOnDeck(PallidAcademic);
            StackDeckAfterShuffle(mythos, new string[] { PallidAcademic });

            Card dok = PlayCard(DoktorVonFaust);
            PutInTrash(mythos, (Card c) => c.Identifier == ClockworkRevenant);

            //At the end of the villain turn, search the villain deck for a Clockwork Revenant and put it into play. Shuffle the villain deck. If no card was put into play this way, this card deals each non-villain target 3 lightning damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-6, -6, -6);

            //{MythosClue} Reduce damage dealt to this card by 2.
            QuickHPStorage(dok);
            DealDamage(haka, dok, 3, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestFaithfulProselyte_NotMadness()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card mere = PlayCard("Mere");
            Card flak = PlayCard("FlakCannon");

            //Clue
            PutOnDeck(PallidAcademic);

            PlayCard(FaithfulProselyte);
            //{MythosMadness} When this card enters play, destroy {H - 2} equipment cards.
            AssertIsInPlay(new Card[] { mere, flak });

            //At the end of the villain turn, this card deals the hero target with the second highest HP {H - 1} lightning damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-3, -3, -5);
        }

        [Test()]
        public void TestFaithfulProselyte_Madness()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card mere = PlayCard("Mere");
            Card flak = PlayCard("FlakCannon");

            //Madness
            PutOnDeck(ClockworkRevenant);

            PlayCard(FaithfulProselyte);
            //{MythosMadness} When this card enters play, destroy {H - 2} equipment cards.
            AssertIsInPlay(mere);
            AssertInTrash(flak);
        }

        [Test()]
        public void TestFaithfulProselyte_Madness_OnlyOngoing()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card moko = PlayCard("TaMoko");

            //Madness
            PutOnDeck(ClockworkRevenant);

            PlayCard(FaithfulProselyte);
            //{MythosMadness} When this card enters play, destroy {H - 2} equipment cards.
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestHallucinatedHorror_Clue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Clue
            Card topCard = PutOnDeck(PallidAcademic);


            //{MythosMadness}{MythosDanger} When this card enters play, play the top card of the villain deck.
            Card horror = PlayCard(HallucinatedHorror);
            AssertOnTopOfDeck(topCard);

            //At the end of the villain turn, this card deals each hero target 2 sonic damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-5, -5, -5);

            //Destroy this card when a hero is dealt damage by another hero target.
            AssertIsInPlay(horror);
            DealDamage(haka, legacy, 2, DamageType.Melee);
            AssertInTrash(horror);
        }

        [Test()]
        public void TestHallucinatedHorror_Danger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Danger
            Card topCard = PutOnDeck(AclastyphWhoPeers);

            //{MythosMadness}{MythosDanger} When this card enters play, play the top card of the villain deck.
            Card horror = PlayCard(HallucinatedHorror);
            AssertIsInPlay(topCard);
        }

        [Test()]
        public void TestHallucinatedHorror_Madness()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Madness
            Card topCard = PutOnDeck(ClockworkRevenant);

            //{MythosMadness}{MythosDanger} When this card enters play, play the top card of the villain deck.
            Card horror = PlayCard(HallucinatedHorror);
            AssertIsInPlay(topCard);
        }

        [Test()]
        public void TestOtherworldlyAlignment_Danger()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Danger
            PutOnDeck(AclastyphWhoPeers);

            PlayCard(OtherworldlyAlignment);

            //At the end of the villain turn:
            //{MythosDanger} {Mythos} deals the hero target with the highest HP {H} infernal damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-6, -3, -3);
        }

        [Test()]
        public void TestOtherworldlyAlignment_Madness()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Madness
            PutOnDeck(ClockworkRevenant);

            PlayCard(OtherworldlyAlignment);


            //At the end of the villain turn:
            //{MythosMadness} {Mythos} deals each non-villain target 1 infernal damage.
            Card traffic = PlayCard("TrafficPileup");
            QuickHPStorage(haka.CharacterCard, bunker.CharacterCard, legacy.CharacterCard, traffic);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-4, -4, -4, -1);
        }

        [Test()]
        public void TestOtherworldlyAlignment_Clue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Clue
            PutOnDeck(PallidAcademic);

            PlayCard(OtherworldlyAlignment);

            //At the end of the villain turn:
            //{MythosClue} {Mythos} deals the hero target with the lowest HP {H} psychic damage.
            QuickHPStorage(haka, bunker, legacy);
            GoToEndOfTurn(mythos);
            //Dangerous Investigation hits all for 3
            QuickHPCheck(-3, -6, -3);
        }

        [Test()]
        public void TestPallidAcademic_Clue()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Clue
            Card topCard = PutOnDeck(TornPage);
            Card bottomCard = mythos.TurnTaker.Deck.BottomCard;

            IEnumerable<Card> trashOngoings = PutInTrash(new string[] { OtherworldlyAlignment, WhispersAndLies });
            IEnumerable<Card> trashOther = PutInTrash(new string[] { AclastyphWhoPeers, ClockworkRevenant });

            //When this card enters play, put all ongoing cards in the villain trash into play.
            PlayCard(PallidAcademic);
            AssertIsInPlay(trashOngoings);
            AssertInTrash(trashOther);

            //When a hero target is dealt damage by another hero target:
            DealDamage(haka, legacy, 2, DamageType.Melee);
            //{MythosDanger} Play the top card of the villain deck.
            AssertOnTopOfDeck(topCard);
            //{MythosMadness} Move the bottom card of a deck to the top of that deck.
            AssertOnBottomOfDeck(bottomCard);
        }

        [Test()]
        public void TestPallidAcademic_DangerMadness()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //Madness
            PutOnDeck(ClockworkRevenant);
            //Danger
            Card topCard = PutOnDeck(AclastyphWhoPeers);
            Card bottomCard = mythos.TurnTaker.Deck.BottomCard;

            PlayCard(PallidAcademic);

            //When a hero target is dealt damage by another hero target:
            DealDamage(haka, legacy, 2, DamageType.Melee);
            //{MythosDanger} Play the top card of the villain deck.
            AssertIsInPlay(topCard);

            //Playing the top card reveals a Madness card
            //{MythosMadness} Move the bottom card of a deck to the top of that deck.
            AssertOnTopOfDeck(bottomCard);
        }

        [Test()]
        public void TestYourDarkestSecrets()
        {
            SetupGameController("Cauldron.Mythos", "Legacy", "Bunker", "Haka", "Unity", "Ra", "Megalopolis");
            StartGame();

            DiscardAllCards(legacy, bunker, haka, unity, ra);

            PutInHand(legacy, new string[] { "SurgeOfStrength", "NextEvolution" });
            Card legacyTop = PutOnDeck("Fortitude");

            PutInHand(bunker, new string[] { "FlakCannon", "GatlingGun", "HeavyPlating" });
            Card bunkerTop = PutOnDeck("FlakCannon");

            PlayCard("TaMoko");
            PutInHand(haka, new string[] { "HakaOfBattle", "HakaOfShielding", "VitalitySurge" });
            Card hakaTop = PutOnDeck("ElbowSmash");

            PutInHand(unity, new string[] { "ChampionBot", "BeeBot", "TurretBot", "StealthBot" });
            Card unityTop = PutOnDeck("SwiftBot");

            PutInHand(ra, new string[] { "TheStaffOfRa", "FlameBarrier" });
            Card raTop = PutOnDeck("FireBlast");

            QuickHPStorage(legacy, bunker, haka, unity, ra);
            PlayCard(YourDarkestSecrets);
            //Discard the top card of each hero deck.
            AssertInTrash(raTop, unityTop, hakaTop, bunkerTop, legacyTop);

            //{Mythos} deals each hero 1 infernal damage for each card in their hand that shares a keyword with the card discarded from their deck.
            //Since it is 3 instances of 1 for Haka and he has DR 1, he takes 0
            QuickHPCheck(-2, -3, 0, -4, 0);
        }
    }
}

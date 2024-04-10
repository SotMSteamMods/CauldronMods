using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class NightloreCitadelTests : CauldronBaseTest
    {

        #region NightloreCitadelHelperFunctions

        protected TurnTakerController nightlore { get { return FindEnvironment(); } }

        protected bool IsConstellation(Card card)
        {
            return card != null && card.DoKeywordsContain("constellation");
        }

        #endregion

        [Test()]
        public void TestNightloreCitadelWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_NightloreTrainer_IsNightloreTrainer([Values("TalinBrosk")] string nightloreTrainer)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreTrainer);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore trainer", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Superweapon_IsSuperweapon([Values("AethiumCannon")] string superweapon)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(superweapon);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "superweapon", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_NightloreCouncilor_IsNighloreCouncilor([Values("ArtemisVector")] string nightloreCouncilor)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreCouncilor);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore councilor", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_NightloreAgent_IsNighloreAgent([Values("StarlightOfZzeck", "StarlightOfNoome")] string nightloreAgent)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreAgent);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore agent", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_AncientAutomaton_IsAncientAutomaton([Values("CitadelGarrison")] string ancientAutomation)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(ancientAutomation);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "ancient automaton", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_NightloreTraitor_IsNighloreTraitor([Values("StarlightOfOros")] string nightloreTraitor)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreTraitor);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore traitor", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_Constellation_IsConstellation([Values("RogueConstellation")] string constellation)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(constellation);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "constellation", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("WarpBridge", "GravityFluctuation", "LonelyCalling", "AssembleTheCouncil", "WatchedByTheStars", "UrgentMission", "AethiumRage")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(keywordLess);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }

        [Test()]
        public void TestLonelyCalling_DestroyAtStart_CriteriaMet()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            Card calling = PlayCard("LonelyCalling");
            GoToStartOfTurn(nightlore);
            AssertInTrash(calling);

        }
        [Test()]
        public void TestLonelyCalling_DestroyAtStart_CriteriaNotMet()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            Card calling = PlayCard("LonelyCalling");
            PlayCard("Mere");
            GoToStartOfTurn(nightlore);
            AssertIsInPlay(calling);

        }

        [Test()]
        public void TestLonelyCalling_KeywordRestricting()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            Card solarFlare = PutInHand("SolarFlare");
            Card flameBarrier = PutInHand("FlameBarrier");
            Card staff = PutInHand("TheStaffOfRa");

            Card calling = PlayCard("LonelyCalling");
            PlayCard("Mere");
            PlayCard(flameBarrier);
            AssertInHand(flameBarrier);
            PlayCard(staff);
            AssertInHand(staff);
            PlayCard(solarFlare);
            AssertInPlayArea(ra, solarFlare);

            //keyword should have changed

            Card ring = PutInHand("TheLegacyRing");
            Card inspiring = PutInHand("InspiringPresence");
            Card danger = PutInHand("DangerSense");
            PlayCard(danger);
            AssertInHand(danger);
            PlayCard(inspiring);
            AssertInHand(inspiring);
            PlayCard(ring);
            AssertInPlayArea(legacy, ring);

        }

        [Test()]
        public void TestAethiumCannon_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(nightlore);
            Card cannon = PlayCard("AethiumCannon");
            Card ra1 = GetRandomCardFromHand(ra);
            Card haka1 = GetRandomCardFromHand(haka);
            DecisionSelectCards = new Card[] { ra1, null, haka1 };
            // At the end of the environment turn, each player may put 1 card from their hand beneath this one. Cards beneath this one are not considered in play.
            GoToEndOfTurn(nightlore);
            AssertUnderCard(cannon, ra1);
            AssertUnderCard(cannon, haka1);
            AssertNumberOfCardsUnderCard(cannon, 2);

        }

        [Test()]
        public void TestAethiumCannon_Fires()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(nightlore);
            Card cannon = PlayCard("AethiumCannon");

            IEnumerable<Card> cardsToMove = FindCardsWhere(c => legacy.TurnTaker.Deck.HasCard(c)).Take(7);
            MoveCards(nightlore, cardsToMove, cannon.UnderLocation);
            AssertNumberOfCardsUnderCard(cannon, 7);
            IEnumerable<Card> cardsUnder = FindCardsWhere(c => cannon.UnderLocation.HasCard(c));
            QuickHPStorage(baron, ra, legacy, haka);
            DecisionSelectTarget = baron.CharacterCard;
            GoToEndOfTurn(nightlore);
            QuickHPCheck(-15, 0, 0, 0);
            AssertInTrash(cardsUnder);
            AssertNumberOfCardsUnderCard(cannon, 1);
        }

        [Test()]
        public void TestAethiumRage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(baron, 1);
            SetHitPoints(battalion, 1);
            DecisionSelectTarget = battalion;
            DecisionAutoDecideIfAble = true;
            AssertNotDamageSource(baron.CharacterCard);
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            //When this card enters play, the villain target with the highest HP regains {H} HP and deals the 2 non-villain targets with the highest HP 3 radiant damage each.",
            Card rage = PlayCard("AethiumRage");
            QuickHPCheck(0, 3, 0, -3, -3);
            //Then, destroy this card.
            AssertInTrash(rage);
        }

        [Test()]
        public void TestArtemisVector_IncreaseDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card artemis = PlayCard("ArtemisVector");
            Card battalion = PlayCard("BladeBattalion");
            // Increase damage dealt to villain targets with Constellations next to them by 1.
            DecisionSelectCards = new Card[] { baron.CharacterCard, ra.CharacterCard };
            DecisionAutoDecideIfAble = true;
            PlayCard("AncientConstellationA");
            PlayCard("AncientConstellationB");
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, starlight.CharacterCard);
            DealDamage(ra, (Card c) => c.IsTarget, 2, DamageType.Fire);
            QuickHPCheck(-3, -2, -2, -2, -2);
        }

        [Test()]
        public void TestArtemisVector_WhenDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Cauldron.Necro", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card artemis = PlayCard("ArtemisVector");
            Card battalion = PlayCard("BladeBattalion");
            Card abomination = PlayCard("Abomination");
            // When this card is destroyed, each hero target deals themselves 2 psychic damage and each player draws a card.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, necro.CharacterCard, abomination);
            QuickHandStorage(ra, legacy, necro);
            DestroyCard(artemis, baron.CharacterCard);
            QuickHandCheck(1, 1, 1);
            QuickHPCheck(0, 0, -2, -2, -2, -2);
        }

        [Test()]
        public void TestAssembleTheCouncil_NonOros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Cauldron.Necro", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card assemble = PutInTrash("AssembleTheCouncil");
            Card ghoul = PlayCard("Ghoul");
            IEnumerable<Card> nonTargets = FindCardsWhere(c => nightlore.TurnTaker.Deck.HasCard(c) && !c.IsTarget).Take(3);
            Card target = PutOnDeck("ArtemisVector");
            PutOnDeck(nightlore, nonTargets);

            //When this card enters play, reveal cards from the top of the environment deck until a target is revealed, put it into play, and discard the other revealed cards. 
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, necro.CharacterCard, ghoul, target);
            PlayCard(assemble);
            QuickHPCheckZero();
            AssertInPlayArea(nightlore, target);
            AssertInTrash(nonTargets);
            AssertInTrash(assemble);
        }

        [Test()]
        public void TestAssembleTheCouncil_Oros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Cauldron.Necro", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card assemble = PutInTrash("AssembleTheCouncil");
            Card ghoul = PlayCard("Ghoul");
            IEnumerable<Card> nonTargets = FindCardsWhere(c => nightlore.TurnTaker.Deck.HasCard(c) && !c.IsTarget).Take(3);
            Card target = PutOnDeck("StarlightOfOros");
            PutOnDeck(nightlore, nonTargets);

            //When this card enters play, reveal cards from the top of the environment deck until a target is revealed, put it into play, and discard the other revealed cards. 
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, necro.CharacterCard, ghoul, target);
            PlayCard(assemble);
            QuickHPCheck(-1, -1, -1, -1, -1, 0);
            AssertInPlayArea(nightlore, target);
            AssertInTrash(nonTargets);
            AssertInTrash(assemble);
        }

        [Test()]
        public void TestCitadelGarrison_NoCannonOrOros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card garrison = PlayCard("CitadelGarrison");
            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            QuickHPStorage(ra, legacy, haka);
            GoToStartOfTurn(nightlore);
            QuickHPCheck(0, -4, 0);
        }

        [Test()]
        public void TestCitadelGarrison_CannonNoOros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card garrison = PlayCard("CitadelGarrison");
            Card cannon = PlayCard("AethiumCannon");
            MoveCard(nightlore, GetRandomCardFromHand(ra), cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            MoveCard(nightlore, GetRandomCardFromHand(legacy), cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            MoveCard(nightlore, GetRandomCardFromHand(haka), cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());

            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            QuickHPStorage(ra, legacy, haka);
            GoToStartOfTurn(nightlore);
            QuickHPCheck(0, -4, 0);

            //Then, if Starlight of Oros and Aethium Cannon are in play, discard 2 cards from beneath Aethium Cannon.
            //should not trigger
            AssertNumberOfCardsUnderCard(cannon, 3);
        }

        [Test()]
        public void TestCitadelGarrison_CannonAndOros_MoreThan2Under()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card garrison = PlayCard("CitadelGarrison");
            Card cannon = PlayCard("AethiumCannon");
            Card raRandom = GetRandomCardFromHand(ra);
            Card legacyRandom = GetRandomCardFromHand(legacy);
            Card hakaRandom = GetRandomCardFromHand(haka);

            MoveCard(nightlore, raRandom, cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            MoveCard(nightlore, legacyRandom, cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            MoveCard(nightlore, hakaRandom, cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            Card oros = PlayCard("StarlightOfOros");

            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            QuickHPStorage(ra, legacy, haka);
            DecisionSelectCards = new Card[] { raRandom, hakaRandom };
            GoToStartOfTurn(nightlore);
            QuickHPCheck(0, -4, 0);

            //Then, if Starlight of Oros and Aethium Cannon are in play, discard 2 cards from beneath Aethium Cannon.
            AssertNumberOfCardsUnderCard(cannon, 1);
            AssertInTrash(ra, raRandom);
            AssertInTrash(haka, hakaRandom);
            AssertUnderCard(cannon, legacyRandom);
        }

        [Test()]
        public void TestCitadelGarrison_CannonAndOros_1Under()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card garrison = PlayCard("CitadelGarrison");
            Card cannon = PlayCard("AethiumCannon");
            Card raRandom = GetRandomCardFromHand(ra);
            Card hakaRandom = GetRandomCardFromHand(haka);

            MoveCard(nightlore, raRandom, cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            Card oros = PlayCard("StarlightOfOros");

            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            QuickHPStorage(ra, legacy, haka);
            DecisionSelectCards = new Card[] { raRandom, hakaRandom };
            GoToStartOfTurn(nightlore);
            QuickHPCheck(0, -4, 0);

            //Then, if Starlight of Oros and Aethium Cannon are in play, discard 2 cards from beneath Aethium Cannon.
            AssertNumberOfCardsUnderCard(cannon, 0);
            AssertInTrash(ra, raRandom);
        }

        [Test()]
        public void TestCitadelGarrison_CannonAndOros_2Under()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card garrison = PlayCard("CitadelGarrison");
            Card cannon = PlayCard("AethiumCannon");
            Card raRandom = GetRandomCardFromHand(ra);
            Card hakaRandom = GetRandomCardFromHand(haka);

            MoveCard(nightlore, raRandom, cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            MoveCard(nightlore, hakaRandom, cannon.UnderLocation, cardSource: FindCardController(cannon).GetCardSource());
            Card oros = PlayCard("StarlightOfOros");

            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            QuickHPStorage(ra, legacy, haka);
            DecisionSelectCards = new Card[] { raRandom, hakaRandom };
            GoToStartOfTurn(nightlore);
            QuickHPCheck(0, -4, 0);

            //Then, if Starlight of Oros and Aethium Cannon are in play, discard 2 cards from beneath Aethium Cannon.
            AssertNumberOfCardsUnderCard(cannon, 0);
            AssertInTrash(ra, raRandom);
            AssertInTrash(haka, hakaRandom);
        }

        [Test()]
        public void TestCitadelGarrison_CannonAndOros_0Under()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card garrison = PlayCard("CitadelGarrison");
            Card cannon = PlayCard("AethiumCannon");

            Card oros = PlayCard("StarlightOfOros");

            //At the start of the environment turn, this card deals the hero target with the second highest HP {H + 1} radiant damage.
            QuickHPStorage(ra, legacy, haka);
            GoToStartOfTurn(nightlore);
            QuickHPCheck(0, -4, 0);

            //Then, if Starlight of Oros and Aethium Cannon are in play, discard 2 cards from beneath Aethium Cannon.
            AssertNumberOfCardsUnderCard(cannon, 0);
        }

        [Test()]
        public void TestGravityFluctuation_DealDamage()
        {
            SetupGameController("BaronBlade", "SkyScraper", "Legacy", "Haka", "TheSentinels", "Cauldron.Titan", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            DiscardAllCards(haka);
            DiscardAllCards(titan);
            DiscardAllCards(sky);

            // When this card enters play, it deals each hero with more than 3 cards in their hand 2 irreducible melee damage. 
            //One hero that was dealt no damage this way may deal 1 target 3 melee damage.
            QuickHPStorage(baron.CharacterCard, sky.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, medico, idealist, writhe, titan.CharacterCard);
            DecisionSelectCards = new Card[] { haka.CharacterCard, baron.CharacterCard };
            DecisionAutoDecideIfAble = true;
            AssertNumberOfChoicesInNextDecision(3, SelectionType.HeroToDealDamage);
            PlayCard("GravityFluctuation");
            //gravity fluctuation reduces damage dealt by 1
            QuickHPCheck(-2, 0, -2, 0, -2, -2, -2, -2, 0);
        }

        [Test()]
        public void TestGravityFluctuation_DestroyAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "TheSentinels", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Destroy this card at the start of the environment turn

            Card gravity = PlayCard("GravityFluctuation");
            AssertInPlayArea(nightlore, gravity);
            GoToStartOfTurn(nightlore);
            AssertInTrash(gravity);
        }

        [Test()]
        public void TestRogueConstellations_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.NightloreCitadel", "Cauldron.FSCContinuanceWanderer", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            SwitchBattleZone(oblivaeon);
            //When this card enters play, move it next to the villain target with the highest HP.
            //since there are no heroes in this battlezone, it should go to the trash
            Card constellation = PlayCard("RogueConstellation");
            AssertInTrash(constellation);

        }

        [Test()]
        public void TestRogueConstellation()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(baron, 3);
            //When this card enters play, move it next to the villain target with the highest HP.
            //Play the top card of the environment deck.",
            Card top = PutOnDeck("AethiumCannon");

            Card rogue = PlayCard("RogueConstellation");
            AssertNextToCard(rogue, battalion);
            AssertInPlayArea(nightlore, top);

            DestroyCard(battalion, ra.CharacterCard);
            AssertInTrash(rogue);
        }

        [Test()]
        public void TestStarlightOfNoome()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(baron, 23);
            SetHitPoints(ra, 23);
            SetHitPoints(legacy, 20);
            SetHitPoints(haka, 27);
            GoToPlayCardPhase(nightlore);
            PlayCard("StarlightOfNoome");
            //At the end of the environment turn, select the 2 non - environment targets with the lowest HP.This card deals 1 of those targets 2 melee damage, and the other target regains 2HP.
            DecisionSelectCards = new Card[] { baron.CharacterCard, legacy.CharacterCard };
            QuickHPStorage(baron, ra, legacy, haka);
            GoToEndOfTurn(nightlore);
            QuickHPCheck(2, 0, -2, 0);
        }

        [Test()]
        public void TestStarlightOfNoome_CannotDealDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(baron, 23);
            SetHitPoints(ra, 23);
            SetHitPoints(legacy, 20);
            SetHitPoints(haka, 27);
            GoToPlayCardPhase(nightlore);
            Card noome = PlayCard("StarlightOfNoome");
            AddCannotDealDamageTrigger(haka, noome);
            //At the end of the environment turn, select the 2 non - environment targets with the lowest HP.This card deals 1 of those targets 2 melee damage, and the other target regains 2HP.
            DecisionSelectCards = new Card[] { baron.CharacterCard, legacy.CharacterCard };
            QuickHPStorage(baron, ra, legacy, haka);
            GoToEndOfTurn(nightlore);
            QuickHPCheck(2, 0, 0, 0);
        }

        [Test()]
        public void TestStarlightOfOros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            SetHitPoints(baron, 23);
            DestroyNonCharacterVillainCards();
            DecisionSelectCard = baron.CharacterCard;
            PlayCard("AncientConstellationA");
            Card artemis = PlayCard("ArtemisVector");
            Card oros = PlayCard("StarlightOfOros");
            Card battalion = PlayCard("BladeBattalion");
            GoToPlayCardPhase(nightlore);
            //At the end of the environment turn, this card deals each hero target 2 infernal damage and each other environment target 2 psychic damage.
            //Then, each villain target next to a Constellation regains {H} HP.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, starlight.CharacterCard, artemis, oros);
            GoToEndOfTurn(nightlore);
            //startlight is a nemesis of oros so +1
            QuickHPCheck(4, 0, -2, -2, -2, -3, -2, 0);
        }

        [Test()]
        public void TestStarlightOfZzeck_NoCannon()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            GoToPlayCardPhase(nightlore);
            PlayCard("StarlightOfZzeck");
            //At the end of the environment turn, this card deals the villain target with the lowest HP 3 toxic damage.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, starlight.CharacterCard);
            GoToEndOfTurn(nightlore);
            QuickHPCheck(0, -3, 0, 0, 0, 0);
        }

        [Test()]
        public void TestStarlightOfZzeck_WithCannon()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            GoToPlayCardPhase(nightlore);
            PlayCard("StarlightOfZzeck");
            Card cannon = PlayCard("AethiumCannon");
            PreventEndOfTurnEffects(baron, cannon);
            Card top = haka.TurnTaker.Deck.TopCard;
            //At the end of the environment turn, this card deals the villain target with the lowest HP 3 toxic damage.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, starlight.CharacterCard);
            DecisionSelectLocation = new LocationChoice(haka.TurnTaker.Deck);
            GoToEndOfTurn(nightlore);
            QuickHPCheck(0, -3, 0, 0, 0, 0);
            AssertUnderCard(cannon, top);
        }

        [Test()]
        public void TestTalinBrosk()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(nightlore);
            PlayCard("TalinBrosk");
            //At the end of the environment turn, this card deals 1 other target 3 sonic damage.
            //Increase damage dealt by a target damaged this way by 1 until the start of the next environment turn.
            DecisionSelectTarget = haka.CharacterCard;
            QuickHPStorage(baron, ra, legacy, haka, starlight);
            GoToEndOfTurn(nightlore);
            QuickHPCheck(0, 0, 0, -3, 0);

            //check for increase
            QuickHPStorage(baron);
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheck(-4);

            //only haka
            QuickHPUpdate();
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-3);

            //expires at start of next turn
            GoToStartOfTurn(nightlore);
            QuickHPUpdate();
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }


        [Test()]
        public void TestUrgentMission()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(new Card[] { ra.CharacterCard, legacy.CharacterCard, starlight.CharacterCard }, 10);
            //Play this card next to the hero with the highest HP.
            Card mission = PlayCard("UrgentMission");
            Card envTarget = PlayCard("StarlightOfZzeck");
            Card turret = PlayCard("RegressionTurret");
            AssertNextToCard(mission, luminary.CharacterCard);
            //Targets in this play area cannot deal damage and are immune to damage dealt by environment cards.
            QuickHPStorage(baron);
            DealDamage(luminary, baron, 5, DamageType.Melee);
            QuickHPCheckZero();

            QuickHPUpdate();
            DealDamage(turret, baron, 5, DamageType.Melee);
            QuickHPCheckZero();

            QuickHPStorage(luminary.CharacterCard, turret);
            DealDamage(baron, luminary, 2, DamageType.Melee);
            DealDamage(baron, turret, 2, DamageType.Melee);
            QuickHPCheck(-2, -2);

            QuickHPUpdate();
            DealDamage(envTarget, luminary, 2, DamageType.Melee);
            DealDamage(envTarget, turret, 2, DamageType.Melee);
            QuickHPCheckZero();

            QuickHandStorage(luminary);
            GoToStartOfTurn(nightlore);
            QuickHandCheck(2);
            AssertInTrash(mission);

        }

        [Test()]
        public void TestUrgentMission_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.NightloreCitadel", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash
            Card mission = PlayCard("UrgentMission");
            AssertInTrash(mission);

        }

        [Test()]
        public void TestWarpBridge_NoRogueConstellation()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card turret = PlayCard("RegressionTurret");
            PlayCard("FlameBarrier");
            StackDeckAfterShuffle(luminary, new string[] { "BacklashGenerator" });
            //At the end of the environment turn, select 1 non-character card in play other than this one and shuffle it back into its associated deck.
            //If a card leaves play this way, play the top card of the associated deck. 
            GoToPlayCardPhase(nightlore);
            Card bridge = PlayCard("WarpBridge");
            DecisionSelectCard = turret;
            QuickShuffleStorage(luminary.TurnTaker.Deck);
            GoToEndOfTurn(nightlore);
            AssertInDeck(turret);
            QuickShuffleCheck(1);
            AssertIsInPlay("BacklashGenerator");
            AssertIsInPlay(bridge);


        }

        [Test()]
        public void TestWarpBridge_Issue1016_NonRealCards()
        {
            SetupGameController("KaargraWarfang", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            PutOnDeck("SoulslayerPerith");
            GoToPlayCardPhase(nightlore);
            DestroyNonCharacterVillainCards();
            MoveAllCards(warfang, warfang.TurnTaker.FindSubPlayArea("TitleDeck"), warfang.TurnTaker.FindSubDeck("TitleDeck"));
            Card barrier = PlayCard("FlameBarrier");
            Card gladiator = PlayCard("AshclawRVelos");
            Card turret = PlayCard("RegressionTurret");
            StackDeckAfterShuffle(luminary, new string[] { "BacklashGenerator" });

            //At the end of the environment turn, select 1 non-character card in play other than this one and shuffle it back into its associated deck.
            //If a card leaves play this way, play the top card of the associated deck. 
            Card bridge = PlayCard("WarpBridge");
            QuickShuffleStorage(luminary.TurnTaker.Deck);
            DecisionSelectCard = turret;
            //there should be 3 choices - a gladiator, flame barrier, and turret
            //this should ignore "The Crowd's Favor" which is a non-real, non-Character card
            AssertNumberOfChoicesInNextDecision(3);
            GoToEndOfTurn(nightlore);
            AssertInDeck(turret);
            QuickShuffleCheck(1);
            AssertIsInPlay(barrier);
            AssertIsInPlay(gladiator);
            AssertIsInPlay("BacklashGenerator");
            AssertIsInPlay(bridge);


        }

        [Test()]
        public void TestWarpBridge_Issue1394_CardsUnderCards()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();

            //get another valid target in play to force a decision
            PlayCard("TheLegacyRing");

            GoToPlayCardPhase(ladyWood);
            MoveCards(ladyWood, c => c.DoKeywordsContain("season") && c.Location.IsDeck, ladyWood.TurnTaker.Trash);
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            //At the end of the environment turn, select 1 non-character card in play other than this one and shuffle it back into its associated deck.
            //If a card leaves play this way, play the top card of the associated deck. 
            Card bridge = PlayCard("WarpBridge");
            //there should be 5 choices - rebirth and the 3 cards under it + the legacy ring
            AssertNumberOfChoicesInNextDecision(5);
            GoToEndOfTurn(nightlore);
        }

        [Test()]
        public void TestWarpBridge_NoRogueConstellation_IndestructibleCard()
        {
            SetupGameController("Cauldron.PhaseVillain", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card turret = PlayCard("RegressionTurret");
            Card wall = PlayCard("ReinforcedWall");
            PlayCard("FlameBarrier");
            //At the end of the environment turn, select 1 non-character card in play other than this one and shuffle it back into its associated deck.
            //If a card leaves play this way, play the top card of the associated deck. 
            GoToPlayCardPhase(nightlore);
            Card bridge = PlayCard("WarpBridge");
            DecisionSelectCard = wall;
            QuickShuffleStorage(phase.TurnTaker.Deck);
            int numCardsInPlay = GetNumberOfCardsInPlay(phase);

            GoToEndOfTurn(nightlore);
            AssertInPlayArea(phase, wall);
            QuickShuffleCheck(0);
            AssertNumberOfCardsInPlay(phase, numCardsInPlay);
            AssertIsInPlay(bridge);

        }

        [Test()]
        public void TestWarpBridge_WithRogueConstellation()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card rogue = PutInTrash("RogueConstellation");
            Card bridge = PutInTrash("WarpBridge");
            Card turret = PlayCard("RegressionTurret");
            PlayCard("FlameBarrier");
            StackDeckAfterShuffle(luminary, new string[] { "BacklashGenerator" });
            Card top = nightlore.TurnTaker.Deck.TopCard;
            PlayCard(rogue);

            //At the end of the environment turn, select 1 non-character card in play other than this one and shuffle it back into its associated deck.
            //If a card leaves play this way, play the top card of the associated deck. 
            //Then, if Rogue Constellation is in play, destroy this card.

            GoToPlayCardPhase(nightlore);
            PlayCard(bridge);
            DecisionSelectCard = turret;
            QuickShuffleStorage(luminary.TurnTaker.Deck);
            PreventEndOfTurnEffects(ra, top);
            GoToEndOfTurn(nightlore);
            AssertInDeck(turret);
            QuickShuffleCheck(1);
            AssertIsInPlay("BacklashGenerator");
            AssertInTrash(bridge);
        }

        [Test()]
        public void TestWatchedByTheStars_NoStarlightOfOros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //When this card enters play, it deals each villain target 2 radiant damage. 
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, luminary.CharacterCard, starlight.CharacterCard);
            Card stars = PlayCard("WatchedByTheStars");
            QuickHPCheck(-2, -2, 0, 0, 0, 0);
            //At the end of the environment turn, destroy this card.
            GoToPlayCardPhase(nightlore);
            AssertInPlayArea(nightlore, stars);
            GoToEndOfTurn(nightlore);
            AssertInTrash(stars);
        }

        [Test()]
        public void TestWatchedByTheStars_WithStarlightOfOros()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Luminary", "Cauldron.Starlight", "Cauldron.NightloreCitadel");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            Card turret = PlayCard("RegressionTurret");
            PlayCard("StarlightOfOros");
            //When this card enters play, it deals each villain target 2 radiant damage. 
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, luminary.CharacterCard, turret, starlight.CharacterCard);
            Card stars = PlayCard("WatchedByTheStars");
            QuickHPCheck(0, 0, -2, -2, -2, -2, -2);
            //At the end of the environment turn, destroy this card.
            GoToPlayCardPhase(nightlore);
            AssertInPlayArea(nightlore, stars);
            GoToEndOfTurn(nightlore);
            AssertInTrash(stars);
        }

    }
}

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Cricket;
using System.Collections;

namespace CauldronTests
{
    [TestFixture()]
    public class CricketTests : CauldronBaseTest
    {

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(cricket.CharacterCard, 1);
            DealDamage(villain, cricket, 2, DamageType.Melee);
        }

        [Test()]
        public void TestLoadCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(CricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(27, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestCricketInnatePower()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Select a target. Reduce damage dealt by that target by 1 until the start of your next turn.
            UsePower(cricket);

            QuickHPStorage(cricket, legacy, bunker);
            DealDamage(apostate, cricket, 2, DamageType.Melee);
            DealDamage(apostate, legacy, 2, DamageType.Cold);
            DealDamage(apostate, bunker, 2, DamageType.Fire);
            QuickHPCheck(-1, -1, -1);

            GoToStartOfTurn(cricket);
            //Reduce damage goes away at start of turn
            QuickHPStorage(cricket, legacy, bunker);
            DealDamage(apostate, cricket, 2, DamageType.Melee);
            DealDamage(apostate, legacy, 2, DamageType.Cold);
            DealDamage(apostate, bunker, 2, DamageType.Fire);
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestCricketIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //One player may draw a card now.
            QuickHandStorage(legacy);
            UseIncapacitatedAbility(cricket, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestCricketIncap2Deck()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            Card ring = PutOnDeck("TheLegacyRing");
            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Deck);

            //Look at the top card of a deck and replace or discard it.
            UseIncapacitatedAbility(cricket, 1);
            AssertOnTopOfDeck(ring);
            AssertNumberOfCardsInRevealed(cricket, 0);

        }

        [Test()]
        public void TestCricketIncap2Trash()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            Card ring = PutOnDeck("TheLegacyRing");
            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Trash);

            //Look at the top card of a deck and replace or discard it.
            UseIncapacitatedAbility(cricket, 1);
            AssertOnTopOfTrash(legacy, ring);
            AssertNumberOfCardsInRevealed(cricket, 0);
        }

        [Test()]
        public void TestCricketIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            Card sword = GetCardInPlay("Condemnation");
            //Each target deals themselves 1 sonic damage.
            QuickHPStorage(apostate.CharacterCard, sword, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(cricket, 2);
            QuickHPCheck(-1, 0, -1, -1, -1);
        }

        [Test()]
        public void TestAcousticDistortion()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("AcousticDistortion");
            //Once per turn when a hero target would be dealt damage, you may redirect that damage to another hero target.

            //Optional Redirect
            QuickHPStorage(cricket);
            DealDamage(apostate, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Choose to redirect
            DecisionRedirectTarget = legacy.CharacterCard;
            QuickHPStorage(legacy, cricket);
            DealDamage(apostate, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //Once per turn
            QuickHPStorage(legacy, cricket);
            DealDamage(apostate, cricket, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

            //Can do again next turn
            GoToStartOfTurn(cricket);
            QuickHPStorage(legacy, cricket);
            DealDamage(apostate, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //can redirect away from any hero target
            GoToNextTurn();
            QuickHPStorage(legacy, bunker);
            DealDamage(apostate, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestBeforeTheThunder()
        {
            SetupGameController("Apostate", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card revealed = apostate.TurnTaker.Deck.TopCard;

            //Reveal the top card of 1 deck, then replace it.
            AssertNextMessage("Revealed card: " + revealed.Title);
            //Draw 3 cards
            QuickHandStorage(cricket);
            PlayCard("BeforeTheThunder");
            QuickHandCheck(3);
            //Make sure the revealed card was put back
            AssertOnTopOfDeck(revealed);
            AssertNumberOfCardsInRevealed(apostate, 0);
        }

        [Test()]
        public void TestChirp0Cards()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card phlange = PlayCard("ArborealPhalanges");
            Card bramb = PlayCard("EnsnaringBrambles");
            Card rocks = PlayCard("LivingRockslide");
            Card chirp = PutInTrash("Chirp");
            //Discard up to 3 cards.
            QuickHandStorage(cricket);
            QuickHPStorage(phlange, bramb, rocks, akash.CharacterCard);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            PlayCard(chirp);
            QuickHandCheck(0);
            //{Cricket} deals up to 4 targets X sonic damage each, where X is 1 plus the number of cards discarded this way.
            QuickHPCheck(-1, -1, -1, -1);
        }

        [Test()]
        public void TestChirp3Cards()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card phlange = PlayCard("ArborealPhalanges");
            Card bramb = PlayCard("EnsnaringBrambles");
            Card rocks = PlayCard("LivingRockslide");

            DecisionYesNo = true;

            //Discard up to 3 cards.
            QuickHandStorage(cricket);
            QuickHPStorage(phlange, bramb, rocks, akash.CharacterCard);
            PlayCard("Chirp");
            QuickHandCheck(-3);
            //{Cricket} deals up to 4 targets X sonic damage each, where X is 1 plus the number of cards discarded this way.
            QuickHPCheck(-4, -4, -4, -4);
        }

        [Test()]
        public void TestChirp3Cards_UpTo()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card phlange = PlayCard("ArborealPhalanges");
            Card bramb = PlayCard("EnsnaringBrambles");
            Card rocks = PlayCard("LivingRockslide");

            DecisionYesNo = true;

            //Discard up to 3 cards.
            QuickHandStorage(cricket);
            QuickHPStorage(phlange, bramb, rocks, akash.CharacterCard);
            DecisionSelectTargets = new Card[] { phlange, bramb, rocks, null };
            PlayCard("Chirp");
            QuickHandCheck(-3);
            //{Cricket} deals up to 4 targets X sonic damage each, where X is 1 plus the number of cards discarded this way.
            QuickHPCheck(-4, -4, -4, -0);
        }

        [Test()]
        public void TestEchonavigation()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(akash.TurnTaker.Trash), new MoveCardDestination(cricket.TurnTaker.Trash), new MoveCardDestination(legacy.TurnTaker.Trash), new MoveCardDestination(bunker.TurnTaker.Deck), new MoveCardDestination(scholar.TurnTaker.Deck), new MoveCardDestination(env.TurnTaker.Deck) };

            Card topAkash = akash.TurnTaker.Deck.TopCard;
            Card topCricket = cricket.TurnTaker.Deck.TopCard;
            Card topLegacy = legacy.TurnTaker.Deck.TopCard;
            Card topBunker = bunker.TurnTaker.Deck.TopCard;
            Card topScholar = scholar.TurnTaker.Deck.TopCard;
            Card topEnv = env.TurnTaker.Deck.TopCard;

            Card echo = PutInHand("Echonavigation");

            //One player may draw a card now.
            QuickHandStorage(cricket);
            DecisionSelectCard = cricket.CharacterCard;
            PlayCard(echo);
            //played echo from hand, drew a card, net 0
            QuickHandCheck(0);
            //Reveal the top card of each deck. You may replace or discard each card.
            AssertOnTopOfTrash(akash, topAkash);
            //Echonavigation gets put on top
            AssertInTrash(cricket, topCricket);
            AssertOnTopOfTrash(legacy, topLegacy);
            AssertOnTopOfDeck(topBunker);
            AssertOnTopOfDeck(topScholar);
            AssertOnTopOfDeck(topEnv);
        }

        [Test]
        public void TestEchonavigationOblivAeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToStartOfTurn(cricket);
            AssertNumberOfChoicesInNextDecision(5, SelectionType.RevealTopCardOfDeck);
            PlayCard("Echonavigation");

        }

        [Test()]
        public void TestEnhancedHearing()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(bunker.TurnTaker.Deck), new LocationChoice(env.TurnTaker.Deck) };
            Card hear = PlayCard("EnhancedHearing");
            //At the start of your turn, reveal the top card of 2 different decks, then replace them.
            GoToStartOfTurn(cricket);
            AssertNumberOfCardsInRevealed(bunker, 0);
            AssertNumberOfCardsInRevealed(env, 0);
            //Increase sonic damage dealt to {Cricket} by 1.
            QuickHPStorage(cricket);
            DealDamage(akash, cricket, 2, DamageType.Sonic);
            QuickHPCheck(-3);

            //check only sonic damage
            QuickHPUpdate();
            DealDamage(akash, cricket, 2, DamageType.Fire);
            QuickHPCheck(-2);

            //check only cricket
            QuickHPStorage(bunker);
            DealDamage(akash, bunker, 2, DamageType.Sonic);
            QuickHPCheck(-2);

            //Power: Destroy this card.
            UsePower(hear);
            AssertInTrash(hear);
        }

        [Test()]
        public void TestGrasshopperKick()
        {
            SetupGameController(new string[] { "AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            Card kick = PlayCard("GrasshopperKick");
            Card cramped = GetCard("CrampedQuartersCombat");
            //{Cricket} deals 1 target 2 melee damage. {Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            QuickHPStorage(akash);
            UsePower(kick);
            QuickHPCheck(-2);

            Card rail = PlayCard("PlummetingMonorail");
            //{Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            QuickHPStorage(cricket);
            DealDamage(rail, cricket, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Non targets and non-environment deal damage
            Card hostage = PlayCard("HostageSituation");
            DealDamage(hostage, cricket, 2, DamageType.Melee);
            DealDamage(akash, cricket, 2, DamageType.Melee);
            if (cramped.IsInPlayAndHasGameText)
            {
                QuickHPCheck(-6);
            }
            else
            {
                QuickHPCheck(-4);
            }

            GoToStartOfTurn(cricket);
            //Until Start of next turn
            QuickHPStorage(cricket);
            if (!rail.IsInPlayAndHasGameText)
            {
                PlayCard(rail);
            }
            DealDamage(rail, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestGrasshopperKick_EnragedTerrorBird()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Megalopolis", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            MoveCard(oblivaeon, "EnragedTerrorBird", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));
            DecisionSelectFunction = 0;
            GoToBeforeStartOfTurn(cricket);
            RunActiveTurnPhase();
            Card bird = GetCardInPlay("EnragedTerrorBird");
            GoToPlayCardPhase(cricket);
            Card kick = PlayCard("GrasshopperKick");
            UsePower(kick);

            //cricket should be immune to environment damage
            QuickHPStorage(cricket);
            DealDamage(bird, cricket, 3, DamageType.Projectile);
            QuickHPCheckZero();

        }

        [Test()]
        public void TestGrasshopperKick_Destroyed()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            Card kick = PlayCard("GrasshopperKick");
            Card cramped = GetCard("CrampedQuartersCombat");
            //{Cricket} deals 1 target 2 melee damage. {Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            UsePower(kick);

            Card rail = PlayCard("PlummetingMonorail");
            DestroyCard(kick);
            //{Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            QuickHPStorage(cricket);
            DealDamage(rail, cricket, 2, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestGrasshopperKick_ReEnterPlay()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            Card kick = PlayCard("GrasshopperKick");
            Card cramped = GetCard("CrampedQuartersCombat");
            //{Cricket} deals 1 target 2 melee damage. {Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            UsePower(kick);

            Card rail = PlayCard("PlummetingMonorail");
            DestroyCard(kick);
            //{Cricket} is immune to damage dealt by environment targets until the start of your next turn.

            GoToEndOfTurn(cricket);
            PlayCard(kick);
            QuickHPStorage(cricket);
            DealDamage(rail, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestInfrasonicCollapseDestroyOngoing()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card entomb = PlayCard("Entomb");

            QuickHPStorage(akash);
            PlayCard("InfrasonicCollapse");
            //Destroy 1 ongoing or environment card.
            AssertInTrash(entomb);
            //If you destroyed an ongoing card this way, {Cricket} deals 1 target 2 sonic damage.
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestInfrasonicCollapseDestroyOngoing_UnableToDestroy()
        {
            SetupGameController("IronLegacy", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card galvanized = PlayCard("Galvanized");

            QuickHPStorage(iron);
            DecisionSelectCard = galvanized;
            PlayCard("InfrasonicCollapse");
            //Destroy 1 ongoing or environment card.
            AssertInPlayArea(iron, galvanized);
            //If you destroyed an ongoing card this way, {Cricket} deals 1 target 2 sonic damage.
            //galvanized is indestructible, so couldn't be destroyed, so no damage should be dealt
            QuickHPCheck(0);
        }

        [Test()]
        public void TestInfrasonicCollapseDestroyEnvironment()
        {
            SetupGameController(new string[] { "AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: new int?(-1486032106));
            StartGame();

            Card phlange = PlayCard("ArborealPhalanges");
            Card bramb = PlayCard("EnsnaringBrambles");
            Card rocks = PlayCard("LivingRockslide");
            Card rail0 = PlayCard("PlummetingMonorail", 0);
            Card rail1 = PlayCard("PlummetingMonorail", 1);

            QuickHPStorage(akash.CharacterCard, phlange, bramb, rocks, rail1, legacy.CharacterCard);
            DecisionSelectCard = rail0;
            DecisionAutoDecideIfAble = true;
            if (!rail0.IsInPlayAndHasGameText)
            {
                PlayCard(rail0, isPutIntoPlay: true);
            }
            PlayCard("InfrasonicCollapse");
            //Destroy 1 ongoing or environment card.
            AssertInTrash(rail0);
            //If you destroyed an environment card this way, {Cricket} deals each non-hero target 1 sonic damage.
            if (FindCardsWhere((Card c) => c.Identifier == "RooftopCombat" && c.IsInPlayAndHasGameText).Any() && !FindCardsWhere((Card c) => c.Identifier == "MountainousCarapace" && c.IsInPlayAndHasGameText).Any())
            {
                //the only way rooftop combat is in play is if disrupt the field came out, and played it
                //that would have destroyed the other monorail
                QuickHPCheck(-2, -2, -2, -2, 0, 0);
            }
            else if ((FindCardsWhere((Card c) => c.Identifier == "RooftopCombat" && c.IsInPlayAndHasGameText).Any() && FindCardsWhere((Card c) => c.Identifier == "MountainousCarapace" && c.IsInPlayAndHasGameText).Any()))
            {
                QuickHPCheck(-1, -2, -2, -2, 0, 0);
            }
            else if ((!FindCardsWhere((Card c) => c.Identifier == "RooftopCombat" && c.IsInPlayAndHasGameText).Any() && FindCardsWhere((Card c) => c.Identifier == "MountainousCarapace" && c.IsInPlayAndHasGameText).Any()))
            {
                QuickHPCheck(-0, -1, -1, -1, -1, 0);
            }
            else
            {
                QuickHPCheck(-1, -1, -1, -1, -1, 0);

            }

        }

        [Test()]
        public void TestReturnPulse1Target()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            MoveAllCardsFromHandToDeck(cricket);
            QuickHandStorage(cricket);
            QuickHPStorage(akash.CharacterCard);
            PlayCard("ReturnPulse");
            //{Cricket} deals up to 3 non-hero targets 1 sonic damage each.
            QuickHPCheck(-1);
            //For each target dealt damage this way, draw a card.
            QuickHandCheck(1);
        }
        [Test()]
        public void TestReturnPulse1Target_ImmuneToDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickHandStorage(cricket);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            PlayCard("ReturnPulse");
            //{Cricket} deals up to 3 non-hero targets 1 sonic damage each.
            //For each target dealt damage this way, draw a card.
            //baron is immune to damage, so a card should not have been drawn
            QuickHandCheck(0);
        }


        [Test()]
        public void TestReturnPulse3Target()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card rocks = PutOnDeck("LivingRockslide");
            Card rail0 = PlayCard("PlummetingMonorail");

            QuickHandStorage(cricket);
            QuickHPStorage(akash.CharacterCard, rocks, rail0);
            PlayCard("ReturnPulse");
            //{Cricket} deals up to 3 non-hero targets 1 sonic damage each.
            QuickHPCheck(-1, -1, -1);
            //For each target dealt damage this way, draw a card.
            QuickHandCheck(3);
        }

        [Test()]
        public void TestReturnPulse3Targets_UpTo()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card rocks = PutOnDeck("LivingRockslide");
            Card rail0 = PlayCard("PlummetingMonorail");

            QuickHandStorage(cricket);
            QuickHPStorage(akash.CharacterCard, rocks, rail0);
            DecisionSelectTargets = new Card[] { akash.CharacterCard, rocks, null };
            PlayCard("ReturnPulse");
            //{Cricket} deals up to 3 non-hero targets 1 sonic damage each.
            QuickHPCheck(-1, -1, 0);
            //For each target dealt damage this way, draw a card.
            QuickHandCheck(2);
        }

        [Test()]
        public void TestSilentStalker()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectPowers = new Card[] { cricket.CharacterCard, null };

            PlayCard("SilentStalker");
            //At the end of your turn, if {Cricket} dealt no damage this turn, you may use a power.
            GoToEndOfTurn(cricket);
            //Use Cricket's base power
            QuickHPStorage(legacy);
            DealDamage(akash, legacy, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSilentStalker_CricketDealtDamage()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectPowers = new Card[] { cricket.CharacterCard, null };
            GoToPlayCardPhase(cricket);
            PlayCard("SilentStalker");
            DealDamage(cricket, bunker, 2, DamageType.Sonic);
            //At the end of your turn, if {Cricket} dealt no damage this turn, you may use a power.
            GoToEndOfTurn(cricket);
            //power should not have been used
            QuickHPStorage(legacy);
            DealDamage(akash, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSilentStalker_OthersDealtDamage()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectPowers = new Card[] { cricket.CharacterCard, null };
            GoToPlayCardPhase(cricket);
            PlayCard("SilentStalker");
            DealDamage(bunker, cricket, 2, DamageType.Sonic);
            //At the end of your turn, if {Cricket} dealt no damage this turn, you may use a power.
            GoToEndOfTurn(cricket);
            //Use Cricket's base power
            QuickHPStorage(legacy);
            DealDamage(akash, legacy, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSonicAmplifier()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card amp = PlayCard("SonicAmplifier");
            Card top = cricket.TurnTaker.Deck.TopCard;
            DecisionYesNo = true;
            //Whenever {Cricket} deals sonic damage to a target, you may put the top card of your deck beneath this one. Cards beneath this one are not considered to be in play.
            DealDamage(cricket, akash, 2, DamageType.Sonic);
            AssertUnderCard(amp, top);

            //Power: Discard all cards beneath this one. {Cricket} deals 1 target X sonic damage, where X is the number of cards discarded this way.
            QuickHPStorage(akash);
            UsePower(amp);
            QuickHPCheck(-1);

            DealDamage(cricket, akash, 2, DamageType.Sonic);
            DealDamage(cricket, akash, 2, DamageType.Sonic);
            DealDamage(cricket, akash, 2, DamageType.Sonic);
            //the power usage puts one under this
            AssertNumberOfCardsUnderCard(amp, 4);

            QuickHPStorage(akash);
            UsePower(amp);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestSonicAmplifier_CardsUnderAreNotInPlay()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");
            StartGame();

            Card amp = PlayCard("SonicAmplifier");
            Card soundMasking = MoveCard(cricket, "SoundMasking", cricket.TurnTaker.Trash);
            Card swarmingFrequency = MoveCard(cricket, "SwarmingFrequency", cricket.TurnTaker.Trash);
            Card top = cricket.TurnTaker.Deck.TopCard;
            DecisionYesNo = true;
            //Whenever {Cricket} deals sonic damage to a target, you may put the top card of your deck beneath this one. Cards beneath this one are not considered to be in play.
            DealDamage(cricket, akash, 2, DamageType.Sonic);
            AssertUnderCard(amp, top);

            PlayCard(soundMasking);
            PlayCard(swarmingFrequency);

            AssertNextDecisionChoices(included: new List<Card>() {amp, soundMasking, swarmingFrequency }, notIncluded: new List<Card>() { top });
            PlayCard("LookingForThis");

            

           
        }

        [Test()]
        public void TestSonicAmplifier_Optional()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card amp = PlayCard("SonicAmplifier");
            Card top = cricket.TurnTaker.Deck.TopCard;
            DecisionYesNo = false;
            //Whenever {Cricket} deals sonic damage to a target, you may put the top card of your deck beneath this one. Cards beneath this one are not considered to be in play.
            DealDamage(cricket, akash, 2, DamageType.Sonic);
            AssertOnTopOfDeck(top);

        }
        [Test()]
        public void TestSonicAmplifier_IsDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "TombOfAnubis");
            StartGame();
            RemoveMobileDefensePlatform();

            Card amp = PlayCard("SonicAmplifier");
            DecisionYesNo = true;
            DealDamage(cricket, baron, 1, DamageType.Sonic);
            DealDamage(cricket, baron, 1, DamageType.Sonic);

            //if Sonic Amplifier destroys the cards under it, Anubis will notice and hit Legacy
            PlayCard("Anubis");
            QuickHPStorage(legacy);

            UsePower(amp);
            QuickHPCheckZero();

        }

        [Test()]
        public void TestSoundMasking()
        {
            SetupGameController(new List<string>() { "AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            StackDeck(akash, "LivingRockslide");
            StackDeck(env, "PoliceBackup");
            Card rail = PlayCard("PlummetingMonorail");
            
            Card mask = PlayCard("SoundMasking");

            //All targets are immune to damage.
            QuickHPStorage(akash.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, rail);
            DealDamage(akash, legacy, 2, DamageType.Sonic);
            DealDamage(cricket, akash, 2, DamageType.Sonic);
            DealDamage(cricket, bunker, 2, DamageType.Sonic);
            DealDamage(cricket, rail, 2, DamageType.Sonic);
            QuickHPCheckZero();

            //At the start of your turn, destroy this card.
            GoToStartOfTurn(cricket);
            AssertInTrash(mask);
        }

        [Test()]
        public void TestSubharmonicReceiver()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card sub = PlayCard("SubharmonicReceiver");
            DecisionYesNo = true;
            //Each player may draw a card. When a player draws a card this way, 1 other player must discard a card.
            QuickHandStorage(cricket, legacy, bunker, scholar);
            UsePower(sub);
            //All draw 1, Cricket -3 for other heroes, Legacy -1 for Cricket
            QuickHandCheck(-2, 0, 1, 1);
        }

        [Test()]
        public void TestSubharmonicReceiverSuperimposedRealities()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Cauldron.FSCContinuanceWanderer");
            StartGame();

            DecisionSelectCard = cricket.CharacterCard;
            PlayCard("SuperimposedRealities");
            ResetDecisions();
            Card sub = PlayCard("SubharmonicReceiver");
            DecisionYesNo = true;
            //Each player may draw a card. When a player draws a card this way, 1 other player must discard a card.
            QuickHandStorage(cricket, legacy, bunker, scholar);
            UsePower(sub);
            //Add draws bounce to Cricket, cricket's own draw forces legacy to discard
            QuickHandCheck(4, -1, 0, 0);
        }

        [Test()]
        public void TestSwarmingFrequency()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();

            PutOnDeck("ArborealPhalanges");
            PutOnDeck("EnsnaringBrambles");
            PutOnDeck("LivingRockslide");

            Card swarm = PlayCard("SwarmingFrequency");
            //If there is at least 1 environment target in play, redirect all damage dealt by villain targets to the environment target with the lowest HP.

            //No environment
            QuickHPStorage(cricket);
            DealDamage(akash, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2);
            PrintSpecialStringsForCard(swarm);

            Card defender = PlayCard("SeismicDefender");
            PrintSpecialStringsForCard(swarm);

            Card tunneler = PlayCard("InnerCoreTunneler");
            PrintSpecialStringsForCard(swarm);

            //Redirect to Lowest
            QuickHPStorage(cricket.CharacterCard, akash.CharacterCard, tunneler, defender);
            DealDamage(akash, cricket, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -2);

            QuickHPStorage(cricket.CharacterCard, akash.CharacterCard, tunneler, defender);
            DealDamage(akash, akash, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -2);

            QuickHPStorage(cricket.CharacterCard, akash.CharacterCard, tunneler, defender);
            DealDamage(akash, tunneler, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -2);

            //Only villain damage
            QuickHPStorage(cricket.CharacterCard, akash.CharacterCard, tunneler, defender);
            DealDamage(cricket, akash, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0);

            QuickHPStorage(cricket.CharacterCard, akash.CharacterCard, tunneler, defender);
            DealDamage(cricket, tunneler, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);

            QuickHPStorage(cricket.CharacterCard, akash.CharacterCard, tunneler, defender);
            DealDamage(tunneler, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);

            //At the start of your turn, destroy this card.
            GoToStartOfTurn(cricket);
            AssertInTrash(swarm);
        }

        [Test()]
        public void TestTelescopingStaff()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();

            Card sub = PutInHand("SubharmonicReceiver");
            DecisionSelectCard = sub;

            Card staff = PlayCard("TelescopingStaff");
            //{Cricket} deals 1 target 1 melee damage. You may play a card.
            QuickHPStorage(akash);
            UsePower(staff);
            QuickHPCheck(-1);
            AssertIsInPlay(sub);
        }

        [Test()]
        public void TestTelescopingStaff_Optional()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();

            Card sub = PutInHand("SubharmonicReceiver");
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            Card staff = PlayCard("TelescopingStaff");
            //{Cricket} deals 1 target 1 melee damage. You may play a card.
            QuickHPStorage(akash);
            UsePower(staff);
            QuickHPCheck(-1);
            AssertInHand(sub);
        }

        [Test()]
        public void TestVantagePoint()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();

            Card staff = PutInTrash("TelescopingStaff");
            Card ring = PutInTrash("TheLegacyRing");
            Card flak = PutInTrash("FlakCannon");
            Card loose = PutInTrash("KnowWhenToTurnLoose");

            Card vant = PlayCard("VantagePoint");
            //Each player may put a card other than Vantage Point from their trash into their hand. Destroy this card.
            UsePower(vant);
            AssertInHand(new Card[] { staff, ring, flak, loose });
            AssertInTrash(vant);
        }

        [Test()]
        [Sequential]
        public void TestVoiceMimicry([Values("ArborealPhalanges", "TheLegacyRing", "SeismicDefender")] string identifier)
        {
            SetupGameController(new[] { "AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Magmaria" });
            StartGame();
            PutOnDeck("LivingRockslide"); //to prevent chain plays

            Card card = PutOnDeck(identifier);
            DecisionSelectLocation = new LocationChoice(card.Owner.Deck);
            //Play the top card of a deck.
            PlayCard("VoiceMimicry");
            AssertIsInPlay(card);

        }

        [Test()]
        public void TestFirstResponsePromo()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Cauldron.Echelon", "Bunker", "TheScholar", "Cauldron.WindmillCity");
            StartGame();

            DealDamage(akash, cricket, cricket.CharacterCard.MaximumHitPoints.Value - 2, DamageType.Projectile);
            Assert.That(cricket.CharacterCard.HitPoints.Value, Is.LessThan(5));

            Func<IEnumerator> gainHpAction = () => GameController.GainHP(cricket.CharacterCard, 6, cardSource: echelon.CharacterCardController.GetCardSource());
            this.RunCoroutine(gainHpAction());
            this.RunCoroutine(gainHpAction());

            DealDamage(cricket, akash, akash.CharacterCard.HitPoints.Value + 5, DamageType.Sonic);
        }

        [Test()]
        public void TestRenegadePromo()
        {
            SetupGameController("Cauldron.Dynamo", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Cauldron.WindmillCity");

            StackDeck(dynamo, new[] { "HeresThePlan" }); // Dynamo discards the top card of his deck. Make sure it isn't the card we are testing.
            StartGame();

            Card pyt = PlayCard("Python");

            //The first time a hero target deals damage to this card each turn, reduce damage dealt by that target by 1 until the start of the next villain turn.
            DealDamage(cricket, pyt, 1, DamageType.Melee);

            QuickHPStorage(dynamo.CharacterCard, pyt, cricket.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(cricket, dynamo, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, 0);

            Card responder = PlayCard("WCPDSquad");

            DealDamage(responder, cricket, cricket.CharacterCard.MaximumHitPoints.Value + 2, DamageType.Projectile);
            AssertIncapacitated(cricket);

            DestroyCard(legacy);
            DestroyCard(bunker); 
            DestroyCard(scholar);

            AssertGameOver();

        }

        [Test()]
        public void TestWastelandRoninPromo()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket", "Cauldron.Impact", "Cauldron.Pyre", "Cauldron.TheStranger", "Cauldron.Gargoyle", "Cauldron.FSCContinuanceWanderer");

            StartGame();
            AssertPromoCardIsUnlockableThisGame("WastelandRoninCricketCharacter");
            AssertPromoCardIsUnlockableThisGame("WastelandRoninGargoyleCharacter");
            AssertPromoCardIsUnlockableThisGame("WastelandRoninImpactCharacter");
            AssertPromoCardIsUnlockableThisGame("WastelandRoninPyreCharacter");
            AssertPromoCardIsUnlockableThisGame("WastelandRoninTheStrangerCharacter");



            Card heartOfTheWanderer = PlayCard("HeartOfTheWanderer");

            AssertPromoCardNotUnlocked("WastelandRoninCricketCharacter");

            PlayCard("RogueFissionCascade");

            AssertPromoCardNotUnlocked("WastelandRoninCricketCharacter");

            AssertPromoCardUnlocked("WastelandRoninCricketCharacter");
            AssertPromoCardUnlocked("WastelandRoninGargoyleCharacter");
            AssertPromoCardUnlocked("WastelandRoninImpactCharacter");
            AssertPromoCardUnlocked("WastelandRoninPyreCharacter");
            AssertPromoCardUnlocked("WastelandRoninTheStrangerCharacter");

        }
    }
}

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Cricket;

namespace CauldronTests
{
    [TestFixture()]
    public class CricketTests : BaseTest
    {
        protected HeroTurnTakerController cricket { get { return FindHero("Cricket"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(cricket.CharacterCard, 1);
            DealDamage(villain, cricket, 2, DamageType.Melee);
        }

        [Test()]
        public void TestLoadCrickett()
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
        }

        [Test()]
        public void TestChirp0Cards()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card phlange = PlayCard("ArborealPhalanges");
            Card bramb = PlayCard("EnsnaringBrambles");
            Card rocks = PlayCard("LivingRockslide");

            //Discard up to 3 cards.
            QuickHandStorage(cricket);
            QuickHPStorage(phlange, bramb, rocks, akash.CharacterCard);
            PlayCard("Chirp");
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

            //One player may draw a card now.
            QuickHandStorage(cricket);
            PlayCard("Echonavigation");
            QuickHandCheck(1);
            //Reveal the top card of each deck. You may replace or discard each card.
            AssertOnTopOfTrash(akash, topAkash);
            //Echonavigation gets put on top
            AssertInTrash(cricket, topCricket);
            AssertOnTopOfTrash(legacy, topLegacy);
            AssertOnTopOfDeck(topBunker);
            AssertOnTopOfDeck(topScholar);
            AssertOnTopOfDeck(topEnv);
        }

        [Test()]
        public void TestEnhancedHearing()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card hear = PlayCard("EnhancedHearing");
            //At the start of your turn, reveal the top card of 2 different decks, then replace them.
            GoToStartOfTurn(cricket);
            //Increase sonic damage dealt to {Cricket} by 1.
            QuickHPStorage(cricket);
            DealDamage(akash, cricket, 2, DamageType.Sonic);
            QuickHPCheck(-3);
            //Power: Destroy this card.
            UsePower(hear);
            AssertInTrash(hear);
        }

        [Test()]
        public void TestGrasshopperKick()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card kick = PlayCard("GrasshopperKick");
            //{Cricket} deals 1 target 2 melee damage. {Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            QuickHPStorage(akash);
            UsePower(kick);
            QuickHPCheck(-2);

            Card rail = PlayCard("PlummetingMonorail");
            //{Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            QuickHPStorage(cricket);
            DealDamage(rail, cricket, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Non targets deal damage
            Card hostage = PlayCard("HostageSituation");
            DealDamage(hostage, cricket, 2, DamageType.Melee);
            QuickHPCheck(-2);

            GoToStartOfTurn(cricket);
            //Until Start of next turn
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
        public void TestInfrasonicCollapseDestroyEnvironment()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card phlange = PlayCard("ArborealPhalanges");
            Card bramb = PlayCard("EnsnaringBrambles");
            Card rocks = PlayCard("LivingRockslide");
            Card rail0 = PlayCard("PlummetingMonorail", 0);
            Card rail1 = PlayCard("PlummetingMonorail", 1);

            QuickHPStorage(akash.CharacterCard, phlange, bramb, rocks, rail1);
            PlayCard("InfrasonicCollapse");
            //Destroy 1 ongoing or environment card.
            AssertInTrash(rail0);
            //If you destroyed an environment card this way, {Cricket} deals each non-hero target 1 sonic damage.
            QuickHPCheck(-1, -1, -1, -1, -1);
        }

        [Test()]
        public void TestReturnPulse1Target()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickHandStorage(cricket);
            QuickHPStorage(akash.CharacterCard);
            PlayCard("ReturnPulse");
            //{Cricket} deals up to 3 non-hero targets 1 sonic damage each.
            QuickHPCheck(-1);
            //For each target dealt damage this way, draw a card.
            QuickHandCheck(1);
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
        public void Test()
        {

        }
    }
}

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
    public class CricketVariantTests : BaseTest
    {
        protected HeroTurnTakerController cricket { get { return FindHero("Cricket"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(cricket.CharacterCard, 1);
            DealDamage(villain, cricket, 2, DamageType.Melee);
        }

        [Test()]
        public void TestLoadFirstResponseCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(FirstResponseCricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(28, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestFRCricketInnatePower()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //{Cricket} deals 2 targets 1 sonic damage each. You may use a power now.
            //Second power will be Subharmonic Reciever
            PlayCard("SubharmonicReceiver");
            QuickHandStorage(cricket, legacy, bunker, scholar);
            QuickHPStorage(choke, cricket);
            UsePower(cricket);
            QuickHPCheck(-1, -1);
            //Each player may draw a card. When a player draws a card this way, 1 other player must discard a card.
            //All draw 1, Cricket -3 for other heroes, Legacy -1 for Cricket
            QuickHandCheck(-2, 0, 1, 1);
        }

        [Test()]
        public void TestFRCricketIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            Card ring = PutInHand("TheLegacyRing");
            DecisionSelectCard = ring;

            //One player may play a card now.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(ring);
        }

        [Test()]
        public void TestFRCricketIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            PutInTrash("PlummetingMonorail");
            PutInTrash("HostageSituation");
            PutInTrash("RooftopCombat");

            //Shuffle the environment trash into the environment deck.
            UseIncapacitatedAbility(cricket, 1);
            AssertNumberOfCardsInTrash(env, 0);
            AssertNumberOfCardsInDeck(env, 15);
        }

        [Test()]
        public void TestFRCricketIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            SetHitPoints(apostate, 17);
            SetHitPoints(legacy, 17);
            //1 hero target regains 2 HP.
            QuickHPStorage(apostate, legacy);
            UseIncapacitatedAbility(cricket, 2);
            QuickHPCheck(0, 2);
        }

        [Test()]
        public void TestLoadRenegadeCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(RenegadeCricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(26, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestRenegadeCricketInnatePower()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card staff = PutOnDeck("TelescopingStaff");
            Card ring = PutOnDeck("TheLegacyRing");
            //Reveal the top card of a hero deck. You may discard a card to put it into play, otherwise put it into that player's hand.
            QuickHandStorage(cricket);
            UsePower(cricket);
            QuickHandCheck(-1);
            AssertIsInPlay(staff);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            QuickHandStorage(cricket);
            UsePower(cricket);
            QuickHandCheck(-1);
            AssertIsInPlay(ring);
        }

        [Test()]
        public void TestRenegadeCricketIncap1()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(akash.TurnTaker.Deck), new LocationChoice(legacy.TurnTaker.Deck), new LocationChoice(env.TurnTaker.Deck) };

            Card phlange = PutOnDeck("ArborealPhalanges");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(phlange);

            Card ring = PutOnDeck("TheLegacyRing");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(ring);

            Card defender = PutOnDeck("SeismicDefender");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(defender);
        }

        [Test()]
        public void TestRenegadeCricketIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //Shuffle the environment trash into the environment deck.
            UseIncapacitatedAbility(cricket, 1);
            AssertNumberOfCardsInTrash(apostate, 2);
        }

        [Test()]
        public void TestRenegadeCricketIncap3()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            Card phlange = PutInTrash("ArborealPhalanges");
            Card ring = PutInTrash("TheLegacyRing");
            Card defender = PutInTrash("SeismicDefender");

            //Shuffle 1 card from a trash back into its deck.
            UseIncapacitatedAbility(cricket, 2);
            UseIncapacitatedAbility(cricket, 2);
            UseIncapacitatedAbility(cricket, 2);
            AssertInDeck(new Card[] { phlange, ring, defender });
        }

        [Test()]
        public void TestLoadWastelandRoninCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(WastelandRoninCricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(28, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestWastelandRoninCricketInnatePower()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Increase damage dealt by {Cricket} during your next turn by 1. {Cricket} may deal 1 target 1 sonic damage.

            //{Cricket} may deal 1 target 1 sonic damage.
            QuickHPStorage(choke);
            UsePower(cricket);
            QuickHPCheck(-1);

            //{Cricket} may deal 1 target 1 sonic damage.
            //Increase damage dealt by {Cricket} during your next turn by 1.
            GoToStartOfTurn(cricket);
            QuickHPStorage(choke);
            UsePower(cricket);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap1()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(akash.TurnTaker.Deck), new LocationChoice(legacy.TurnTaker.Deck), new LocationChoice(env.TurnTaker.Deck) };

            Card phlange = PutOnDeck("ArborealPhalanges");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(phlange);

            Card ring = PutOnDeck("TheLegacyRing");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(ring);

            Card defender = PutOnDeck("SeismicDefender");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(defender);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //Shuffle the environment trash into the environment deck.
            UseIncapacitatedAbility(cricket, 1);
            AssertNumberOfCardsInTrash(apostate, 2);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap3()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            Card phlange = PutInTrash("ArborealPhalanges");
            Card ring = PutInTrash("TheLegacyRing");
            Card defender = PutInTrash("SeismicDefender");

            //Shuffle 1 card from a trash back into its deck.
            UseIncapacitatedAbility(cricket, 2);
            UseIncapacitatedAbility(cricket, 2);
            UseIncapacitatedAbility(cricket, 2);
            AssertInDeck(new Card[] { phlange, ring, defender });
        }
    }
}

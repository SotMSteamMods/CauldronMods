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
    }
}

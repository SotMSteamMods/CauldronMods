using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheKnight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheKnightVariantTests : BaseTest
    {
        #region HelperFunctions
        protected string HeroNamespace = "Cauldron.TheKnight";

        protected HeroTurnTakerController knight { get { return FindHero("TheKnight"); } }

        protected Card youngKnight { get { return GetCard("TheYoungKnightCharacter"); } }
        protected Card oldKnight { get { return GetCard("TheOldKnightCharacter"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(knight, 1);
            DealDamage(villain, knight, 2, DamageType.Melee);
        }
        
        #endregion

        [Test]
        public void TestFairKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/FairTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(FairTheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(25, knight.CharacterCard.HitPoints);

            StartGame();
            Assert.AreEqual(knight.TurnTaker.InTheBox, youngKnight.Location);
            Assert.AreEqual(knight.TurnTaker.InTheBox, oldKnight.Location);
        }
        [Test]
        public void TestBerzerkerKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/BerzerkerTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(BerzerkerTheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(27, knight.CharacterCard.HitPoints);

            StartGame();
            Assert.AreEqual(knight.TurnTaker.InTheBox, youngKnight.Location);
            Assert.AreEqual(knight.TurnTaker.InTheBox, oldKnight.Location);
        }
        [Test]
        public void TestPastKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/PastTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(PastTheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(27, knight.CharacterCard.HitPoints);

            StartGame();
            Assert.AreEqual(knight.TurnTaker.InTheBox, youngKnight.Location);
            Assert.AreEqual(knight.TurnTaker.InTheBox, oldKnight.Location);
        }
        [Test]
        public void TestRoninKnightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Card knightInstructions = knight.TurnTaker.FindCard("TheKnightCharacter", realCardsOnly: false);
            Assert.IsNotNull(knightInstructions);
            Assert.IsFalse(knightInstructions.IsRealCard);
            Assert.IsInstanceOf(typeof(WastelandRoninTheKnightCharacterCardController), FindCardController(knightInstructions));

            StartGame();
            AssertIsInPlay(youngKnight);
            AssertIsInPlay(oldKnight);
            AssertNumberOfCardsInPlay(knight, 2);
            Assert.AreEqual(15, youngKnight.HitPoints);
            Assert.AreEqual(18, oldKnight.HitPoints);


        }
    }
}

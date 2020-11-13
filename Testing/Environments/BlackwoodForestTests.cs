using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.BlackwoodForest;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class BlackwoodForestTests : BaseTest
    {
        protected TurnTakerController BlackForest => FindEnvironment();


        [Test]
        public void TestBlackForestLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest");

            // Assert
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }


        [Test]
        public void TestOldBonesEmptyTrashPiles()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest");

            StartGame();

            // Act
            PutIntoPlay(OldBonesCardController.Identifier);
            Card oldBones = GetCardInPlay(OldBonesCardController.Identifier);

            GoToStartOfTurn(BlackForest);

            // Assert
            AssertNotInPlay(oldBones);
            Assert.AreEqual(0, ra.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, legacy.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, haka.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, baron.TurnTaker.Trash.Cards.Count());
        }

        [Test]
        public void TestOldBonesSeededTrashPiles()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest");

            StartGame();

            // Seed trash decks
            PutInTrash(ra, GetBottomCardOfDeck(ra));
            PutInTrash(ra, GetBottomCardOfDeck(ra));
            PutInTrash(legacy, GetBottomCardOfDeck(legacy));
            PutInTrash(legacy, GetBottomCardOfDeck(legacy));
            PutInTrash(legacy, GetBottomCardOfDeck(legacy));
            PutInTrash(haka, GetBottomCardOfDeck(haka));
            PutInTrash(baron, GetBottomCardOfDeck(baron));
            PutInTrash(baron, GetBottomCardOfDeck(baron));
            PutInTrash(baron, GetBottomCardOfDeck(baron));
            PutInTrash(baron, GetBottomCardOfDeck(baron));

            // Act
            PutIntoPlay(OldBonesCardController.Identifier);
            Card oldBones = GetCardInPlay(OldBonesCardController.Identifier);

            GoToStartOfTurn(BlackForest);

            // Assert
            AssertNotInPlay(oldBones);
            Assert.AreEqual(1, ra.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(2, legacy.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, haka.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(3, baron.TurnTaker.Trash.Cards.Count());
        }

        [Test]
        public void TestDontStrayFromThePathHoundRevealed()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest");

            StartGame();
            Card theHound = GetCard(TheHoundCardController.Identifier);
            PutOnDeck(BlackForest, theHound); // Top deck The Hound

            // Act
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);
            Card dontStray = GetCardInPlay(DontStrayFromThePathCardController.Identifier);

            PutIntoPlay(ShadowWeaverCardController.Identifier);

            GoToStartOfTurn(BlackForest);


            // Assert
            AssertIsInPlay(theHound);

        }

        [Test]
        public void TestDontStrayFromThePathHoundNotRevealed()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest");

            StartGame();
            Card theHound = GetCard(TheHoundCardController.Identifier);
            PutOnDeck(BlackForest, GetCard(OvergrownCathedralCardController.Identifier)); // Top deck something other than The Hound

            // Act
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);
            Card dontStray = GetCardInPlay(DontStrayFromThePathCardController.Identifier);

            PutIntoPlay(ShadowWeaverCardController.Identifier);

            GoToStartOfTurn(BlackForest);

            // Assert
            AssertNotInPlay(theHound);
        }

        [Test]
        public void TestDenseBrambles()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest");

            StartGame();

            string statusEffectMessageRa = $"{ra.Name} is immune to damage.";

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            QuickHPStorage(ra);

            //AssertStatusEffectsContains("ImmuneToDamage");
            //AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            //AssertStatusEffectsContains(messageText);



            DealDamage(baron, ra, 3, DamageType.Toxic);
            AssertStatusEffectsContains(statusEffectMessageRa);

            GoToStartOfTurn(BlackForest);
            AssertStatusEffects(statusEffectMessageRa);
            // Assert
            QuickHPCheck(0);


        }

    }
}

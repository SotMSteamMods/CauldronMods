using System.Collections.Generic;
using System.Linq;

using Cauldron.TangoOne;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class TangoOnePromoTests : CauldronBaseTest
    {

        private const string DeckNamespaceGhostOps = "Cauldron.TangoOne/GhostOpsTangoOneCharacter";
        private const string DeckNamespace1929 = "Cauldron.TangoOne/PastTangoOneCharacter";
        private const string DeckNamespaceCreed = "Cauldron.TangoOne/CreedOfTheSniperTangoOneCharacter";

        [Test]
        public void TestTangoOneGhostOpsLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(tango);
            Assert.IsInstanceOf(typeof(GhostOpsTangoOneCharacterCardController), tango.CharacterCardController);

            Assert.AreEqual(28, tango.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePowerGhostOps()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                FarsightCardController.Identifier, CriticalHitCardController.Identifier,
                GhostReactorCardController.Identifier, OneShotOneKillCardController.Identifier
            });

            StartGame();
            QuickHandStorage(tango);

            Card ghostReactor = GetCardFromHand(GhostReactorCardController.Identifier);
            Card farsight = GetCardFromHand(FarsightCardController.Identifier);
            DecisionSelectCards = new[]
            {
                farsight,
                ghostReactor
            };

            // Act
            GoToStartOfTurn(tango);
            UsePower(tango);

            // Assert
            AssertOnTopOfDeck(farsight);
            QuickHandCheck(0);
        }

        [Test]
        public void TestGhostOpsIncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;
            PutInHand("FlameBarrier");
            // Act
            AssertIncapLetsHeroPlayCard(tango, 0, ra, "FlameBarrier");

        }

        [Test]
        public void TestGhostOpsIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            Assert.NotNull(this.GameController.StatusEffectControllers
                .FirstOrDefault(s => s.StatusEffect.ToString().ToLower().Equals("damage dealt is irreducible.")
                                     && s.StatusEffect.CardSource.Equals(tango.CharacterCard)));

            GoToStartOfTurn(tango);

            // Effect expired
            Assert.Null(this.GameController.StatusEffectControllers
                .FirstOrDefault(s => s.StatusEffect.ToString().ToLower().Equals("damage dealt is irreducible.")
                                     && s.StatusEffect.CardSource.Equals(tango.CharacterCard)));

            AssertIncapacitated(tango);
        }

        [Test]
        public void TestGhostOpsIncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectTarget = mdp;

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 2);

            GoToUsePowerPhase(ra);
            UsePower(ra);

            QuickHPCheck(-4); // Ra's Pyre 2 dmg + 2 from Tango's Incap #3

            QuickHPStorage(mdp);
            UsePower(ra);
            QuickHPCheck(-2); // Ra's Pyre 2 dmg, Incap expired
            AssertIncapacitated(tango);
        }

        [Test]
        public void TestTangoOne1929Loads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace1929, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(tango);
            Assert.IsInstanceOf(typeof(PastTangoOneCharacterCardController), tango.CharacterCardController);

            Assert.AreEqual(24, tango.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePower1929()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            StartGame();

            QuickHPStorage(legacy);
            DecisionSelectTarget = legacy.CharacterCard;

            GoToStartOfTurn(tango);

            GoToUsePowerPhase(tango);
            UsePower(tango);

            GoToDrawCardPhase(tango);

            GoToEndOfTurn(tango);
            QuickHPCheck(0); // Damage won't trigger until start of Tango's next turn

            //save and load a few times to make sure the status effect sticks around
            SaveAndLoad();
            GoToNextTurn();
            SaveAndLoad();
            GoToNextTurn();
            SaveAndLoad();

            //regrab the hp value of legaacy
            QuickHPStorage(legacy);
            GoToEndOfTurn(tango);  // Go back around to Tango's turn
            QuickHPCheck(-3); // Damage should now have been attempted
        }

        [Test]
        public void TestInnatePower1929_TargetDies()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            GoToStartOfTurn(tango);

            GoToUsePowerPhase(tango);
            UsePower(tango);

            GoToDrawCardPhase(tango);

            GoToEndOfTurn(tango);
            QuickHPCheck(0); // Damage won't trigger until start of Tango's next turn

            DestroyCard(mdp);

            AssertNumberOfStatusEffectsInPlay(0); //status effect should have expired.
            GoToEndOfTurn(tango);  // Go back around to Tango's turn
            QuickHPCheck(0); // Damage should not have been attempted
        }

        [Test]
        public void TestInnatePower1929_TangoOneDies()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");
            StartGame();

            var mdp = GetMobileDefensePlatform().Card;

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            GoToStartOfTurn(tango);

            GoToUsePowerPhase(tango);
            UsePower(tango);

            GoToDrawCardPhase(tango);

            GoToEndOfTurn(tango);
            QuickHPCheck(0); // Damage won't trigger until start of Tango's next turn

            DealDamage(baron, tango, 99, DamageType.Cold); //incap tango one

            //status effect doesn't expire till after start of turn
            GoToEndOfTurn(tango);  // Go back around to Tango's turn
            AssertNumberOfStatusEffectsInPlay(0); //status effect should have expired.
            QuickHPCheck(0); // Damage should not have been attempted
        }
        [Test]
        public void TestInnatePower1929_PowerModifiersTrack()
        {
            SetupGameController("BaronBlade", DeckNamespace1929, "Unity", "Legacy", "Megalopolis");
            StartGame();

            QuickHPStorage(legacy);
            DecisionSelectTarget = legacy.CharacterCard;
            PlayCard("HastyAugmentation");
            GoToStartOfTurn(tango);
            QuickHPCheck(-5);
        }
        [Test]
        public void Test1929IncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionYesNo = true;
            QuickHandStorage(legacy);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 0);

            GoToEndOfTurn(tango);
            QuickHandCheck(0); // Card draw option won't trigger until start of Tango's next turn

            GoToEndOfTurn(tango); // Go back around to Tango's turn
            QuickHandCheck(2);

            // Assert
            AssertIncapacitated(tango);
        }

        [Test]
        public void Test1929IncapacitateOption1DeclineDraw()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DecisionSelectCard = legacy.CharacterCard;
            DecisionYesNo = false;
            QuickHandStorage(legacy);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 0);

            GoToEndOfTurn(tango);
            QuickHandCheck(0); // Card draw option won't trigger until start of Tango's next turn

            GoToEndOfTurn(tango); // Go back around to Tango's turn
            QuickHandCheck(0);

            // Assert
            AssertIncapacitated(tango);
        }

        [Test]
        public void Test1929IncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(legacy, new List<string>()
            {
                "Fortitude", "DangerSense", "NextEvolution", "SurgeOfStrength"
            });


            StartGame();

            QuickHandStorage(legacy);

            //DecisionSelectCard = legacy.CharacterCard;
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCards = new[] { GetCardFromHand(legacy, 0), GetCardFromHand(legacy, 1) };
            DecisionYesNo = true;

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            GoToEndOfTurn(tango);
            QuickHandCheck(0);

            GoToEndOfTurn(tango); // Go back around to Tango's turn
            QuickHandCheck(-2);
            AssertNumberOfCardsInPlay(legacy, 3); // Character card + 2 randomly played cards

            // Assert
            AssertIncapacitated(tango);
        }

        [Test]
        public void Test1929IncapacitateOption2DeclinePlay()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            StartGame();

            QuickHandStorage(legacy);

            DecisionSelectCard = legacy.CharacterCard;
            DecisionYesNo = false;

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            GoToEndOfTurn(tango);
            QuickHandCheck(0);

            GoToEndOfTurn(tango); // Go back around to Tango's turn
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(legacy, 1); // Character card

            // Assert
            AssertIncapacitated(tango);
        }

        [Test]
        public void Test1929IncapacitateOption2NoCardsToPlay()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            StartGame();
            DiscardAllCards(legacy);

            QuickHandStorage(legacy);

            DecisionSelectTurnTaker = legacy.TurnTaker;
            //DecisionSelectCards = new[] { legacy.CharacterCard, GetCardFromHand(legacy, 0), GetCardFromHand(legacy, 1) };
            DecisionYesNo = true;

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            GoToEndOfTurn(tango);
            QuickHandCheck(0);

            GoToEndOfTurn(tango); // Go back around to Tango's turn
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(legacy, 1); // Character card

            // Assert
            AssertIncapacitated(tango);
        }

        [Test]
        public void Test1929IncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "InsulaPrimalis");

            StartGame();

            PutIntoPlay("EnragedTRex");
            Card enragedTRex = GetCardInPlay("EnragedTRex");
            DecisionSelectCard = enragedTRex;

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 2);

            // Assert
            AssertIncapacitated(tango);
            AssertInTrash(enragedTRex);
        }


        [Test]
        public void TestCreedTangoOneLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(tango);
            Assert.IsInstanceOf(typeof(CreedOfTheSniperTangoOneCharacterCardController), tango.CharacterCardController);

            Assert.AreEqual(25, tango.CharacterCard.HitPoints);
        }

        [Test]
        public void TestCreedTangoOneInnatePower_Draw()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            var card = PutOnDeck("SniperRifle");

            QuickHandStorage(tango);

            GoToStartOfTurn(tango);
            UsePower(tango);
            AssertInHand(card);
            QuickHandCheck(1);
        }

        [Test]
        public void TestCreedTangoOneInnatePower_DrawCritical()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            var card = PutOnDeck("GhostReactor");
            var play = PutInHand(ra, "ImbuedFire");

            QuickHandStorage(tango);
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCard = play;
            UsePower(tango);
            AssertInTrash(card);
            AssertIsInPlay(play);
            QuickHandCheck(0);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption1()
        {
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, tango, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            AssertIncapLetsHeroDrawCard(tango, 0, ra, 1);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption2()
        {
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, tango, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);
            AssertIncapLetsHeroUsePower(tango, 1, ra);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption3_NoMatch()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, tango, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);

            var c1 = PutOnDeck("BackFistStrike");
            var c2 = PutOnDeck("DangerSense");
                        
            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionSelectCards = new[] { c1, c2 };
            UseIncapacitatedAbility(tango, 2);
            AssertInTrash(c1);
            AssertInTrash(c2);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption3_Match()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, tango, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(tango);

            var c1 = PutOnDeck("NextEvolution");
            var c2 = PutOnDeck("DangerSense");

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionSelectCards = new[] { c1, c2 };
            UseIncapacitatedAbility(tango, 2);
            AssertIsInPlay(c1);
            AssertIsInPlay(c2);
        }
    }
}

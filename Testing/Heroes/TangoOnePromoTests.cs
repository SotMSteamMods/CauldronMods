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
    public class TangoOnePromoTests : BaseTest
    {
        protected HeroTurnTakerController TangoOne => FindHero("TangoOne");

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
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(GhostOpsTangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(28, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePowerGhostOps()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                FarsightCardController.Identifier, CriticalHitCardController.Identifier,
                GhostReactorCardController.Identifier, OneShotOneKillCardController.Identifier
            });

            StartGame();
            QuickHandStorage(TangoOne);

            Card ghostReactor = GetCardFromHand(GhostReactorCardController.Identifier);
            Card farsight = GetCardFromHand(FarsightCardController.Identifier);
            DecisionSelectCards = new[]
            {
                farsight,
                ghostReactor
            };

            // Act
            GoToStartOfTurn(TangoOne);
            UsePower(TangoOne);

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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 0);


            // Assert
            AssertIncapacitated(TangoOne);
            QuickHandCheck(1);

        }

        [Test]
        public void TestGhostOpsIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);

            Assert.NotNull(this.GameController.StatusEffectControllers
                .FirstOrDefault(s => s.StatusEffect.ToString().Equals("damage dealt is irreducible.")
                                     && s.StatusEffect.CardSource.Equals(TangoOne.CharacterCard)));

            GoToStartOfTurn(TangoOne);

            // Effect expired
            Assert.Null(this.GameController.StatusEffectControllers
                .FirstOrDefault(s => s.StatusEffect.ToString().Equals("damage dealt is irreducible.")
                                     && s.StatusEffect.CardSource.Equals(TangoOne.CharacterCard)));

            AssertIncapacitated(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = mdp;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 2);

            GoToUsePowerPhase(ra);
            UsePower(ra);

            QuickHPCheck(-4); // Ra's Pyre 2 dmg + 2 from Tango's Incap #3

            QuickHPStorage(mdp);
            UsePower(ra);
            QuickHPCheck(-2); // Ra's Pyre 2 dmg, Incap expired
            AssertIncapacitated(TangoOne);
        }

        [Test]
        public void TestTangoOne1929Loads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace1929, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(PastTangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(24, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePower1929()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            StartGame();

            QuickHPStorage(legacy);

            DecisionSelectTarget = legacy.CharacterCard;

            GoToStartOfTurn(TangoOne);

            GoToUsePowerPhase(TangoOne);
            UsePower(TangoOne);

            GoToDrawCardPhase(TangoOne);

            GoToEndOfTurn(TangoOne);
            QuickHPCheck(0); // Damage won't trigger until start of Tango's next turn

            GoToEndOfTurn(TangoOne);  // Go back around to Tango's turn
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

            GoToStartOfTurn(TangoOne);

            GoToUsePowerPhase(TangoOne);
            UsePower(TangoOne);

            GoToDrawCardPhase(TangoOne);

            GoToEndOfTurn(TangoOne);
            QuickHPCheck(0); // Damage won't trigger until start of Tango's next turn

            DestroyCard(mdp);

            AssertNumberOfStatusEffectsInPlay(0); //status effect should have expired.
            GoToEndOfTurn(TangoOne);  // Go back around to Tango's turn
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

            GoToStartOfTurn(TangoOne);

            GoToUsePowerPhase(TangoOne);
            UsePower(TangoOne);

            GoToDrawCardPhase(TangoOne);

            GoToEndOfTurn(TangoOne);
            QuickHPCheck(0); // Damage won't trigger until start of Tango's next turn

            DealDamage(baron, TangoOne, 99, DamageType.Cold); //incap tango one

            //status effect doesn't expire till after start of turn
            GoToEndOfTurn(TangoOne);  // Go back around to Tango's turn
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
            GoToStartOfTurn(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 0);

            GoToEndOfTurn(TangoOne);
            QuickHandCheck(0); // Card draw option won't trigger until start of Tango's next turn

            GoToEndOfTurn(TangoOne); // Go back around to Tango's turn
            QuickHandCheck(2);

            // Assert
            AssertIncapacitated(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 0);

            GoToEndOfTurn(TangoOne);
            QuickHandCheck(0); // Card draw option won't trigger until start of Tango's next turn

            GoToEndOfTurn(TangoOne); // Go back around to Tango's turn
            QuickHandCheck(0);

            // Assert
            AssertIncapacitated(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);

            GoToEndOfTurn(TangoOne);
            QuickHandCheck(0);

            GoToEndOfTurn(TangoOne); // Go back around to Tango's turn
            QuickHandCheck(-2);
            AssertNumberOfCardsInPlay(legacy, 3); // Character card + 2 randomly played cards

            // Assert
            AssertIncapacitated(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);

            GoToEndOfTurn(TangoOne);
            QuickHandCheck(0);

            GoToEndOfTurn(TangoOne); // Go back around to Tango's turn
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(legacy, 1); // Character card

            // Assert
            AssertIncapacitated(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);

            GoToEndOfTurn(TangoOne);
            QuickHandCheck(0);

            GoToEndOfTurn(TangoOne); // Go back around to Tango's turn
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(legacy, 1); // Character card

            // Assert
            AssertIncapacitated(TangoOne);
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

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 2);

            // Assert
            AssertIncapacitated(TangoOne);
            AssertInTrash(enragedTRex);
        }


        [Test]
        public void TestCreedTangoOneLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(CreedOfTheSniperTangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(25, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestCreedTangoOneInnatePower_Draw()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            var card = PutOnDeck("SniperRifle");

            QuickHandStorage(TangoOne);

            GoToStartOfTurn(TangoOne);
            UsePower(TangoOne);
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

            QuickHandStorage(TangoOne);
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCard = play;
            UsePower(TangoOne);
            AssertInTrash(card);
            AssertIsInPlay(play);
            QuickHandCheck(0);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption1()
        {
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, TangoOne, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            AssertIncapLetsHeroDrawCard(TangoOne, 0, ra, 1);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption2()
        {
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, TangoOne, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);
            AssertIncapLetsHeroUsePower(TangoOne, 1, ra);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption3_NoMatch()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, TangoOne, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);

            var c1 = PutOnDeck("BackFistStrike");
            var c2 = PutOnDeck("DangerSense");
                        
            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionSelectCards = new[] { c1, c2 };
            UseIncapacitatedAbility(TangoOne, 2);
            AssertInTrash(c1);
            AssertInTrash(c2);
        }

        [Test]
        public void TestCreedTangoOneIncapacitateOption3_Match()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceCreed, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, TangoOne, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(TangoOne);

            var c1 = PutOnDeck("NextEvolution");
            var c2 = PutOnDeck("DangerSense");

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionSelectCards = new[] { c1, c2 };
            UseIncapacitatedAbility(TangoOne, 2);
            AssertIsInPlay(c1);
            AssertIsInPlay(c2);
        }
    }
}

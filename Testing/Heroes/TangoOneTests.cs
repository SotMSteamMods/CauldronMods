using System;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using Cauldron.TangoOne;

using NUnit.Framework;


namespace CauldronTests
{
    [TestFixture()]
    public class TangoOneTests : BaseTest
    {
        protected HeroTurnTakerController TangoOne => FindHero("TangoOne");
        protected TurnTakerController Anathema => FindVillain("Anathema");

        [Test]
        public void TestTangoOneLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(TangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(24, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePower()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(TangoOne);
            UsePower(TangoOne);

            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestIncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            StartGame();

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHandStorage(ra);

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 0);


            // Assert
            AssertIncapacitated(TangoOne);
            QuickHandCheck(1);
        }

        [Test]
        public void TestIncapacitateOption2ToDeck()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            StartGame();

            Card backlashField = GetCard("BacklashField");
            PutOnDeck(baron, backlashField);

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Deck, false);

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);

            // Assert
            AssertIncapacitated(TangoOne);
            AssertOnTopOfDeck(baron, backlashField);
        }

        [Test]
        public void TestIncapacitateOption2ToTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            StartGame();

            Card backlashField = GetCard("BacklashField");
            PutOnDeck(baron, backlashField);

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Trash, false);

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);
            
            // Assert
            AssertIncapacitated(TangoOne);
            AssertOnTopOfTrash(baron, backlashField);
        }

        [Test]
        public void TestIncapacitateOption3MakeChoices()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Legacy", "Megalopolis");

            StartGame();

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            PutInHand("DangerSense");
            Card dangerSense = GetCardFromHand("DangerSense");
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            DecisionSelectCards = new Card[] { dangerSense, nextEvolution };
            DecisionYesNo = true;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 2);

            // Assert
            AssertIncapacitated(TangoOne);
            AssertIsInPlay(dangerSense, nextEvolution);
        }

        [Test]
        public void TestIncapacitateOption3DontMakeChoices()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Legacy", "Megalopolis");

            StartGame();

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            PutInHand("DangerSense");
            Card dangerSense = GetCardFromHand("DangerSense");
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            DecisionYesNo = false;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 2);

            // Assert
            AssertIncapacitated(TangoOne);
            AssertNotInPlay(dangerSense, nextEvolution);
        }

        [Test]
        public void TestChameleonArmorDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();
            PutOnDeck(TangoOne, GetCard(DamnGoodGroundCardController.Identifier));

            DecisionYesNo = true;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(0);
        }

        [Test]
        public void TestChameleonArmorDiscardCriticalCardFail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();
            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));

            DecisionYesNo = true;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(-5);
        }

        [Test]
        public void TestChameleonArmorDontDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            DecisionYesNo = false;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(-5);
        }

        [Test]
        public void TestCriticalHitDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PutOnDeck(TangoOne, GetCard(DamnGoodGroundCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);


            // Assert
            QuickHPCheck(-4);
        }

        [Test]
        public void TestCriticalHitDiscardCriticalCardFail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);


            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestCriticalHitDontDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = false;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);


            // Assert
            QuickHPCheck(-1);
        }

        
        [Test]
        public void TestCriticalHitWithNemesisDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("Cauldron.Anathema", "Cauldron.TangoOne", "Ra", "Megalopolis");

            // Supress Anathema's ability to play cards to create ideal testing env.
            CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
            cannotPlayCardsStatusEffect.CardCriteria.IsVillain = true;
            cannotPlayCardsStatusEffect.UntilTargetLeavesPlay(Anathema.CharacterCard);
            RunCoroutine(this.GameController.AddStatusEffect(cannotPlayCardsStatusEffect, true, new CardSource(Anathema.CharacterCardController)));

            StartGame();

            // Remove body part cards to create ideal testing env.
            RemoveVillainCards();

            
            DecisionSelectCard = ra.CharacterCard;

            PutOnDeck(TangoOne, GetCard(DamnGoodGroundCardController.Identifier));

            QuickHPStorage(Anathema);

            DecisionYesNo = true;
            DecisionSelectTarget = Anathema.CharacterCard;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);

            // Assert
            QuickHPCheck(-5); // Snipe +1, Nemesis +1, Critical Hit + 3 = 5
        }
        
        [Test]
        public void TestDamnGoodGround()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectTargets = new[] {baron.CharacterCard, mdp, null};
            QuickHPStorage(baron.CharacterCard, mdp, TangoOne.CharacterCard);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(DamnGoodGroundCardController.Identifier);
            PlayCard(DamnGoodGroundCardController.Identifier);

            // Assert
            QuickHPCheck(0, -1, 2);
        }

        [Test]
        public void TestDisablingShot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PlayCard(baron, "BacklashField");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = FindCardInPlay("BacklashField");
            DecisionSelectTarget = mdp;
            //DecisionSelectTargets = new[] {mdp};
            QuickHPStorage(mdp);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(DisablingShotCardController.Identifier);
            PlayCard(DisablingShotCardController.Identifier);

            // Assert
            AssertNotInPlay(new []{ "BacklashField"});
            QuickHPCheck(-2);
        }

        [Test]
        public void TestFarsight()
        {
            // Arrange
            SetupGameController("GloomWeaver", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PlayCard("StrengthOfTheGrave"); // Reduce damage to zombies by 1
            
            Card zombie = FindCardInPlay("ZombieServant");
            DecisionSelectTarget = zombie;
            QuickHPStorage(zombie);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(FarsightCardController.Identifier);
            PlayCard(FarsightCardController.Identifier);

            UsePower(TangoOne);

            // Assert
            QuickHPCheck(-1); // Farsight allows Innate Power to deal 1 damage
        }

        [Test]
        public void TestGhostReactor()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                GhostReactorCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            QuickHPStorage(TangoOne.CharacterCard, mdp);
            QuickHandStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, GhostReactorCardController.Identifier);

            UsePower(GhostReactorCardController.Identifier);

            DealDamage(baron, TangoOne, 10, DamageType.Psychic);
            DealDamage(TangoOne, mdp, 1, DamageType.Projectile); // With +2 increase from Ghost Reactor
            DealDamage(TangoOne, mdp, 1, DamageType.Projectile); // +1, Ghost Reactor effect expired

            // Assert
            QuickHPCheck(0, -4); // Tango One (0) due to Ghost Reactor immunity, MDP -4 
            QuickHandCheck(0); // Discard Ghost Reactor (-1), Draw a card (+1)
        }

        [Test]
        public void TestInfiltrate()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                InfiltrateCardController.Identifier, GhostReactorCardController.Identifier
            });

            StartGame();

            PutOnDeck(TangoOne, GetCard(CriticalHitCardController.Identifier));
            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));


            DecisionSelectLocation = new LocationChoice(TangoOne.HeroTurnTaker.Deck);
            DecisionSelectCard = GetCard(CriticalHitCardController.Identifier); // First drawn card to put back on deck
            DecisionSelectCardToPlay = GetCard(FarsightCardController.Identifier);

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, InfiltrateCardController.Identifier);

            // Assert
            AssertIsInPlay(GetCardInPlay(FarsightCardController.Identifier));
        }

        [Test]
        public void TestLineEmUp()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                LineEmUpCardController.Identifier
            });

            StartGame();

            Card bb = PlayCard(baron, "BladeBattalion");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            SetHitPoints(bb, 1);
            QuickHPStorage(mdp);
            DecisionSelectTargets = new[] {bb, mdp};


            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, LineEmUpCardController.Identifier);
            UsePower(TangoOne);

            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestOneShotShotOneKillSuccessfulDestruction()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                OneShotOneKillCardController.Identifier,
                GhostReactorCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 4);
            QuickHandStorage(TangoOne);

            DecisionSelectCards = new[]
            {
                GetCardFromHand(TangoOne, GhostReactorCardController.Identifier),
                GetCardFromHand(TangoOne, FarsightCardController.Identifier),
                null,
                mdp
            };

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, OneShotOneKillCardController.Identifier);

            // Assert
            AssertNotInPlay(mdp); // 2 cards were discarded: 2 cards * 2 = 4 which MDP's HP qualifies for destruction
            QuickHandCheck(-2); // (4 -1 for playing One Shot, -2 for the 2 discards, +1 for successfully destroying a target)
        }

        [Test]
        public void TestOneShotShotOneKillMultipleEligibleTargetsForDestruction()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                OneShotOneKillCardController.Identifier,
                GhostReactorCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();

            Card bb = GetCard("BladeBattalion");
            PlayCard(bb);
            SetHitPoints(bb, 3);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 4);
            
            QuickHandStorage(TangoOne);

            DecisionSelectCards = new[]
            {
                GetCardFromHand(TangoOne, GhostReactorCardController.Identifier),
                GetCardFromHand(TangoOne, FarsightCardController.Identifier),
                null,
                mdp
            };

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, OneShotOneKillCardController.Identifier);

            // Assert
            AssertNotInPlay(mdp); // 2 cards were discarded: 2 cards * 2 = 4 which MDP's HP qualifies for destruction
            AssertIsInPlay(bb); // Blade Battalion was not chosen as the destruction target
            QuickHandCheck(-2); // (4 -1 for playing One Shot, -2 for the 2 discards, +1 for successfully destroying a target)
        }

        [Test]
        public void TestOneShotShotOneKillUnsuccessfulDestruction()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                OneShotOneKillCardController.Identifier,
                GhostReactorCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 6);
            QuickHandStorage(TangoOne);

            DecisionSelectCards = new[]
            {
                GetCardFromHand(TangoOne, GhostReactorCardController.Identifier),
                GetCardFromHand(TangoOne, FarsightCardController.Identifier),
                null,
                mdp
            };

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, OneShotOneKillCardController.Identifier);

            // Assert
            AssertIsInPlay(mdp); // 2 cards were discarded: 2 cards * 2 = 4 which MDP's HP (6) *DOES NOT* qualify for destruction
            QuickHandCheck(-3); // (4 -1 for playing One Shot, -2 for the 2 discards)
        }

        [Test]
        public void TestOpportunistShuffleTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                OpportunistCardController.Identifier
            });

            // Fill trash with a few cards so we can assert trash has changed if we shuffle it into the deck
            PutInTrash(TangoOne, GetCard(ChameleonArmorCardController.Identifier));
            PutInTrash(TangoOne, GetCard(GhostReactorCardController.Identifier));

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            QuickHandStorage(TangoOne);
            QuickShuffleStorage(TangoOne);

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, OpportunistCardController.Identifier);
            UsePower(TangoOne); // Snipe power

            // Assert
            QuickHPCheck(-4); // Snipe (1) + Opportunist (+3)
            QuickHandCheck(1);
            Assert.AreEqual(1, GetNumberOfCardsInTrash(TangoOne)); // One card in trash (Opportunist)
            QuickShuffleCheck(1); // Tango's deck was shuffled by Opportunist card
        }

        [Test]
        public void TestOpportunistDontShuffleTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                OpportunistCardController.Identifier
            });

            // Fill trash with a few cards so we can assert trash has changed after if we shuffle it into the deck
            PutInTrash(TangoOne, GetCard(ChameleonArmorCardController.Identifier));
            PutInTrash(TangoOne, GetCard(GhostReactorCardController.Identifier));

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            QuickHandStorage(TangoOne);
            QuickShuffleStorage(TangoOne);

            DecisionSelectTarget = mdp;
            DecisionYesNo = false;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, OpportunistCardController.Identifier);
            UsePower(TangoOne); // Snipe power

            // Assert
            QuickHPCheck(-4); // Snipe (1) + Opportunist (+3)
            QuickHandCheck(1);
            Assert.AreEqual(3, GetNumberOfCardsInTrash(TangoOne)); // (Opportunist, Ghost Reactor, Chameleon Armor)
            QuickShuffleCheck(0); // Tango's deck was not shuffled by Opportunist card
        }

        [Test]
        public void TestPerfectFocusDrawCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                PerfectFocusCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;
            DecisionSelectCard = GetCardFromHand(DisablingShotCardController.Identifier);
            DecisionsYesNo = new[] {true, false};

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, PerfectFocusCardController.Identifier);
            UsePower(TangoOne); // Snipe power

            // Assert
            QuickHPCheck(-6); // Disabling Shot (2 + Perfect Focus +3), Snipe (1)
        }

        [Test]
        public void TestPerfectFocusDontDrawCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                PerfectFocusCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;
            DecisionYesNo = false;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, PerfectFocusCardController.Identifier);
            UsePower(TangoOne); // Snipe power

            // Assert
            QuickHPCheck(-4); // Disabling Shot (2 + Perfect Focus +3), Snipe (1)
        }

        [Test]
        public void TestPsionicSuppression()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                PsionicSuppressionCardController.Identifier
            });

            StartGame();
            Card bb = PlayCard(baron, "BladeBattalion");
            DecisionSelectTarget = bb;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, PsionicSuppressionCardController.Identifier);
            
            GoToEndOfTurn(baron);
            GoToStartOfTurn(TangoOne);

            // Assert
            QuickHPCheck(-5); // Battalion Blade hit prior to playing Psionic Suppression

        }

        [Test]
        public void TestSniperRiflePower1Success()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                SniperRifleCardController.Identifier,
                GhostReactorCardController.Identifier,
                GhostReactorCardController.Identifier,
                DisablingShotCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new[]
            {
                GetCardFromHand(GhostReactorCardController.Identifier), 
                GetCardFromHand(DisablingShotCardController.Identifier), 
                mdp
            };
            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 0);

            // Assert
            AssertNotInPlay(mdp); // MDP destroyed by Sniper Rifle power #1
            Assert.AreEqual(2, GetNumberOfCardsInTrash(TangoOne)); // (Ghost Reactor, Disabling Shot)
        }

        [Test]
        public void TestSniperRiflePower1Fail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                SniperRifleCardController.Identifier, // Critical keyword
                FarsightCardController.Identifier,
                DisablingShotCardController.Identifier, // Critical keyword
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new[]
            {
                GetCardFromHand(DisablingShotCardController.Identifier),
                mdp
            };
            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 0);

            // Assert
            AssertIsInPlay(mdp); // MDP was not destroyed by Sniper Rifle power #1 due to lack of discarded Critical cards
            Assert.AreEqual(1, GetNumberOfCardsInTrash(TangoOne)); // (Disabling Shot)
        }

        [Test]
        public void TestSniperRiflePower2()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                SniperRifleCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 1);

            // Assert
            QuickHPCheck(-2);
        }

        [Test]
        public void TestWetWork()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                WetWorkCardController.Identifier
            });

            // Fill trash with a few cards so we can assert trash has changed after we shuffle some of it into the deck
            PutInTrash(TangoOne, GetCard(ChameleonArmorCardController.Identifier));
            PutInTrash(TangoOne, GetCard(DisablingShotCardController.Identifier));
            PutInTrash(TangoOne, GetCard(GhostReactorCardController.Identifier));


            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            QuickShuffleStorage(TangoOne, ra, baron, env);
            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;
            DecisionSelectCards = new[]
            {
                GetCard(DisablingShotCardController.Identifier), 
                GetCard(GhostReactorCardController.Identifier),
                mdp
            };

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, WetWorkCardController.Identifier);


            // Assert
            QuickHPCheck(-2); // Wet Work (2)
            Assert.AreEqual(2, GetNumberOfCardsInTrash(TangoOne)); // (ChameleonArmorCard, WetWork)
            QuickShuffleCheck(1, 1, 1, 1);

        }

    }
}

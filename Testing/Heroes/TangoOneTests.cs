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
    public class TangoOneTests : CauldronBaseTest
    {

        private const string DeckNamespace = "Cauldron.TangoOne";

        [Test]
        public void TestTangoOneLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(tango);
            Assert.IsInstanceOf(typeof(TangoOneCharacterCardController), tango.CharacterCardController);

            Assert.AreEqual(24, tango.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePower_Villain()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(tango);
            UsePower(tango);

            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestInnatePower_Hero()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHPStorage(ra);

            DecisionSelectTarget = ra.CharacterCard;

            // Act
            GoToStartOfTurn(tango);
            UsePower(tango);

            // Assert
            QuickHPCheck(-1);
        }



        [Test]
        public void TestIncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card staffOfRa = GetCard("TheStaffOfRa");
            PutOnDeck(ra, staffOfRa);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHandStorage(ra);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 0);


            // Assert
            AssertIncapacitated(tango);
            QuickHandCheck(1);
            AssertInHand(staffOfRa);
        }

        [Test]
        public void TestIncapacitateOption2ToDeck_Villain()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card backlashField = GetCard("BacklashField");
            PutOnDeck(baron, backlashField);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Deck, false);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            // Assert
            AssertIncapacitated(tango);
            AssertOnTopOfDeck(baron, backlashField);
        }

        [Test]
        public void TestIncapacitateOption2ToTrash_Villain()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card backlashField = GetCard("BacklashField");
            PutOnDeck(baron, backlashField);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Trash, false);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            // Assert
            AssertIncapacitated(tango);
            AssertOnTopOfTrash(baron, backlashField);
        }

        [Test]
        public void TestIncapacitateOption2ToDeck_Hero()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card dangerSense = GetCard("DangerSense");
            PutOnDeck(legacy, dangerSense);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Deck);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            // Assert
            AssertIncapacitated(tango);
            AssertOnTopOfDeck(legacy, dangerSense);
        }

        [Test]
        public void TestIncapacitateOption2ToTrash_Hero()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card dangerSense = GetCard("DangerSense");
            PutOnDeck(legacy, dangerSense);

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(legacy.TurnTaker.Trash, false);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 1);

            // Assert
            AssertIncapacitated(tango);
            AssertOnTopOfTrash(legacy, dangerSense);
        }

        [Test]
        public void TestIncapacitateOption3MakeChoices()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            PutInHand("DangerSense");
            Card dangerSense = GetCardFromHand("DangerSense");
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            DecisionSelectCards = new Card[] { dangerSense, nextEvolution };
            DecisionYesNo = true;

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 2);

            // Assert
            AssertIncapacitated(tango);
            AssertIsInPlay(dangerSense, nextEvolution);
        }

        [Test]
        public void TestIncapacitateOption3DontMakeChoices()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            PutInHand("DangerSense");
            Card dangerSense = GetCardFromHand("DangerSense");
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            DecisionYesNo = false;
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            QuickHandStorage(ra, legacy);

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 2);

            // Assert
            AssertIncapacitated(tango);
            AssertNotInPlay(dangerSense, nextEvolution);
            QuickHandCheck(0, 0);
        }

        [Test]
        public void TestIncapacitateOption3MakeOneChoice()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);

            PutInHand("DangerSense");
            Card dangerSense = GetCardFromHand("DangerSense");
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { dangerSense, null };

            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 2);

            // Assert
            AssertIncapacitated(tango);
            AssertNotInPlay(nextEvolution);
            AssertIsInPlay(dangerSense);
        }
        [Test]
        public void TestIncapacitateOption3NotAllowLimited()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            StartGame();

            SetHitPoints(tango.CharacterCard, 1);
            DealDamage(baron, tango, 2, DamageType.Melee);
            PutOnDeck(legacy, legacy.HeroTurnTaker.Hand.Cards);
            Card inPlayFortitude = PlayCard("Fortitude");
            Card handFortitude = PutInHand("Fortitude");
            PutInHand("DangerSense");
            Card dangerSense = GetCardFromHand("DangerSense");
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            AssertNextDecisionChoices(included: new Card[] { dangerSense, nextEvolution }, notIncluded: new Card[] { handFortitude, inPlayFortitude });
            // Act
            GoToUseIncapacitatedAbilityPhase(tango);
            UseIncapacitatedAbility(tango, 2);

            // Assert
            AssertIncapacitated(tango);
            AssertNotInPlay(handFortitude);
        }

        [Test]
        public void TestChameleonArmorDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();
            PutOnDeck(tango, GetCard(DamnGoodGroundCardController.Identifier));

            DecisionYesNo = true;
            QuickHPStorage(tango);

            // Act
            GoToStartOfTurn(tango);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, tango, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(0);
        }

        [Test]
        public void TestChameleonArmorDiscardCriticalCardFail()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();
            PutOnDeck(tango, GetCard(FarsightCardController.Identifier));

            DecisionYesNo = true;
            QuickHPStorage(tango);

            // Act
            GoToStartOfTurn(tango);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, tango, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(-5);
        }

        [Test]
        public void TestChameleonArmorDontDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DecisionYesNo = false;
            QuickHPStorage(tango);

            // Act
            GoToStartOfTurn(tango);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, tango, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(-5);
        }

        [Test]
        public void TestCriticalHitDamageFail()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();
            PlayCard("LivingForceField");
            var topDeck = PutOnDeck(tango, GetCard(DamnGoodGroundCardController.Identifier));

            QuickHPStorage(baron);

            DecisionYesNo = true;
            DecisionSelectTarget = baron.CharacterCard;

            // Act
            GoToPlayCardPhase(tango);
            PlayCard(CriticalHitCardController.Identifier);
            DealDamage(tango.CharacterCard.ResponsibleTarget, baron.CharacterCard, 0, DamageType.Infernal);

            // Assert
            QuickHPCheck(0);

            AssertOnTopOfDeck(tango, topDeck);
        }

        [Test]
        public void TestCriticalHitDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            PutOnDeck(tango, GetCard(DamnGoodGroundCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(tango);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(tango);


            // Assert
            QuickHPCheck(-4);
        }

        [Test]
        public void TestCriticalHitDiscardCriticalCardFail()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            PutOnDeck(tango, GetCard(FarsightCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(tango);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(tango);


            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestCriticalHitDontDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            PutOnDeck(tango, GetCard(FarsightCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = false;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(tango);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(tango);


            // Assert
            QuickHPCheck(-1);
        }


        [Test]
        public void TestCriticalHitWithNemesisDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("Cauldron.Anathema", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            // Supress Anathema's ability to play cards to create ideal testing env.
            CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
            cannotPlayCardsStatusEffect.CardCriteria.IsVillain = true;
            cannotPlayCardsStatusEffect.UntilTargetLeavesPlay(anathema.CharacterCard);
            RunCoroutine(this.GameController.AddStatusEffect(cannotPlayCardsStatusEffect, true, new CardSource(anathema.CharacterCardController)));

            StartGame();

            // Remove body part cards to create ideal testing env.
            RemoveVillainCards();


            DecisionSelectCard = ra.CharacterCard;

            PutOnDeck(tango, GetCard(DamnGoodGroundCardController.Identifier));

            QuickHPStorage(anathema);

            DecisionYesNo = true;
            DecisionSelectTarget = anathema.CharacterCard;


            // Act
            GoToPlayCardPhase(tango);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(tango);

            // Assert
            QuickHPCheck(-5); // Snipe +1, Nemesis +1, Critical Hit + 3 = 5
        }

        [Test]
        public void TestDamnGoodGround()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            DealDamage(baron, tango, 5, DamageType.Cold);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectTargets = new[] { baron.CharacterCard, mdp, null };
            QuickHPStorage(baron.CharacterCard, mdp, tango.CharacterCard);

            // Act
            GoToStartOfTurn(tango);
            PutInHand(DamnGoodGroundCardController.Identifier);
            PlayCard(DamnGoodGroundCardController.Identifier);

            // Assert
            QuickHPCheck(0, -1, 2);
        }

        [Test]
        public void TestDisablingShot()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            PlayCard(baron, "BacklashField");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = FindCardInPlay("BacklashField");
            DecisionSelectTarget = mdp;
            //DecisionSelectTargets = new[] {mdp};
            QuickHPStorage(mdp);

            // Act
            GoToStartOfTurn(tango);
            PutInHand(DisablingShotCardController.Identifier);
            PlayCard(DisablingShotCardController.Identifier);

            // Assert
            AssertNotInPlay(new[] { "BacklashField" });
            QuickHPCheck(-2);
        }

        [Test]
        public void TestFarsight()
        {
            // Arrange
            SetupGameController("GloomWeaver", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            PlayCard("StrengthOfTheGrave"); // Reduce damage to zombies by 1

            Card zombie = FindCardInPlay("ZombieServant");
            DecisionSelectTarget = zombie;
            QuickHPStorage(zombie);

            // Act
            GoToStartOfTurn(tango);
            PutInHand(FarsightCardController.Identifier);
            PlayCard(FarsightCardController.Identifier);

            UsePower(tango);

            // Assert
            QuickHPCheck(-1); // Farsight allows Innate Power to deal 1 damage
        }

        [Test]
        public void TestGhostReactor()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                GhostReactorCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            QuickHPStorage(tango.CharacterCard, mdp);
            QuickHandStorage(tango);

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, GhostReactorCardController.Identifier);

            UsePower(GhostReactorCardController.Identifier);

            DealDamage(baron, tango, 10, DamageType.Psychic);
            DealDamage(tango, mdp, 1, DamageType.Projectile); // With +2 increase from Ghost Reactor
            DealDamage(tango, mdp, 1, DamageType.Projectile); // +1, Ghost Reactor effect expired

            // Assert
            QuickHPCheck(0, -4); // Tango One (0) due to Ghost Reactor immunity, MDP -4 
            QuickHandCheck(0); // Discard Ghost Reactor (-1), Draw a card (+1)

            //Act - Ensure not until start of next turn
            GoToStartOfTurn(tango);
            QuickHPStorage(tango);
            DealDamage(baron, tango, 10, DamageType.Psychic);

            //Assert
            QuickHPCheckZero();
        }

        [Test]
        public void TestInfiltrate()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                InfiltrateCardController.Identifier, GhostReactorCardController.Identifier
            });

            StartGame();

            PutOnDeck(tango, GetCard(CriticalHitCardController.Identifier));
            PutOnDeck(tango, GetCard(FarsightCardController.Identifier));


            DecisionSelectLocation = new LocationChoice(tango.HeroTurnTaker.Deck);
            DecisionSelectCard = GetCard(CriticalHitCardController.Identifier); // First drawn card to put back on deck
            DecisionSelectCardToPlay = GetCard(CriticalHitCardController.Identifier);

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, InfiltrateCardController.Identifier);

            // Assert
            AssertIsInPlay(GetCardInPlay(CriticalHitCardController.Identifier));
        }

        [Test]
        public void TestInfiltrate_Oblivaeon()
        {
            // Arrange
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TangoOne", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            AssertNumberOfChoicesInNextDecision(5, SelectionType.RevealTopCardOfDeck);
            PlayCard("Infiltrate");
        }

        [Test]
        public void TestLineEmUp_DestroyByDamage()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                LineEmUpCardController.Identifier
            });

            StartGame();

            Card bb = PlayCard(baron, "BladeBattalion");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            SetHitPoints(bb, 1);
            QuickHPStorage(mdp);
            DecisionSelectTargets = new[] { bb, mdp };


            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, LineEmUpCardController.Identifier);
            UsePower(tango);

            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestLineEmUp_DestroyByDestroyEffect()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                LineEmUpCardController.Identifier
            });

            StartGame();

            Card bb = PlayCard(baron, "BladeBattalion");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            DecisionSelectTargets = new[] { mdp };


            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, LineEmUpCardController.Identifier);
            DestroyCard(bb, tango.CharacterCard);


            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestLineEmUp_DestroyBySniperRifle()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                LineEmUpCardController.Identifier,
                SniperRifleCardController.Identifier,
                OneShotOneKillCardController.Identifier,
                OneShotOneKillCardController.Identifier
            });

            StartGame();

            Card bb = PlayCard(baron, "BladeBattalion");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            DecisionSelectCards = new[] {
                GetCardFromHand(OneShotOneKillCardController.Identifier, 0),
                //GetCardFromHand(OneShotOneKillCardController.Identifier, 1), //secondcard autoselected
                bb,
                mdp
            };

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, LineEmUpCardController.Identifier);
            var card = PlayCardFromHand(tango, SniperRifleCardController.Identifier);
            GoToUsePowerPhase(tango);
            UsePower(card, 0);

            // Assert
            QuickHPCheck(-1);
        }


        [Test]
        public void TestLineEmUp_DestroyByDisablingShot()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                LineEmUpCardController.Identifier,
                DisablingShotCardController.Identifier,
            });

            StartGame();

            Card ongoing = PlayCard(baron, "LivingForceField");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            DecisionSelectCards = new[] {
                ongoing,
                mdp,
                mdp
            };

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, LineEmUpCardController.Identifier);
            PlayCardFromHand(tango, DisablingShotCardController.Identifier);
            

            // Assert
            QuickHPCheck(-3);
        }
        [Test]
        public void TestLineEmUp_CharacterCardSelection()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TangoOne/PastTangoOneCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon, tango, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "TangoOne" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.TangoOne";
            RunActiveTurnPhase();

            PlayCard("LineEmUp");
            Card wasp = PlayCard("IronWasp");
            Card reporter = PlayCard("IntrepidReporter");

            AssertMaxNumberOfDecisions(1);
            DealDamage(tango, wasp, 10, DamageType.Melee);
        }

        [Test]
        public void TestOneShotShotOneKillSuccessfulDestruction()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                OneShotOneKillCardController.Identifier,
                GhostReactorCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 4);
            QuickHandStorage(tango);

            DecisionSelectCards = new[]
            {
                GetCardFromHand(tango, GhostReactorCardController.Identifier),
                GetCardFromHand(tango, FarsightCardController.Identifier),
                null,
                mdp
            };

            // Act

            AssertCardSpecialString(GetCardFromHand(OneShotOneKillCardController.Identifier), 0, $"Tango One's hand has {GetNumberOfCardsInHand(tango) - 1} other cards in it.");
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, OneShotOneKillCardController.Identifier);

            // Assert
            AssertNotInPlay(mdp); // 2 cards were discarded: 2 cards * 2 = 4 which MDP's HP qualifies for destruction
            QuickHandCheck(-2); // (4 -1 for playing One Shot, -2 for the 2 discards, +1 for successfully destroying a target)
        }

        [Test]
        public void TestOneShotShotOneKillMultipleEligibleTargetsForDestruction()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
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

            QuickHandStorage(tango);

            DecisionSelectCards = new[]
            {
                GetCardFromHand(tango, GhostReactorCardController.Identifier),
                GetCardFromHand(tango, FarsightCardController.Identifier),
                null,
                mdp
            };

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, OneShotOneKillCardController.Identifier);

            // Assert
            AssertNotInPlay(mdp); // 2 cards were discarded: 2 cards * 2 = 4 which MDP's HP qualifies for destruction
            AssertIsInPlay(bb); // Blade Battalion was not chosen as the destruction target
            QuickHandCheck(-2); // (4 -1 for playing One Shot, -2 for the 2 discards, +1 for successfully destroying a target)
        }

        [Test]
        public void TestOneShotShotOneKillUnsuccessfulDestruction()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                OneShotOneKillCardController.Identifier,
                GhostReactorCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 6);
            QuickHandStorage(tango);

            DecisionSelectCards = new[]
            {
                GetCardFromHand(tango, GhostReactorCardController.Identifier),
                GetCardFromHand(tango, FarsightCardController.Identifier),
                null,
                mdp
            };

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, OneShotOneKillCardController.Identifier);

            // Assert
            AssertIsInPlay(mdp); // 2 cards were discarded: 2 cards * 2 = 4 which MDP's HP (6) *DOES NOT* qualify for destruction
            QuickHandCheck(-3); // (4 -1 for playing One Shot, -2 for the 2 discards)
        }

        [Test]
        public void TestOpportunistShuffleTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                OpportunistCardController.Identifier
            });

            // Fill trash with a few cards so we can assert trash has changed if we shuffle it into the deck
            PutInTrash(tango, GetCard(ChameleonArmorCardController.Identifier));
            PutInTrash(tango, GetCard(GhostReactorCardController.Identifier));

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            QuickHandStorage(tango);
            QuickShuffleStorage(tango);

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, OpportunistCardController.Identifier);
            UsePower(tango); // Snipe power

            // Assert
            QuickHPCheck(-4); // Snipe (1) + Opportunist (+3)
            QuickHandCheck(1);
            Assert.AreEqual(1, GetNumberOfCardsInTrash(tango)); // One card in trash (Opportunist)
            QuickShuffleCheck(1); // Tango's deck was shuffled by Opportunist card
        }

        [Test]
        public void TestOpportunistDontShuffleTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                OpportunistCardController.Identifier
            });

            // Fill trash with a few cards so we can assert trash has changed after if we shuffle it into the deck
            PutInTrash(tango, GetCard(ChameleonArmorCardController.Identifier));
            PutInTrash(tango, GetCard(GhostReactorCardController.Identifier));

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            QuickHandStorage(tango);
            QuickShuffleStorage(tango);

            DecisionSelectTarget = mdp;
            DecisionYesNo = false;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, OpportunistCardController.Identifier);
            UsePower(tango); // Snipe power

            // Assert
            QuickHPCheck(-4); // Snipe (1) + Opportunist (+3)
            QuickHandCheck(1);
            Assert.AreEqual(3, GetNumberOfCardsInTrash(tango)); // (Opportunist, Ghost Reactor, Chameleon Armor)
            QuickShuffleCheck(0); // Tango's deck was not shuffled by Opportunist card
        }

        [Test]
        public void TestOpportunistNoTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                OpportunistCardController.Identifier
            });

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            QuickHandStorage(tango);
            QuickShuffleStorage(tango);

            DecisionSelectTarget = mdp;
            DecisionYesNo = false;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, OpportunistCardController.Identifier);
            UsePower(tango); // Snipe power

            // Assert
            QuickHPCheck(-4); // Snipe (1) + Opportunist (+3)
            QuickHandCheck(1);
            Assert.AreEqual(1, GetNumberOfCardsInTrash(tango)); // (Opportunist)
            QuickShuffleCheck(0); // Tango's deck was not shuffled by Opportunist card
        }



        [Test]
        public void TestPerfectFocusPlayCard()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
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

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, PerfectFocusCardController.Identifier);
            UsePower(tango); // Snipe power

            // Assert
            QuickHPCheck(-6); // Disabling Shot (2 + Perfect Focus +3), Snipe (1)
        }

        [Test]
        public void TestPerfectFocusDontPlayCard()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                PerfectFocusCardController.Identifier,
                DisablingShotCardController.Identifier,
                FarsightCardController.Identifier
            });

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, PerfectFocusCardController.Identifier);
            UsePower(tango); // Snipe power

            // Assert
            QuickHPCheck(-4); // (Perfect Focus +3), Snipe (1)
        }

        [Test]
        public void TestPerfectFocusNoCardsToPlay()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                PerfectFocusCardController.Identifier
            });

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, PerfectFocusCardController.Identifier);
            UsePower(tango); // Snipe power

            // Assert
            QuickHPCheck(-4); // (Perfect Focus +3), Snipe (1)
        }

        [Test]
        public void TestPsionicSuppression()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                PsionicSuppressionCardController.Identifier
            });

            SetHitPoints(ra, 20);
            SetHitPoints(legacy, 20);

            StartGame();
            Card bb = PlayCard(baron, "BladeBattalion");
            DecisionSelectTarget = bb;
            QuickHPStorage(tango);

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, PsionicSuppressionCardController.Identifier);

            GoToEndOfTurn(baron);
            GoToStartOfTurn(tango);

            // Assert
            QuickHPCheck(-5); // Battalion Blade hit prior to playing Psionic Suppression

        }

        [Test]
        public void TestSniperRiflePower1Success()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
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
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 0);

            // Assert
            AssertNotInPlay(mdp); // MDP destroyed by Sniper Rifle power #1
            Assert.AreEqual(2, GetNumberOfCardsInTrash(tango)); // (Ghost Reactor, Disabling Shot)
        }

        [Test]
        public void TestSniperRiflePower1Fail()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
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
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 0);

            // Assert
            AssertIsInPlay(mdp); // MDP was not destroyed by Sniper Rifle power #1 due to lack of discarded Critical cards
            Assert.AreEqual(1, GetNumberOfCardsInTrash(tango)); // (Disabling Shot)
        }

        [Test]
        public void TestSniperRiflePower1NoCardsInHand()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                SniperRifleCardController.Identifier // Critical keyword
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new[]
            {
                mdp
            };
            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 0);

            // Assert
            AssertIsInPlay(mdp); // MDP was not destroyed by Sniper Rifle power #1 due to lack of discarded Critical cards
            Assert.AreEqual(0, GetNumberOfCardsInTrash(tango));
        }

        [Test]
        public void TestSniperRiflePower2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                SniperRifleCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, SniperRifleCardController.Identifier);
            UsePower(GetCardInPlay(SniperRifleCardController.Identifier), 1);

            // Assert
            QuickHPCheck(-2);
        }

        [Test]
        public void TestWetWork()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(tango, new List<string>()
            {
                WetWorkCardController.Identifier
            });

            // Fill trash with a few cards so we can assert trash has changed after we shuffle some of it into the deck
            PutInTrash(tango, GetCard(ChameleonArmorCardController.Identifier));
            PutInTrash(tango, GetCard(DisablingShotCardController.Identifier));
            PutInTrash(tango, GetCard(GhostReactorCardController.Identifier));


            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            QuickShuffleStorage(tango, ra, baron, env);
            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;
            DecisionSelectCards = new[]
            {
                GetCard(DisablingShotCardController.Identifier),
                GetCard(GhostReactorCardController.Identifier),
                mdp
            };

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, WetWorkCardController.Identifier);


            // Assert
            QuickHPCheck(-2); // Wet Work (2)
            Assert.AreEqual(2, GetNumberOfCardsInTrash(tango)); // (ChameleonArmorCard, WetWork)
            QuickShuffleCheck(1, 1, 1, 1);

        }

        [Test]
        public void TestWetWork_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TangoOne", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Dictionary<Location, Card> topCardsOfSubDecks = new Dictionary<Location, Card>();

            SwitchBattleZone(haka);

            DiscardTopCards(oblivaeon, 1);
            PlayCard(oblivaeon, GetCard("AeonWarrior"), isPutIntoPlay: true, overridePlayLocation: scionOne.TurnTaker.PlayArea);
            foreach(Location subdeck in oblivaeon.TurnTaker.SubDecks.Where(d => d.IsRealDeck))
            {
                topCardsOfSubDecks.Add(subdeck, subdeck.TopCard);
                DiscardTopCards(subdeck, 1);
            }
            DiscardTopCards(tango, 1);
            DiscardTopCards(haka, 1);
            DiscardTopCards(legacy, 1);
            DiscardTopCards(envOne, 1);
            DiscardTopCards(envTwo, 1);

            Card wet = PlayCard("WetWork");

            AssertNumberOfCardsAtLocation(oblivaeon.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(tango.TurnTaker.Trash, 1);
            AssertNumberOfCardsAtLocation(haka.TurnTaker.Trash, 1);
            AssertNumberOfCardsAtLocation(legacy.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(envOne.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(envTwo.TurnTaker.Trash, 1);

            foreach (Location subtrash in oblivaeon.TurnTaker.SubTrashes.Where(d => d.IsRealTrash))
            {
                if (GameController.IsLocationVisibleToSource(subtrash, new CardSource(FindCardController(wet))))
                {
                    AssertNumberOfCardsAtLocation(subtrash, 0);
                }
                else
                {
                    AssertNumberOfCardsAtLocation(subtrash, 1);
                }
            }

            foreach (Location subdeck in oblivaeon.TurnTaker.SubDecks.Where(d => d.IsRealDeck))
            {
                if (GameController.IsLocationVisibleToSource(subdeck, new CardSource(FindCardController(wet))))
                {
                    AssertAtLocation(topCardsOfSubDecks[subdeck], subdeck);
                }
                else
                {
                    AssertAtLocation(topCardsOfSubDecks[subdeck], oblivaeon.TurnTaker.FindSubTrash(subdeck.Identifier));
                }
            }

        }


        [Test]
        public void TestWetWorkEmptyTrashes()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");


            MakeCustomHeroHand(tango, new List<string>()
            {
                WetWorkCardController.Identifier
            });

            StartGame();
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            QuickShuffleStorage(tango, ra, baron, env);
            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(tango);
            PlayCardFromHand(tango, WetWorkCardController.Identifier);


            // Assert
            QuickHPCheck(-2); // Wet Work (2)
            Assert.AreEqual(1, GetNumberOfCardsInTrash(tango)); // (WetWork)
            QuickShuffleCheck(1, 1, 1, 1);

        }

    }
}

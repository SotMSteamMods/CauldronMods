using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;

using Cauldron.Dendron;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class DendronTests : BaseTest
    {
        protected TurnTakerController Dendron => FindVillain("Dendron");

        private const string DeckNamespace = "Cauldron.Dendron";

        [Test]
        public void TestDendronDeckList()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card adornedOak = GetCard(AdornedOakCardController.Identifier);
            Assert.IsTrue(adornedOak.DoKeywordsContain("tattoo"));

            Card bloodThornAura = GetCard(BloodThornAuraCardController.Identifier);
            Assert.IsTrue(bloodThornAura.DoKeywordsContain("ongoing"));

            Card chokingInscription = GetCard(ChokingInscriptionCardController.Identifier);
            Assert.IsTrue(chokingInscription.DoKeywordsContain("one-shot"));

            Card darkDesign = GetCard(DarkDesignCardController.Identifier);
            Assert.IsTrue(darkDesign.DoKeywordsContain("one-shot"));

            Card inkScar = GetCard(InkScarCardController.Identifier);
            Assert.IsTrue(inkScar.DoKeywordsContain("one-shot"));

            Card livingInk = GetCard(LivingInkCardController.Identifier);
            Assert.IsTrue(livingInk.DoKeywordsContain("tattoo"));

            Card markOfTheWrithingNight = GetCard(MarkOfTheWrithingNightCardController.Identifier);
            Assert.IsTrue(markOfTheWrithingNight.DoKeywordsContain("one-shot"));

            Card obsidianSkin = GetCard(ObsidianSkinCardController.Identifier);
            Assert.IsTrue(obsidianSkin.DoKeywordsContain("ongoing"));

            Card paintedViper = GetCard(PaintedViperCardController.Identifier);
            Assert.IsTrue(paintedViper.DoKeywordsContain("tattoo"));

            Card restoration = GetCard(RestorationCardController.Identifier);
            Assert.IsTrue(restoration.DoKeywordsContain("one-shot"));

            Card shadedOwl = GetCard(ShadedOwlCardController.Identifier);
            Assert.IsTrue(shadedOwl.DoKeywordsContain("tattoo"));

            Card stainedWolf = GetCard(StainedWolfCardController.Identifier);
            Assert.IsTrue(stainedWolf.DoKeywordsContain("tattoo"));

            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            Assert.IsTrue(tintedStag.DoKeywordsContain("tattoo"));

            Card ursaMajor = GetCard(UrsaMajorCardController.Identifier);
            Assert.IsTrue(ursaMajor.DoKeywordsContain("tattoo"));
        }

        [Test]
        public void TestDendronLoads()
        {
            SetupGameController(DeckNamespace, "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Dendron);
            Assert.IsInstanceOf(typeof(DendronCharacterCardController), Dendron.CharacterCardController);

            Assert.AreEqual(50, Dendron.CharacterCard.HitPoints);
        }

        [Test]
        public void TestDendronStartGame()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            GoToStartOfTurn(Dendron);

            AssertNotFlipped(Dendron);


            // Stained Wolf and Painted Viper are put in play at game start
            Assert.IsNotEmpty(FindCardsWhere(card => card.Identifier.Equals(StainedWolfCardController.Identifier) 
                                                     && card.Location.Name == LocationName.PlayArea ));

            Assert.IsNotEmpty(FindCardsWhere(card => card.Identifier.Equals(PaintedViperCardController.Identifier)
                                                     && card.Location.Name == LocationName.PlayArea));

        }


        [Test]
        public void TestDendronFlipsWhenReducedToZeroHp()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            SetHitPoints(Dendron, 1);
            StartGame();

            PutInTrash(Dendron, GetCard(AdornedOakCardController.Identifier));
            DecisionSelectTarget = Dendron.CharacterCard;

            QuickShuffleStorage(Dendron);

            // Act
            AssertNotFlipped(Dendron);

            
            GoToUsePowerPhase(ra);
            UsePower(ra);
            
            // Assert
            AssertFlipped(Dendron); // Dendron flipped once reduced to 0 HP
            AssertHitPoints(Dendron, 50); // Dendron's HP was restored to 50
            QuickShuffleCheck(1); // Villain deck was shuffled
        }

        [Test]
        public void TestDendronFlipsWhenDestroyedByEffect()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Tachyon", "Megalopolis");

            SetHitPoints(Dendron, 1);
            StartGame();

            PutInTrash(Dendron, GetCard(AdornedOakCardController.Identifier));
            DecisionSelectCard = Dendron.CharacterCard;

            QuickShuffleStorage(Dendron);

            // Act
            AssertNotFlipped(Dendron);


            GoToPlayCardPhase(tachyon);
            PlayCard("SuckerPunch");

            // Assert
            AssertFlipped(Dendron); // Dendron flipped once reduced to 0 HP
            AssertHitPoints(Dendron, 50); // Dendron's HP was restored to 50
            QuickShuffleCheck(1); // Villain deck was shuffled
        }

        [Test]
        public void TestDendronPlaysCardIfBelowTattoosInPlayThreshold()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");

            
            StartGame();

            Card stag = PutOnDeck("TintedStag");
            Card owl = PutOnDeck("ShadedOwl");

            GoToEndOfTurn(FindEnvironment());
            // Act

            DestroyCard(GetStainedWolfInPlay(), ra.CharacterCard);
            DestroyCard(GetPaintedViperInPlay(), ra.CharacterCard);


            // Loop back around to Dendron and she should play an extra card from the deck due to no tattoos being out
            GoToStartOfTurn(Dendron);

            // Assert
            AssertInPlayArea(Dendron, owl);

            GoToEndOfTurn(Dendron);
            AssertInPlayArea(Dendron, stag);
        }

        [Test]
        public void TestDendronBackPlaysCardStartAndEnd()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");


            StartGame();


            GoToEndOfTurn(FindEnvironment());
            // Act

            FlipCard(Dendron);
            AssertFlipped(Dendron.CharacterCard);


            Card stag = PutOnDeck("TintedStag");
            Card owl = PutOnDeck("ShadedOwl");

            // Loop back around to Dendron and she should play an extra card from the deck due to no tattoos being out
            GoToStartOfTurn(Dendron);

            // Assert
            AssertInPlayArea(Dendron, owl);

            GoToEndOfTurn(Dendron);
            AssertInPlayArea(Dendron, stag);
        }

        [Test]
        public void TestNonFlippedAdvancedIncreaseTattooDamage()
        {
            // Advanced: Increase damage dealt by tattoos by 1.

            // Arrange
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" }, 
                advanced: true, advancedIdentifiers: new[] { DeckNamespace });

            QuickHPStorage(legacy, ra, haka);
            StartGame();

            GoToEndOfTurn(Dendron);

            // Tattoos deal +1 damage: -2 on Ra from Painted Viper (normally -1), -3 on Haka from Stained Wolf (normally -2)
            QuickHPCheck(0, -2, -3);
        }

        [Test]
        public void TestFlippedAdvancedDendronGainsHealth()
        {
            // Advanced: At the start of the villain turn, {Dendron} regains 5 HP.

            // Arrange
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" },
                advanced: true, advancedIdentifiers: new[] { DeckNamespace });

            Card staffOfRa = GetCard("TheStaffOfRa");

            QuickHPStorage(legacy, ra, haka);
            SetHitPoints(Dendron, 1);

            StartGame();

            StackDeck(Dendron, "ShadedOwl");
            //keep from messing up the test with random healing
            PutInTrash(Dendron, Dendron.TurnTaker.Deck.Cards.Where(c => c.Identifier == "Restoration"));

            GoToEndOfTurn(Dendron);

            // Act
            GoToPlayCardPhase(ra);
            PlayCard(staffOfRa);
            GoToUsePowerPhase(ra);
            UsePower(ra);
            UsePower(ra);
            UsePower(ra);

            QuickHPStorage(Dendron);

            //Have to keep her *start*-of-turn card play from messing things up too, after the reshuffle
            PutInTrash(Dendron, Dendron.TurnTaker.Deck.Cards.Where(c => c.Identifier == "Restoration"));

            GoToStartOfTurn(Dendron);

            // Assert
            QuickHPCheck(5);
        }


        [Test]
        public void TestAdornedOak()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card adornedOwl = PlayCard(AdornedOakCardController.Identifier);

            QuickHPStorage(GetStainedWolfInPlay(), GetPaintedViperInPlay(),Dendron.CharacterCard);

            // Act
            DealDamage(ra, GetStainedWolfInPlay(), 3, DamageType.Fire);
            DealDamage(ra, GetPaintedViperInPlay(), 3, DamageType.Fire);
            //check that it only reduces for tattoos
            DealDamage(ra, Dendron, 3, DamageType.Fire);


            // Assert
            QuickHPCheck(-2, -2, -3);
        }

        [Test]
        public void TestBloodThornAura()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card bloodThornAura = PlayCard(BloodThornAuraCardController.Identifier);
            Card monorail = PlayCard("PlummetingMonorail");
            QuickHPStorage(GetStainedWolfInPlay(), GetPaintedViperInPlay(), ra.CharacterCard, monorail, Dendron.CharacterCard);

            // Act
            DealDamage(ra, GetStainedWolfInPlay(), 1, DamageType.Fire);
            DealDamage(ra, GetPaintedViperInPlay(), 1, DamageType.Fire);
            DealDamage(monorail, GetPaintedViperInPlay(), 1, DamageType.Projectile);
            DealDamage(Dendron, GetStainedWolfInPlay(), 1, DamageType.Projectile);

            // Assert
            QuickHPCheck(-2, -2, -2, -1, 0);
        }

        [Test]
        public void TestChokingInscription()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Expatriette", "Haka", "Megalopolis");

            StartGame();
            QuickShuffleStorage(legacy, expatriette, haka);

            Card chokingInscription = GetCard(ChokingInscriptionCardController.Identifier);
            Card assaultRifle = GetCard("AssaultRifle");
            Card jacket = PutInHand("FlakJacket");
            PutInHand("Unload"); //so Expariatte has cards in hand that she shouldn't be able to play
            Card bolsterAllies = PutInHand("BolsterAllies");

            // Act
            PlayCard(assaultRifle); // Put Assault Rifle into play so expatriette will incur the most cards in play penalty
            //have legacy draw a card to ensure they have the most cards in hand
            DrawCard(legacy);

            GoToPlayCardPhase(Dendron);

            AssertNumberOfStatusEffectsInPlay(0);

            //Chocking Inscription applies a series of status effects to targets.
            PlayCard(chokingInscription);

            AssertNumberOfStatusEffectsInPlay(2);

            // Legacy and Expatriette were both affected with phase penalties.  Haka was the only one unaffected so he shuffles his trash into his deck
            QuickShuffleCheck(0, 0, 1);
            QuickHandStorage(legacy);
            GoToPlayCardPhase(legacy);
            RunCoroutine(base.GameController.DrawCard(legacy.HeroTurnTaker, cardSource: legacy.CharacterCardController.GetCardSource()));
            
            QuickHandCheck(0);

            PlayCard(bolsterAllies);
            QuickHandCheck(-1);

            GoToDrawCardPhase(legacy);

            AssertCannotPerformPhaseAction();

            GoToEndOfTurn(legacy);

            GoToStartOfTurn(expatriette);

            AssertCannotPlayCards(expatriette);

            GoToUsePowerPhase(expatriette);
            DecisionSelectCard = jacket;
            UsePower(expatriette.CharacterCard);
            AssertInHand(jacket);

            GoToDrawCardPhase(expatriette);
            RunActiveTurnPhase();
            GoToEndOfTurn(expatriette);

            //Expiration
            AssertNextMessageContains("Expatriette cannot play cards.");
            GoToStartOfTurn(haka);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void TestChokingInscription_BothEffectsOnSameHero()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Expatriette", "Haka", "Megalopolis");

            StartGame();
            QuickShuffleStorage(legacy, expatriette, haka);

            Card chokingInscription = GetCard(ChokingInscriptionCardController.Identifier);
            Card assaultRifle = GetCard("AssaultRifle");
            Card jacket = PutInHand("FlakJacket");
            PutInHand("Unload"); //so Expariatte has cards in hand that she shouldn't be able to play

            // Act
            PlayCard(assaultRifle); // Put Assault Rifle into play so expatriette will incur the most cards in play penalty
            //have expat draw cards to ensure they have the most cards in hand
            DrawCard(expatriette, 2);

            GoToPlayCardPhase(Dendron);

            AssertNumberOfStatusEffectsInPlay(0);

            //Chocking Inscription applies a series of status effects to targets.
            PlayCard(chokingInscription);

            AssertNumberOfStatusEffectsInPlay(2);

            // Expatriette affected with phase penalties.  Legacy and Haka shuffles his trash into his deck
            QuickShuffleCheck(1, 0, 1);

            GoToStartOfTurn(expatriette);

            AssertCannotPlayCards(expatriette);

            QuickHandStorage(expatriette);
            RunCoroutine(base.GameController.DrawCard(expatriette.HeroTurnTaker, cardSource: expatriette.CharacterCardController.GetCardSource()));
            QuickHandCheck(0);


            GoToUsePowerPhase(expatriette);
            DecisionSelectCard = jacket;
            UsePower(expatriette.CharacterCard);
            AssertInHand(jacket);

            GoToDrawCardPhase(expatriette);
            AssertCannotPerformPhaseAction();

            GoToEndOfTurn(expatriette);

            //Expiration
            GoToStartOfTurn(haka);
            AssertNumberOfStatusEffectsInPlay(0);
        }


        [Test]
        public void TestDarkDesign()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            GoToStartOfTurn(legacy);
            DealDamage(legacy, Dendron, 4, DamageType.Melee);
            
            DealDamage(ra, Dendron, 4, DamageType.Fire); // Ra will be the last person to deal damage to Dendron

            QuickHandStorage(legacy, ra, haka);

            // Act

            GoToStartOfTurn(Dendron);
            Card darkDesign = GetCard(DarkDesignCardController.Identifier);
            PlayCard(darkDesign);

            // Assert
            QuickHandCheck(0, -3, 0); // Ra had to discard <H> cards (3) due to Dark Design

        }

        [Test]
        public void TestInkScar()
        {
            // Arrange
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" });

            StartGame();

            var c1 = PutInTrash(TintedStagCardController.Identifier);
            var c2 = PutInTrash(LivingInkCardController.Identifier);

            //put all non targets in the trash
            MoveCards(Dendron, c => c.Owner == Dendron.TurnTaker && !c.IsTarget, Dendron.TurnTaker.Trash);
            Card bloodThornAura = GetCard(BloodThornAuraCardController.Identifier);

            //then load an inkscar as the top card
            Card inkScar = GetCard(InkScarCardController.Identifier);
            PutOnDeck(Dendron, inkScar);

            AssertNumberOfCardsInPlay(Dendron, 3); //dendron, viper, wolf

            QuickShuffleStorage(Dendron.TurnTaker.Deck);

            GoToPlayCardPhase(Dendron);
            //plays inkscar
            RunActiveTurnPhase();
            // Assert

            AssertInTrash(Dendron, bloodThornAura); //still in trach
            AssertInTrash(Dendron, inkScar); //in trash after being played
            AssertNotInTrash(c1, c2); //could be in the deck or playarea
            AssertNumberOfCardsInPlay(Dendron, 5); //base plus 2 more from inkscar
            QuickShuffleCheck(1);
        }

        [Test]
        public void TestLivingInk()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodThornAura = GetCard(BloodThornAuraCardController.Identifier);
            PutInTrash(bloodThornAura);
            
            Card livingInk = GetCard(LivingInkCardController.Identifier);

            StartGame();

            SetHitPoints(Dendron.CharacterCard, 40);
            SetHitPoints(GetStainedWolfInPlay(), 1);
            SetHitPoints(GetPaintedViperInPlay(), 1);
            QuickHPStorage(Dendron.CharacterCard, GetStainedWolfInPlay(), GetPaintedViperInPlay());

            // Act
            PlayCard(livingInk);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(3, 3, 3);
        }

        [Test]
        public void TestMarkOfTheWrithingNightNoOngoingsOrEquipment()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card markOfTheWritingNight = GetCard(MarkOfTheWrithingNightCardController.Identifier);

            StartGame();
            QuickHPStorage(legacy, ra, haka);

            // Act
            PlayCard(markOfTheWritingNight);
            GoToEndOfTurn(Dendron);

            // Assert

            // Legacy: -2 fr Stained Wolf,
            // Ra: 0
            // Haka: -5 for hero with highest HP (Mark), -2 for hero with lowest HP (Mark - became lowest after highest HP damage), -1 from Painted Viper
            QuickHPCheck(-2, 0, -8); 
        }

        [Test]
        public void TestMarkOfTheWrithingNightWithEquipmentOnly()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card markOfTheWritingNight = GetCard(MarkOfTheWrithingNightCardController.Identifier);

            StartGame();

            Card legacyRing = GetCard("TheLegacyRing");
            Card mere = GetCard("Mere");

            PlayCard(legacyRing);
            PlayCard(mere);

            QuickHPStorage(legacy, ra, haka);

            // Act
            PlayCard(markOfTheWritingNight);
            GoToEndOfTurn(Dendron);

            // Assert

            // Legacy: -2 fr Stained Wolf,
            // Ra: 0
            // Haka: -5 for hero with highest HP (Mark), -2 for hero with lowest HP (Mark - became lowest after highest HP damage), -1 from Painted Viper
            QuickHPCheck(-2, 0, -8);
            AssertInTrash(legacy, legacyRing);
            AssertNotInTrash(haka, mere.Identifier); // Character with least cards in play is Ra (0) 
        }

        [Test]
        public void TestMarkOfTheWrithingNightWithMixedTypes()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card markOfTheWritingNight = GetCard(MarkOfTheWrithingNightCardController.Identifier);

            StartGame();


            Card dangerSense = GetCard("DangerSense");
            Card fortitude = GetCard("Fortitude");
            Card legacyRing = GetCard("TheLegacyRing");
            Card nextEvolution = GetCard("NextEvolution");

            Card dominion = GetCard("Dominion");

            DecisionSelectCards = new[] {dangerSense, fortitude};


            PlayCard(dangerSense);
            PlayCard(fortitude);
            PlayCard(legacyRing);
            PlayCard(nextEvolution);

            PlayCard(dominion);

            QuickHPStorage(legacy, ra, haka);

            // Act
            PlayCard(markOfTheWritingNight);
            GoToEndOfTurn(Dendron);

            // Assert

            // Legacy: -2 fr Stained Wolf,
            // Ra: 0
            // Haka: -5 for hero with highest HP (Mark), -2 for hero with lowest HP (Mark - became lowest after highest HP damage), -1 from Painted Viper
            QuickHPCheck(-2, 0, -8);
            AssertInTrash(legacy, new [] {dangerSense, fortitude, legacyRing }); // 2 ongoings, 1 equip (Most cards in play)
            AssertInPlayArea(legacy, new [] { nextEvolution}); // 1 ongoing that wasn't destroyed
            AssertNotInTrash(haka, dominion.Identifier); // Character with least cards in play is Ra (0) 

            
        }



        [Test]
        public void TestObsidianSkin()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            QuickHPStorage(Dendron);

            Card obsidianSkin = GetCard(ObsidianSkinCardController.Identifier);

            GoToPlayCardPhase(Dendron);
            PlayCard(obsidianSkin);

            // Act
            DealDamage(ra, Dendron, 4, DamageType.Fire);

            // Assert
            QuickHPCheck(-3); // -1 reduction from Obsidian Skin

        }

        [Test]
        public void TestPaintedViper()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            QuickHPStorage(ra);

            // Act

            // Painted Viper will be in play @ game start

            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(-1); // Painted Viper deals hero with lowest HP H - 2 toxic (1 damage in this case)
        }

        [Test]
        public void TestRestoration()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            SetHitPoints(Dendron, 30);
            SetHitPoints(GetPaintedViperInPlay(), 1);
            SetHitPoints(GetStainedWolfInPlay(), 1);


            QuickHPStorage(Dendron.CharacterCard, GetPaintedViperInPlay(), GetStainedWolfInPlay());

            Card restoration = GetCard(RestorationCardController.Identifier);


            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(restoration);

            // Assert
            QuickHPCheck(10, 7, 7);

        }

        [Test]
        public void TestShadedOwl()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card shadedOwl = GetCard(ShadedOwlCardController.Identifier);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(shadedOwl);
            GoToEndOfTurn(Dendron);

            // Assert

            // Legacy: -2 from Shaded Owl (+1 from Shaded Owl)
            // Ra: -2 from Painted Viper (+1 from Shaded Owl), -2 from Shaded Owl (+1 from Shaded Owl)
            // Haka: -3 from Stained Wolf (+1 from Shaded Owl), -2 from Shaded Owl (+1 from Shaded Owl)
            QuickHPCheck(-2, -4, -5);
        }

        [Test]
        public void TestStainedWolf()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(0, -1, -2); // 0 for Legacy, -1 Ra from Painted Viper (lowest hp hero), -2 Haka from Stained Wolf (highest hp hero)
        }

        [Test]
        public void TestTintedStagEmptyTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
        }

        [Test]
        public void TestTintedStagNoTattoosInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            // Put some non Tattoo cards in trash
            PutInTrash(Dendron, GetCard(RestorationCardController.Identifier));
            PutInTrash(Dendron, GetCard(InkScarCardController.Identifier));


            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);


        }

        [Test]
        public void TestTintedStagTattooInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            // Put one Tattoo card in trash
            Card stainedWolfCopy2 = GetCard(StainedWolfCardController.Identifier);
            PutInTrash(Dendron, stainedWolfCopy2);
            PutInTrash(Dendron, GetCard(RestorationCardController.Identifier));


            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
            AssertIsInPlay(stainedWolfCopy2);

        }

        [Test]
        public void TestTintedStagMultipleTattoosInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            // Put multiple Tattoo card in trash
            PutInTrash(Dendron, GetCard(StainedWolfCardController.Identifier));
            PutInTrash(Dendron, GetCard(PaintedViperCardController.Identifier));
            PutInTrash(Dendron, GetCard(ObsidianSkinCardController.Identifier));
            PutInTrash(Dendron, GetCard(AdornedOakCardController.Identifier));


            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
            Assert.True(GetNumberOfCardsInPlay(Dendron) >= 5); // At least Dendron, Stained Wolf, Painted Viper, Tinted Stag, Tattoo from trash
        }

        [Test]
        public void TestUrsaMajor()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card ursaMajor = GetCard(UrsaMajorCardController.Identifier);
            PutOnDeck(Dendron, ursaMajor);

            QuickHPStorage(ursaMajor);
            DecisionSelectTarget = legacy.CharacterCard;
            
            

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(ursaMajor);
            GoToEndOfTurn(Dendron);

            DealDamage(ra, ursaMajor, 3, DamageType.Fire);

            // Assert
            
            // Legacy: -2 fr Ursa Major, Ra: -1 fr Painted Viper, Haka: -2 fr Stained Wolf, Ursa Major: -2 due to 1 damage reduction on it
            QuickHPCheck(-2); // Ursa Major: -2 due to 1 damage reduction on it

        }


        private Card GetStainedWolfInPlay()
        {
            return GetCardInPlay(StainedWolfCardController.Identifier);
        }

        private Card GetPaintedViperInPlay()
        {
            return GetCardInPlay(PaintedViperCardController.Identifier);
        }

    }
}

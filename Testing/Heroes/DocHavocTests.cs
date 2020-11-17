using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.DocHavoc;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture()]
    public class DocHavocTests : BaseTest
    {
        protected HeroTurnTakerController DocHavoc => FindHero("DocHavoc");


        [Test]
        public void TestDocHavocLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(DocHavoc);
            Assert.IsInstanceOf(typeof(DocHavocCharacterCardController), DocHavoc.CharacterCardController);

            Assert.AreEqual(30, DocHavoc.CharacterCard.HitPoints);
        }


        [Test]
        public void TestDocHavocInnatePowerSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Setup to reduce variance
            Card cardToPlay = PutInHand("FlameBarrier");

            GoToUsePowerPhase(DocHavoc);
            DecisionSelectTarget = ra.CharacterCard;
            DecisionSelectCard = cardToPlay;

            QuickHandStorage(ra);
            QuickHPStorage(ra);

            // Act

            //{DocHavoc} deals 1 hero 3 toxic damage. If that hero took damage this way, they may play a card now.
            UsePower(DocHavoc.CharacterCard);

            // Assert
            QuickHPCheck(-3);
            QuickHandCheck(-1);
            AssertInPlayArea(ra, cardToPlay);

        }

        [Test]
        public void TestDocHavocInnatePowerFail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Cauldron.DocHavoc", "Haka", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(legacy);
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            PlayCard(nextEvolution);
            GoToUsePowerPhase(legacy);
            DecisionSelectDamageType = DamageType.Toxic;
            UsePower(nextEvolution);

            GoToUsePowerPhase(DocHavoc);
            DecisionSelectTarget = legacy.CharacterCard;

            QuickHandStorage(legacy);
            QuickHPStorage(legacy);

            // Act
            //{ DocHavoc} deals 1 hero 3 toxic damage. If that hero took damage this way, they may play a card now.
            UsePower(DocHavoc.CharacterCard);

            // Assert
            QuickHPCheck(0); // Legacy immune to toxic, no change to HP
            QuickHandCheck(0); // Hand size should still be 4 as Legacy could only play a card if he sustained damage
        }

        [Test]
        public void TestIncapacitateOption1_3Targets()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //incap doc havoc
            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 18);
            SetHitPoints(haka.CharacterCard, 18);

            Card[] healTargets = new Card[] { legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard };
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(DocHavoc);

            // Act
            UseIncapacitatedAbility(DocHavoc, 0);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(0,0,1,1,1);

        }

        [Test]
        public void TestIncapacitateOption1_2Targets()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //incap doc havoc
            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 18);

            Card[] healTargets = new Card[] { legacy.CharacterCard, ra.CharacterCard, null};
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(DocHavoc);

            // Act
            UseIncapacitatedAbility(DocHavoc, 0);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(0, 0, 1, 1, 0);

        }

        [Test]
        public void TestIncapacitateOption1_1Target()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //incap doc havoc
            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);
            SetHitPoints(legacy.CharacterCard, 15);

            Card[] healTargets = new Card[] { legacy.CharacterCard, null};
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(DocHavoc);

            // Act
            UseIncapacitatedAbility(DocHavoc, 0);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(0, 0, 1, 0, 0);

        }

        [Test()]
        public void TestIncapacitateOption1_0Targets()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //incap doc havoc
            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);


            
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionDoNotSelectCard = SelectionType.GainHP;
            GoToUseIncapacitatedAbilityPhase(DocHavoc);

            // Act
            UseIncapacitatedAbility(DocHavoc, 0);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(0, 0, 0, 0, 0);

        }

        [Test]
        public void TestIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            DecisionSelectTarget = legacy.CharacterCard;

            QuickHandStorage(legacy);

            // Act
            UseIncapacitatedAbility(DocHavoc, 1);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHandCheck(1);

        }

        [Test]
        public void TestIncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Legacy", "Ra", "MobileDefensePlatform");
            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            QuickHPStorage(new[] { legacy.CharacterCard, ra.CharacterCard });

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 2);
            
            GoToStartOfTurn(env);

            PutIntoPlay("BattalionGunner"); // End of env. each hero target takes 1 damage

            GoToEndOfTurn(env);

            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(0, 0);

            //check that hero and villains can still deal damage
            QuickHPStorage(ra);
            DealDamage(baron, ra, 3, DamageType.Lightning);
            QuickHPCheck(-3);

            QuickHPStorage(ra);
            DealDamage(legacy, ra, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestDocsFlask()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            SetHitPoints(baron.CharacterCard, 35);
            SetHitPoints(mdp, 5);
            SetHitPoints(DocHavoc.CharacterCard, 20);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, DocHavoc.CharacterCard, haka.CharacterCard);

            // Act
            PutInHand(DocsFlaskCardController.Identifier);
            Card docsFlask = GetCardFromHand(DocsFlaskCardController.Identifier);
            PlayCard(docsFlask);

            DecisionSelectCard = ra.CharacterCard;
            // Procs flask to heal Ra +1 HP
            GoToStartOfTurn(DocHavoc);

            QuickHPCheck(0, 0, 1, 0, 0);
            QuickHPUpdate();

            // Now use power portion of Doc's Flask
            GoToUsePowerPhase(DocHavoc);
            UsePower(docsFlask);

            // Assert
            AssertTriggersWhere((Func<ITrigger, bool>)(t => t.Types.Contains(TriggerType.GainHP)));

            QuickHPCheck(0, 0, 1, 1, 1);
        }

        [Test]
        public void TestFieldDressing()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "Megalopolis");

            // We need to make an explicit hand so two FieldDressings aren't in hand so it doesn't play another FieldDressing which will throw off the Asserts
            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier, RecklessChargeCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            SetHitPoints(baron.CharacterCard, 35);
            SetHitPoints(mdp, 5);
            SetHitPoints(DocHavoc.CharacterCard, 20);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, DocHavoc.CharacterCard, haka.CharacterCard);
            QuickHandStorage(DocHavoc);

            // Act
            Card recklessCharge = GetCardFromHand(RecklessChargeCardController.Identifier);
            DecisionSelectCardToPlay = recklessCharge;

            GoToPlayCardPhase(DocHavoc);
            Card fieldDressing = GetCardFromHand(FieldDressingCardController.Identifier);
            PlayCard(fieldDressing);

            // Assert
            QuickHPCheck(0,0,1,1,1);
            QuickHandCheck(-2);
            AssertInPlayArea(DocHavoc, recklessCharge);
        }

        [Test]
        public void TestFieldDressing_NoPlay()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "Megalopolis");

            // We need to make an explicit hand so two FieldDressings aren't in hand so it doesn't play another FieldDressing which will throw off the Asserts
            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier, RecklessChargeCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            SetHitPoints(baron.CharacterCard, 35);
            SetHitPoints(mdp, 5);
            SetHitPoints(DocHavoc.CharacterCard, 20);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, DocHavoc.CharacterCard, haka.CharacterCard);
            QuickHandStorage(DocHavoc);

            // Act
            Card recklessCharge = GetCardFromHand(RecklessChargeCardController.Identifier);
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            GoToPlayCardPhase(DocHavoc);
            Card fieldDressing = GetCardFromHand(FieldDressingCardController.Identifier);
            PlayCard(fieldDressing);

            // Assert
            QuickHPCheck(0, 0, 1, 1, 1);
            QuickHandCheck(-1);
            AssertInHand(DocHavoc, recklessCharge);
        }

        [Test]
        public void TestPhosphorBlast()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Legacy", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card bladeBattalion = GetCard("BladeBattalion");
            PlayCard(bladeBattalion);
            QuickHPStorage(baron.CharacterCard, mdp, bladeBattalion, DocHavoc.CharacterCard, legacy.CharacterCard, ra.CharacterCard);


            // Act
            Card phosphorBlast = GetCard(PhosphorBlastCardController.Identifier);
            PutInHand(phosphorBlast);
            GoToPlayCardPhase(DocHavoc);
            PlayCard(phosphorBlast);


            // Assert

            /*
             * Check HP Loss:
             * Baron (0: Immune)
             * MDP (-1: Phosphor Blast)
             * Blade Battalion (-1: Phosphor Blast)
             * Doc Havoc (-1: Phosphor Blast)
             * Legacy (-6: Blade Battalion, Phosphor Blast)
             * Ra (-1: Phosphor Blast)
             *
             */
            QuickHPCheck(0, -1, -1, -1, -6, -1);

            // 2 status effects in play (MDP, Blade Battalion)
            AssertNumberOfStatusEffectsInPlay(2);

            // Both should be of type CannotGainHPStatusEffect
            Assert.AreEqual(2 ,this.GameController.StatusEffectControllers.Count(sec => sec.StatusEffect is CannotGainHPStatusEffect));

            // Go back around to Doc Havoc's turn again to ensure effects have expired
            GoToStartOfTurn(DocHavoc);

            // Both status effects should have expired
            AssertNumberOfStatusEffectsInPlay(0);

        }

        [Test]
        public void TestGasMask()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "InsulaPrimalis");
            PutInHand(GasMaskCardController.Identifier);

            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 20);
            QuickHPStorage(DocHavoc);

            // Act
            GoToPlayCardPhase(DocHavoc);
            Card gasMask = GetCardFromHand(GasMaskCardController.Identifier);
            PlayCard(gasMask);

            GoToStartOfTurn(env);
            Card volcanicEruption = GetCard("ObsidianField");
            PlayCard(volcanicEruption);
            DestroyCard(volcanicEruption, ra.CharacterCard);

            // Assert
            Assert.AreEqual(1,
                this.GameController.FindTriggersWhere((Func<ITrigger, bool>)(t => t.Types.Contains(TriggerType.GainHP))).Count());
            AssertTriggersWhere((Func<ITrigger, bool>)(t => t.Types.Contains(TriggerType.GainHP)));
            QuickHPCheck(2);


            //check not when non-Environment cards are destroyed
            QuickHPUpdate();
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), ra.CharacterCard);
            QuickHPCheckZero();
        }

        [Test]
        public void TestRecklessCharge()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier, 
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            QuickHPStorage(DocHavoc, ra);

            // Act
            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, RecklessChargeCardController.Identifier);

            GoToDrawCardPhase(DocHavoc);
            GoToEndOfTurn(DocHavoc);

            GoToStartOfTurn(env);
            Card mysticalDefences = GetCard("MysticalDefenses");
            PlayCard(mysticalDefences);
            GoToEndOfTurn(env);

            // Assert
            Assert.AreEqual(1,
                this.GameController.FindTriggersWhere((Func<ITrigger, bool>)(t 
                    => t.CardSource.Card.Identifier == RecklessChargeCardController.Identifier 
                       && t.Types.Contains(TriggerType.IncreaseDamage))).Count());

            Assert.AreEqual(1,
           this.GameController.FindTriggersWhere((Func<ITrigger, bool>)(t 
                => t.CardSource.Card.Identifier == RecklessChargeCardController.Identifier
                && t.ActionType == typeof(PhaseChangeAction)
                )).Count());


            QuickHPCheck(-3, -2); // 1 Mystical Defenses dmg + 1 from Reckless Charge for Doc Havoc

        }

        [Test]
        public void TestRapidRegen()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                RapidRegenCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            //PlayCard("CustomHandCannon");
            DealDamage(tempest, baron, 20, DamageType.Lightning);
            DealDamage(baron, DocHavoc, 4, DamageType.Projectile);
            DealDamage(baron, tempest, 4, DamageType.Projectile);

            StartGame();

            // Act
            GoToPlayCardPhase(DocHavoc);

            PlayCardFromHand(DocHavoc, RapidRegenCardController.Identifier);

            QuickHPStorage(DocHavoc, tempest, baron);

            GoToPlayCardPhase(tempest);
            PutInHand("CleansingDownpour");
            Card cleansingDownpour = GetCardFromHand("CleansingDownpour");

            PlayCard(cleansingDownpour);
            UsePower(cleansingDownpour);

            PlayCard(baron, "FleshRepairNanites");

            // Assert
            QuickHPCheck(3, 3, 11); // Heroes: Cleansing Rains +2 (+1 with Rapid Regen), // Baron: Flesh-Repair Nanites +10 (+1 with Rapid Regen)

        }

        [Test]
        public void TestStimShot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                StimShotCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            QuickHandStorage(DocHavoc);

            DecisionSelectTurnTaker = tempest.TurnTaker;

            // Act
            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, StimShotCardController.Identifier);

            // Assert
            QuickHandCheck(0); // No change as Stim Shot allowed Doc Havoc to draw a card
            QuickHPCheck(-1); // Stim Shot allowed Tempest to use power Squall, 1 dmg to MDP
        }

        [Test]
        public void TestSyringeDarts()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                SyringeDartsCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            
            DecisionSelectTargets = new[] {mdp, null};
            DecisionSelectWordSkip = true;

            // Act
            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, SyringeDartsCardController.Identifier);
            GoToUsePowerPhase(DocHavoc);
            UsePower(SyringeDartsCardController.Identifier);

            GoToEndOfTurn(env);

            // Assert
            QuickHPCheck(-2);
        }

        [Test]
        public void TestSearchAndRescue()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                SearchAndRescueCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            PutOnDeck(DocHavoc, GetCard(BrawlerCardController.Identifier));
            PutOnDeck(DocHavoc, GetCard(StimShotCardController.Identifier));
            PutOnDeck(DocHavoc, GetCard(DocsFlaskCardController.Identifier));

            StartGame();

            QuickHandStorage(DocHavoc, tempest);

            DecisionSelectCards = new[] 
            {
                GetCardFromHand(RecklessChargeCardController.Identifier), 
                null,
                GetCard(DocsFlaskCardController.Identifier),
                GetCard(BrawlerCardController.Identifier),
                GetCard(StimShotCardController.Identifier)
            };

            // Act
            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, SearchAndRescueCardController.Identifier);

            // Assert

            // DocHavoc started with 4, played Search & Rescue (-1), discarded Reckless Charge (-1), put Docs Flask into hand (+1)
            // Tempest did not discard so Search and Rescue did not proc for him
            QuickHandCheck(-1, 0);

            // Reckless charge and Stim shot should be in Doc's trash
            Assert.IsNotNull(GetCardFromTrash(DocHavoc, RecklessChargeCardController.Identifier) != null);
            Assert.IsNotNull(GetCardFromTrash(DocHavoc, StimShotCardController.Identifier) != null);

            // Bottom of Doc's deck should be Brawler
            Assert.AreEqual(BrawlerCardController.Identifier, GetBottomCardOfDeck(DocHavoc).Identifier);

        }

        [Test]
        public void TestImmediateEvac()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                ImmediateEvacCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });


            StartGame();

            SetHitPoints(baron.CharacterCard, 35);

            PutInTrash(DocHavoc, new List<Card>() { GetCard(DocsFlaskCardController.Identifier), GetCard(BrawlerCardController.Identifier) });
            PutInTrash(tempest, new List<Card>() { GetCard("ChainLightning"), GetCard("FlashFlood") });
            PutInTrash(haka, new List<Card>() { GetCard("Mere"), GetCard("GroundPound") });

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 5);

            QuickHPStorage(baron.CharacterCard, mdp, DocHavoc.CharacterCard, tempest.CharacterCard, haka.CharacterCard);
            QuickHandStorage(DocHavoc, tempest, haka);

            DecisionSelectFunctions = new int?[]{ 1, 0, 1};

            DecisionSelectCards = new[] {GetCardFromHand(DocHavoc, RecklessChargeCardController.Identifier), GetCardFromTrash(tempest, "FlashFlood"), GetCardFromHand(haka) };

            // Act
            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, ImmediateEvacCardController.Identifier);

            // Assert
            QuickHPCheck(2, 2, 0, 0,0); // Villain HP gain check
            QuickHandCheck(0, 1, 1);

        }

        [Test]
        public void TestPainkillersAcceptSelfDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                PainkillersCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            QuickHPStorage(tempest.CharacterCard, GetCardInPlay("MobileDefensePlatform"));

            // Act
            DecisionNextToCard = tempest.CharacterCard;
            DecisionSelectTarget = GetCardInPlay("MobileDefensePlatform");
            DecisionYesNo = true;

            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, PainkillersCardController.Identifier);

            GoToStartOfTurn(tempest);

            PutInHand(tempest, "LightningSlash");
            PlayCardFromHand(tempest, "LightningSlash");
            
            // Assert
            QuickHPCheck(-2, -5); // Tempest lost 2 HP for keeping Painkillers, -5 HP on MDP from Lightning Slash
            AssertNumberOfCardsNextToCard(tempest.CharacterCard, 1); // Painkillers next to Tempest

            AssertTriggersWhere(trigger => trigger.CardSource.Card.Identifier.Equals(PainkillersCardController.Identifier));
        }

        [Test]
        public void TestPainkillersDeclineSelfDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                PainkillersCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            QuickHPStorage(tempest.CharacterCard, GetCardInPlay("MobileDefensePlatform"));

            // Act
            DecisionNextToCard = tempest.CharacterCard;
            DecisionSelectTarget = GetCardInPlay("MobileDefensePlatform");
            DecisionYesNo = false;

            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, PainkillersCardController.Identifier);

            GoToStartOfTurn(tempest);

            PutInHand(tempest, "LightningSlash");
            PlayCardFromHand(tempest, "LightningSlash");

            // Assert
            QuickHPCheck(0, -5); // Tempest lost 0 HP for discarding Painkillers, -5 HP on MDP from Lightning Slash
            AssertNumberOfCardsNextToCard(tempest.CharacterCard, 0); // Painkillers not next to Tempest

            AssertTriggersWhere(trigger => !trigger.CardSource.Card.Identifier.Equals(PainkillersCardController.Identifier));
        }

        [Test]
        public void TestUnstableSerumDestroyOngoing()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                UnstableSerumCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            PlayCard(baron, "BacklashField");

            DealDamage(baron, tempest, 4, DamageType.Projectile);
            QuickHPStorage(tempest);

            // Act
            DecisionSelectCards = new [] { tempest.CharacterCard, GetCardInPlay("BacklashField") };

            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, UnstableSerumCardController.Identifier);
            GoToEndOfTurn(DocHavoc);

            // Assert
            QuickHPCheck(2);
            Assert.IsNull(FindCardInPlay(UnstableSerumCardController.Identifier));
            Assert.IsNull(FindCardInPlay("BacklashField"));
        }

        [Test]
        public void TestUnstableSerumSkipDestroyOngoing()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                UnstableSerumCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            PlayCard(baron, "BacklashField");

            DealDamage(baron, tempest, 4, DamageType.Projectile);
            QuickHPStorage(tempest);

            // Act
            DecisionSelectCards = new[] { tempest.CharacterCard, null };

            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, UnstableSerumCardController.Identifier);
            GoToEndOfTurn(DocHavoc);

            // Assert
            QuickHPCheck(2);
            Assert.NotNull(FindCardInPlay(UnstableSerumCardController.Identifier));
            Assert.NotNull(FindCardInPlay("BacklashField"));
        }

        [Test]
        public void TestImprovisedMineDestroyDrawnVillainCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            // Act
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionSelectCards = new [] { GetCardFromHand(GasMaskCardController.Identifier), mdp };
            //DecisionSelectCards = new Card[] {null};
            DecisionYesNo = true;

            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, ImprovisedMineCardController.Identifier);
            GoToEndOfTurn(DocHavoc);

            PlayCard(baron, "BacklashField");


            // Assert

            // Improvised Mine should be in Doc's trash
            AssertInTrash(DocHavoc, ImprovisedMineCardController.Identifier);

            // Improvised Mine shouldn't be in play
            Assert.AreEqual(0, FindCardsWhere(c => c.Identifier == ImprovisedMineCardController.Identifier && c.IsInPlay).Count());

            // Backlash Field shouldn't be in play
            Assert.AreEqual(0, FindCardsWhere(c => c.Identifier == "BacklashField" && c.IsInPlay).Count());

            // Mobile Defense Platform -2 HP
            QuickHPCheck(-2);
        }

        [Test]
        public void TestImprovisedMineDontDestroyDrawnVillainCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            // Act
            DecisionSelectCard = GetCardFromHand(GasMaskCardController.Identifier);
            DecisionYesNo = false;

            GoToPlayCardPhase(DocHavoc);
            PlayCardFromHand(DocHavoc, ImprovisedMineCardController.Identifier);
            GoToEndOfTurn(DocHavoc);

            PlayCard(baron, "BacklashField");


            // Assert

            // Improvised Mine shouldn't be in Doc's trash
            AssertNotInTrash(DocHavoc, ImprovisedMineCardController.Identifier);

            // Improvised Mine should be in play
            Assert.AreEqual(1, FindCardsWhere(c => c.Identifier == ImprovisedMineCardController.Identifier && c.IsInPlay).Count());

            // Backlash Field should be in play
            Assert.AreEqual(1, FindCardsWhere(c => c.Identifier == "BacklashField" && c.IsInPlay).Count());
        }

        [Test]
        public void TestBrawler()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                BrawlerCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToStartOfTurn(DocHavoc);
            //have mdp deal 3 damage earlier in the turn
            DealDamage(mdp, DocHavoc.CharacterCard, 3, DamageType.Melee);

            QuickHPStorage(DocHavoc.CharacterCard, mdp, bunker.CharacterCard, haka.CharacterCard);
            
            // Act
            DecisionSelectTarget = mdp;

            GoToPlayCardPhase(DocHavoc);
            Card brawler = PlayCardFromHand(DocHavoc, BrawlerCardController.Identifier);
            //One non-hero target deals {DocHavoc} 4 melee damage. Then {DocHavoc} deals that target X melee damage, where X is the amount of damage that target dealt {DocHavoc} this turn.
            UsePower(brawler);
            GoToEndOfTurn(DocHavoc);

            // Assert
            QuickHPCheck(-4, -7, 0, 0);
        }

        
        [Test]
        public void TestCauterizeAcceptChangeToHeal_Hero()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                CauterizeCardController.Identifier, SyringeDartsCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            SetHitPoints(bunker.CharacterCard, 15);
            QuickHPStorage(bunker);

            //DecisionsYesNo = new[] {true, false};
            DecisionYesNo = true;
            DecisionSelectTargets = new[] { bunker.CharacterCard, null };

            // Act
            PlayCardFromHand(DocHavoc, CauterizeCardController.Identifier);
            GoToPlayCardPhase(DocHavoc);
            Card syringe = PlayCardFromHand(DocHavoc, SyringeDartsCardController.Identifier);
            UsePower(syringe);

            // Assert
            QuickHPCheck(2);
        }

        [Test]
        public void TestCauterizeAcceptChangeToHeal_Villain()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                CauterizeCardController.Identifier, SyringeDartsCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 4);

            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTargets = new[] {mdp, null };

            // Act
            PlayCardFromHand(DocHavoc, CauterizeCardController.Identifier);
            GoToPlayCardPhase(DocHavoc);
            Card syringe = PlayCardFromHand(DocHavoc, SyringeDartsCardController.Identifier);
            UsePower(syringe);

            // Assert
            QuickHPCheck(2);
        }

        [Test]
        public void TestCauterizeDeclineChangeToHeal()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(DocHavoc, new List<string>()
            {
                CauterizeCardController.Identifier, SyringeDartsCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            DealDamage(baron, bunker, 5, DamageType.Energy);
            QuickHPStorage(bunker);

            DecisionYesNo = false;
            DecisionSelectTargets = new[] { bunker.CharacterCard, null };

            // Act
            PlayCardFromHand(DocHavoc, CauterizeCardController.Identifier);
            GoToPlayCardPhase(DocHavoc);
            Card syringe = PlayCardFromHand(DocHavoc, SyringeDartsCardController.Identifier);
            UsePower(syringe);

            // Assert
            QuickHPCheck(-2);
        }

    }
}

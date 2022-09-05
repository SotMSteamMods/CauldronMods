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
    public class DocHavocTests : CauldronBaseTest
    {


        [Test]
        public void TestDocHavocLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(doc);
            Assert.IsInstanceOf(typeof(DocHavocCharacterCardController), doc.CharacterCardController);

            Assert.AreEqual(30, doc.CharacterCard.HitPoints);
        }


        [Test]
        public void TestDocHavocInnatePowerSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Setup to reduce variance
            Card cardToPlay = PutInHand("FlameBarrier");

            GoToUsePowerPhase(doc);
            DecisionSelectTarget = ra.CharacterCard;
            DecisionSelectCard = cardToPlay;

            QuickHandStorage(ra);
            QuickHPStorage(ra);

            // Act

            //{DocHavoc} deals 1 hero 3 toxic damage. If that hero took damage this way, they may play a card now.
            UsePower(doc.CharacterCard);

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

            GoToUsePowerPhase(doc);
            DecisionSelectTarget = legacy.CharacterCard;

            QuickHandStorage(legacy);
            QuickHPStorage(legacy);

            // Act
            //{ DocHavoc} deals 1 hero 3 toxic damage. If that hero took damage this way, they may play a card now.
            UsePower(doc.CharacterCard);

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
            SetHitPoints(doc.CharacterCard, 1);
            DealDamage(baron, doc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 18);
            SetHitPoints(haka.CharacterCard, 18);

            Card[] healTargets = new Card[] { legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard };
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(doc);

            // Act
            UseIncapacitatedAbility(doc, 0);


            // Assert
            AssertIncapacitated(doc);
            QuickHPCheck(0, 0, 1, 1, 1);

        }

        [Test]
        public void TestIncapacitateOption1_2Targets()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //incap doc havoc
            SetHitPoints(doc.CharacterCard, 1);
            DealDamage(baron, doc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 18);

            Card[] healTargets = new Card[] { legacy.CharacterCard, ra.CharacterCard, null };
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(doc);

            // Act
            UseIncapacitatedAbility(doc, 0);


            // Assert
            AssertIncapacitated(doc);
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
            SetHitPoints(doc.CharacterCard, 1);
            DealDamage(baron, doc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);
            SetHitPoints(legacy.CharacterCard, 15);

            Card[] healTargets = new Card[] { legacy.CharacterCard, null };
            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(doc);

            // Act
            UseIncapacitatedAbility(doc, 0);


            // Assert
            AssertIncapacitated(doc);
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
            SetHitPoints(doc.CharacterCard, 1);
            DealDamage(baron, doc, 2, DamageType.Melee);

            //give room to gain hp
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);



            QuickHPStorage(baron.CharacterCard, mdp, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            DecisionDoNotSelectCard = SelectionType.GainHP;
            GoToUseIncapacitatedAbilityPhase(doc);

            // Act
            UseIncapacitatedAbility(doc, 0);


            // Assert
            AssertIncapacitated(doc);
            QuickHPCheck(0, 0, 0, 0, 0);

        }

        [Test]
        public void TestIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            SetHitPoints(doc.CharacterCard, 1);
            DealDamage(baron, doc, 2, DamageType.Melee);

            DecisionSelectTarget = legacy.CharacterCard;

            QuickHandStorage(legacy);

            // Act
            UseIncapacitatedAbility(doc, 1);


            // Assert
            AssertIncapacitated(doc);
            QuickHandCheck(1);

        }

        [Test]
        public void TestIncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Legacy", "Ra", "MobileDefensePlatform");
            StartGame();

            SetHitPoints(doc.CharacterCard, 1);
            DealDamage(baron, doc, 2, DamageType.Melee);

            QuickHPStorage(new[] { legacy.CharacterCard, ra.CharacterCard });

            // Act
            GoToUseIncapacitatedAbilityPhase(doc);
            UseIncapacitatedAbility(doc, 2);

            GoToStartOfTurn(env);

            PutIntoPlay("BattalionGunner"); // End of env. each hero target takes 1 damage

            GoToEndOfTurn(env);

            // Assert
            AssertIncapacitated(doc);
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
            SetHitPoints(doc.CharacterCard, 20);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, doc.CharacterCard, haka.CharacterCard);

            // Act
            PutInHand(DocsFlaskCardController.Identifier);
            Card docsFlask = GetCardFromHand(DocsFlaskCardController.Identifier);
            PlayCard(docsFlask);

            DecisionSelectCard = ra.CharacterCard;
            // Procs flask to heal Ra +1 HP
            GoToStartOfTurn(doc);

            QuickHPCheck(0, 0, 1, 0, 0);
            QuickHPUpdate();

            // Now use power portion of Doc's Flask
            GoToUsePowerPhase(doc);
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
            MakeCustomHeroHand(doc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier, RecklessChargeCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            SetHitPoints(baron.CharacterCard, 35);
            SetHitPoints(mdp, 5);
            SetHitPoints(doc.CharacterCard, 20);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, doc.CharacterCard, haka.CharacterCard);
            QuickHandStorage(doc);

            // Act
            Card recklessCharge = GetCardFromHand(RecklessChargeCardController.Identifier);
            DecisionSelectCardToPlay = recklessCharge;

            GoToPlayCardPhase(doc);
            Card fieldDressing = GetCardFromHand(FieldDressingCardController.Identifier);
            PlayCard(fieldDressing);

            // Assert
            QuickHPCheck(0, 0, 1, 1, 1);
            QuickHandCheck(-2);
            AssertInPlayArea(doc, recklessCharge);
        }

        [Test]
        public void TestFieldDressing_NoPlay()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "Megalopolis");

            // We need to make an explicit hand so two FieldDressings aren't in hand so it doesn't play another FieldDressing which will throw off the Asserts
            MakeCustomHeroHand(doc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier, RecklessChargeCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            SetHitPoints(baron.CharacterCard, 35);
            SetHitPoints(mdp, 5);
            SetHitPoints(doc.CharacterCard, 20);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, doc.CharacterCard, haka.CharacterCard);
            QuickHandStorage(doc);

            // Act
            Card recklessCharge = GetCardFromHand(RecklessChargeCardController.Identifier);
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            GoToPlayCardPhase(doc);
            Card fieldDressing = GetCardFromHand(FieldDressingCardController.Identifier);
            PlayCard(fieldDressing);

            // Assert
            QuickHPCheck(0, 0, 1, 1, 1);
            QuickHandCheck(-1);
            AssertInHand(doc, recklessCharge);
        }

        [Test]
        public void TestPhosphorBlast()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Legacy", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card bladeBattalion = PlayCard("BladeBattalion");

            QuickHPStorage(baron.CharacterCard, mdp, bladeBattalion, doc.CharacterCard, legacy.CharacterCard, ra.CharacterCard);


            // Act
            Card phosphorBlast = PutInHand(PhosphorBlastCardController.Identifier);
            GoToPlayCardPhase(doc);

            QuickHPStorage(baron.CharacterCard, mdp, bladeBattalion, doc.CharacterCard, legacy.CharacterCard, ra.CharacterCard);

            PlayCard(phosphorBlast);


            // Assert

            /*
             * Check HP Loss:
             * Baron (0: Immune)
             * MDP (-1: Phosphor Blast)
             * Blade Battalion (-1: Phosphor Blast)
             * Doc Havoc (-1: Phosphor Blast)
             * Legacy (-1: Phosphor Blast)
             * Ra (-1: Phosphor Blast)
             *
             */
            QuickHPCheck(0, -1, -1, -1, -1, -1);

            // 2 status effects in play (MDP, Blade Battalion)
            AssertNumberOfStatusEffectsInPlay(2);

            // Both should be of type CannotGainHPStatusEffect
            Assert.AreEqual(2, this.GameController.StatusEffectControllers.Count(sec => sec.StatusEffect is CannotGainHPStatusEffect));

            // Go back around to Doc Havoc's turn again to ensure effects have expired
            GoToStartOfTurn(doc);

            // Both status effects should have expired
            AssertNumberOfStatusEffectsInPlay(0);

        }

        [Test]
        public void TestRecklessCharge_Triggers()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            QuickHPStorage(doc, ra);

            // Act
            GoToPlayCardPhase(doc);
            Card reckless = PlayCardFromHand(doc, RecklessChargeCardController.Identifier);

            GoToDrawCardPhase(doc);
            GoToEndOfTurn(doc);

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

            GoToDrawCardPhase(doc);
            //should be 3 -> 1 normal, 1 for not playing or using power, 1 from reckless
            AssertPhaseActionCount(new int?(3));

            GoToDrawCardPhase(ra);
            //should only be 2 -> 1 normal, 1 for not playing or using power
            AssertPhaseActionCount(new int?(2));

            QuickHPCheck(-3, -2); // 1 Mystical Defenses dmg + 1 from Reckless Charge for Doc Havoc

        }

        [Test]
        public void TestRecklessCharge_Power()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                FieldDressingCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();


            // Act
            GoToPlayCardPhase(doc);
            Card reckless = PlayCardFromHand(doc, RecklessChargeCardController.Identifier);
            GoToUsePowerPhase(doc);
            UsePower(reckless);
            AssertInTrash(doc, reckless);

        }

        [Test]
        public void TestRapidRegen()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                RapidRegenCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });


            //set initial hp
            SetHitPoints(baron, 20);
            SetHitPoints(doc, 10);
            SetHitPoints(tempest, 10);
            SetHitPoints(haka, 10);

            StartGame();

            // Act
            GoToPlayCardPhase(doc);

            PlayCardFromHand(doc, RapidRegenCardController.Identifier);

            QuickHPStorage(doc, tempest, haka, baron);

            GoToPlayCardPhase(tempest);
            PutInHand("CleansingDownpour");
            Card cleansingDownpour = GetCardFromHand("CleansingDownpour");

            PlayCard(cleansingDownpour);
            UsePower(cleansingDownpour);

            PlayCard(baron, "FleshRepairNanites");

            // Assert
            QuickHPCheck(3, 3, 3, 11); // Heroes: Cleansing Rains +2 (+1 with Rapid Regen), // Baron: Flesh-Repair Nanites +10 (+1 with Rapid Regen)

        }

        [Test]
        public void TestStimShot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                StimShotCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            QuickHandStorage(doc);

            DecisionSelectTurnTaker = tempest.TurnTaker;

            // Act
            GoToPlayCardPhase(doc);
            PlayCardFromHand(doc, StimShotCardController.Identifier);

            // Assert
            QuickHandCheck(0); // No change as Stim Shot allowed Doc Havoc to draw a card
            QuickHPCheck(-1); // Stim Shot allowed Tempest to use power Squall, 1 dmg to MDP
        }

        [Test]
        public void TestSyringeDarts()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                SyringeDartsCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp, tempest.CharacterCard, haka.CharacterCard);

            DecisionSelectTargets = new[] { mdp, tempest.CharacterCard };
            DecisionSelectWordSkip = true;

            // Act
            GoToPlayCardPhase(doc);
            Card syringe = PlayCardFromHand(doc, SyringeDartsCardController.Identifier);
            GoToUsePowerPhase(doc);
            UsePower(syringe);

            GoToEndOfTurn(env);

            // Assert
            QuickHPCheck(-2, -2, 0);
        }

        [Test]
        public void TestSyringeDarts_UpTo()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                SyringeDartsCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp, tempest.CharacterCard, haka.CharacterCard);

            DecisionSelectTargets = new[] { mdp, null };
            DecisionSelectWordSkip = true;

            // Act
            GoToPlayCardPhase(doc);
            Card syringe = PlayCardFromHand(doc, SyringeDartsCardController.Identifier);
            GoToUsePowerPhase(doc);
            UsePower(syringe);

            GoToEndOfTurn(env);

            // Assert
            QuickHPCheck(-2, 0, 0);
        }

        [Test]
        public void TestSearchAndRescue_DocHavoc()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                SearchAndRescueCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            Card reckless = GetCardFromHand(doc, RecklessChargeCardController.Identifier);
            Card brawler = PutOnDeck(doc, GetCard(BrawlerCardController.Identifier));
            Card stimshot = PutOnDeck(doc, GetCard(StimShotCardController.Identifier));
            Card flask = PutOnDeck(doc, GetCard(DocsFlaskCardController.Identifier));

            StartGame();

            QuickHandStorage(doc, tempest, haka);

            DecisionSelectCards = new[]
            {
               reckless,
                null,
                null,
               brawler,
               stimshot,
               flask
            };

            // Act
            GoToPlayCardPhase(doc);
            DecisionYesNo = true;
            PlayCardFromHand(doc, SearchAndRescueCardController.Identifier);

            // Assert

            // DocHavoc started with 4, played Search & Rescue (-1), discarded Reckless Charge (-1), put Docs Flask into hand (+1)
            // Tempest did not discard so Search and Rescue did not proc for him
            // Haka did not discard so Search and Rescue did not proc for him
            QuickHandCheck(-1, 0, 0);

            //check all locations
            AssertInHand(brawler);
            AssertOnBottomOfDeck(stimshot);
            AssertInTrash(flask);
            AssertInTrash(reckless);
            AssertNumberOfCardsInRevealed(doc, 0);

        }

        [Test]
        public void TestSearchAndRescue_Tempest()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(tempest, new[]
            {
                "LightningSlash", "LightningSlash", "LightningSlash", "OtherworldlyResilience"
            });

            Card discard = GetCardFromHand(tempest, "OtherworldlyResilience");
            Card d1 = PutOnDeck(tempest, GetCard("GeneBoundShackles"));
            Card d2 = PutOnDeck(tempest, GetCard("ElectricalStorm"));
            Card d3 = PutOnDeck(tempest, GetCard("FlashFlood"));

            StartGame();

            QuickHandStorage(doc, tempest, haka);

            DecisionSelectCards = new[]
            {
                null,
                discard,
                null,
                d1,
                d2,
                d3
            };

            // Act
            GoToPlayCardPhase(doc);
            DecisionYesNo = true;
            PlayCard(SearchAndRescueCardController.Identifier);

            // Assert

            // DocHavoc started with 4
            // Tempest discard 1, draw 1
            // Haka did not discard so Search and Rescue did not proc for him
            QuickHandCheck(0, 0, 0);

            //check all locations
            AssertInHand(d1);
            AssertOnBottomOfDeck(d2);
            AssertInTrash(d3);
            AssertInTrash(discard);
            AssertNumberOfCardsInRevealed(doc, 0);

        }

        [Test]
        public void TestSearchAndRescue_RevealOptional()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                SearchAndRescueCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            Card reckless = GetCardFromHand(doc, RecklessChargeCardController.Identifier);
            Card brawler = PutOnDeck(doc, GetCard(BrawlerCardController.Identifier));
            Card stimshot = PutOnDeck(doc, GetCard(StimShotCardController.Identifier));
            Card flask = PutOnDeck(doc, GetCard(DocsFlaskCardController.Identifier));

            StartGame();

            QuickHandStorage(doc, tempest, haka);

            DecisionSelectCards = new[]
            {
               reckless,
                null,
                null,
               brawler,
               stimshot,
               flask
            };

            // Act
            GoToPlayCardPhase(doc);
            DecisionYesNo = false;
            PlayCardFromHand(doc, SearchAndRescueCardController.Identifier);

            // Assert

            // DocHavoc started with 4, played Search & Rescue (-1), discarded Reckless Charge (-1)
            // Tempest did not discard so Search and Rescue did not proc for him
            // Haka did not discard so Search and Rescue did not proc for him
            QuickHandCheck(-2, 0, 0);

            //check all locations
            //reckless was discarded but opted to not reveal
            AssertInTrash(reckless);
            AssertInDeck(brawler);
            AssertInDeck(stimshot);
            AssertInDeck(flask);

        }
        [Test]
        public void TestSearchAndRescue_1CardInDeck()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                SearchAndRescueCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            Card reckless = GetCardFromHand(doc, RecklessChargeCardController.Identifier);

            DiscardTopCards(doc, 36);
            Card brawler = PutOnDeck(doc, GetCard(BrawlerCardController.Identifier));

            StartGame();

            QuickHandStorage(doc, tempest, haka);

            DecisionSelectCards = new[]
            {
               reckless,
                null,
                null,
               brawler
            };

            // Act
            GoToPlayCardPhase(doc);
            DecisionYesNo = true;
            PlayCardFromHand(doc, SearchAndRescueCardController.Identifier);

            // Assert

            // DocHavoc started with 4, played Search & Rescue (-1), discarded Reckless Charge (-1), put Docs Flask into hand (+1)
            // Tempest did not discard so Search and Rescue did not proc for him
            // Haka did not discard so Search and Rescue did not proc for him
            QuickHandCheck(-1, 0, 0);

            //check all locations
            AssertInHand(brawler);
            AssertInTrash(reckless);
            AssertNumberOfCardsInDeck(doc, 0);

            AssertNumberOfCardsInRevealed(doc, 0);

        }

        [Test]
        public void TestImmediateEvac()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImmediateEvacCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });


            StartGame();

            SetHitPoints(baron.CharacterCard, 35);

            PutInTrash(doc, new List<Card>() { GetCard(DocsFlaskCardController.Identifier), GetCard(BrawlerCardController.Identifier) });
            PutInTrash(tempest, new List<Card>() { GetCard("ChainLightning"), GetCard("FlashFlood") });
            PutInTrash(haka, new List<Card>() { GetCard("Mere"), GetCard("GroundPound") });

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 5);

            QuickHPStorage(baron.CharacterCard, mdp, doc.CharacterCard, tempest.CharacterCard, haka.CharacterCard);
            QuickHandStorage(doc, tempest, haka);

            DecisionSelectFunctions = new int?[] { 1, 0, 1 };

            DecisionSelectCards = new[] { GetCardFromHand(doc, RecklessChargeCardController.Identifier), GetCardFromTrash(tempest, "FlashFlood"), GetCardFromHand(haka) };

            // Act
            GoToPlayCardPhase(doc);
            PlayCardFromHand(doc, ImmediateEvacCardController.Identifier);

            // Assert
            QuickHPCheck(2, 2, 0, 0, 0); // Villain HP gain check
            QuickHandCheck(0, 1, 1);

        }

        [Test]
        public void TestImmediateEvac_Sentinels()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "TheSentinels", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImmediateEvacCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });


            StartGame();

            SetHitPoints(baron.CharacterCard, 35);

            PutInTrash(doc, new List<Card>() { GetCard(DocsFlaskCardController.Identifier), GetCard(BrawlerCardController.Identifier) });
            PutInTrash(tempest, new List<Card>() { GetCard("ChainLightning"), GetCard("FlashFlood") });
            PutInTrash(haka, new List<Card>() { GetCard("Mere"), GetCard("GroundPound") });
            PutInTrash(sentinels, new List<Card>() { GetCard("HumanShield"), GetCard("PositiveEnergy") });

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 5);

            QuickHPStorage(baron.CharacterCard, mdp, doc.CharacterCard, tempest.CharacterCard, haka.CharacterCard, medico,mainstay, idealist, writhe);
            QuickHandStorage(doc, tempest, haka, sentinels);

            DecisionSelectFunctions = new int?[] { 1, 0, 1, 1};

            Card sentinelsDiscard = GetRandomCardFromHand(sentinels);

            DecisionSelectCards = new[] { GetCardFromHand(doc, RecklessChargeCardController.Identifier), GetCardFromTrash(tempest, "FlashFlood"), GetCardFromHand(haka), sentinelsDiscard };

            // Act
            GoToPlayCardPhase(doc);
            PlayCardFromHand(doc, ImmediateEvacCardController.Identifier);

            // Assert
            QuickHPCheck(2, 2, 0, 0, 0, 0, 0, 0, 0); // Villain HP gain check
            QuickHandCheck(0, 1, 1, 1);

        }

        [Test]
        public void TestPainkillersAcceptSelfDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
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

            GoToPlayCardPhase(doc);
            PlayCardFromHand(doc, PainkillersCardController.Identifier);

            GoToStartOfTurn(tempest);

            Card slash = PutInHand(tempest, "LightningSlash");
            PlayCard(slash);

            // Assert
            QuickHPCheck(-2, -5); // Tempest lost 2 HP for keeping Painkillers, -5 HP on MDP from Lightning Slash
            AssertNumberOfCardsNextToCard(tempest.CharacterCard, 1); // Painkillers next to Tempest

            AssertTriggersWhere(trigger => trigger.CardSource.Card.Identifier.Equals(PainkillersCardController.Identifier));
        }

        [Test]
        public void TestPainkillers_Irreducible()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                PainkillersCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            QuickHPStorage(tempest.CharacterCard, GetCardInPlay("MobileDefensePlatform"));

            // Act
            DecisionNextToCard = tempest.CharacterCard;
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            //add reduce damage trigger to mdp
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(3);
            reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = mdp;
            RunCoroutine(base.GameController.AddStatusEffect(reduceDamageStatusEffect, true, FindCardController(baron.CharacterCard).GetCardSource()));

            GoToPlayCardPhase(doc);
            Card painkillers = PlayCardFromHand(doc, PainkillersCardController.Identifier);
            AssertNextToCard(painkillers, tempest.CharacterCard);

            QuickHPStorage(mdp);
            Card slash = PutInHand(tempest, "LightningSlash");
            PlayCard(slash);

            // Assert
            QuickHPCheck(-5); // should be -5 still because irreducible

        }

        [Test]
        public void TestPainkillersDeclineSelfDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                PainkillersCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            QuickHPStorage(tempest.CharacterCard, GetCardInPlay("MobileDefensePlatform"));
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            // Act
            DecisionNextToCard = tempest.CharacterCard;
            DecisionSelectTarget = mdp;
            DecisionYesNo = false;

            GoToPlayCardPhase(doc);
            PlayCardFromHand(doc, PainkillersCardController.Identifier);

            GoToStartOfTurn(tempest);


            //add reduce damage trigger to mdp
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(3);
            reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = mdp;
            RunCoroutine(base.GameController.AddStatusEffect(reduceDamageStatusEffect, true, FindCardController(baron.CharacterCard).GetCardSource()));


            Card slash = PutInHand(tempest, "LightningSlash");
            PlayCard(slash);

            // Assert
            QuickHPCheck(0, -2); // Tempest lost 0 HP for discarding Painkillers, -2 HP on MDP from Lightning Slash
            AssertNumberOfCardsNextToCard(tempest.CharacterCard, 0); // Painkillers not next to Tempest

            AssertTriggersWhere(trigger => !trigger.CardSource.Card.Identifier.Equals(PainkillersCardController.Identifier));
        }

        [Test]
        public void TestUnstableSerumDestroyOngoing()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                UnstableSerumCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card backlash = PlayCard(baron, "BacklashField");

            DealDamage(baron, tempest, 4, DamageType.Projectile);
            QuickHPStorage(tempest);

            // Act
            DecisionSelectCards = new[] { tempest.CharacterCard, backlash };

            GoToPlayCardPhase(doc);
            Card serum = PlayCardFromHand(doc, UnstableSerumCardController.Identifier);
            GoToEndOfTurn(doc);

            // Assert
            QuickHPCheck(2);
            AssertInTrash(doc, serum);
            AssertInTrash(backlash);
        }

        [Test]
        public void TestUnstableSerumDestroyEnvironment()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                UnstableSerumCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card defenses = PlayCard("MysticalDefenses");

            DealDamage(baron, tempest, 4, DamageType.Projectile);
            QuickHPStorage(tempest);

            // Act
            DecisionSelectCards = new[] { tempest.CharacterCard, defenses };

            GoToPlayCardPhase(doc);
            Card serum = PlayCardFromHand(doc, UnstableSerumCardController.Identifier);
            GoToEndOfTurn(doc);

            // Assert
            QuickHPCheck(2);
            AssertInTrash(doc, serum);
            AssertInTrash(defenses);
        }

        [Test]
        public void TestUnstableSerumSkipDestroyOngoing()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tempest", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                UnstableSerumCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();
            Card backlash = PlayCard(baron, "BacklashField");

            DealDamage(baron, tempest, 4, DamageType.Projectile);
            QuickHPStorage(tempest);

            // Act
            DecisionSelectCards = new[] { tempest.CharacterCard, null };

            GoToPlayCardPhase(doc);
            Card serum = PlayCardFromHand(doc, UnstableSerumCardController.Identifier);
            GoToEndOfTurn(doc);

            // Assert
            QuickHPCheck(2);
            AssertInPlayArea(doc, serum);
            AssertInPlayArea(baron, backlash);

        }

        [Test]
        public void TestImprovisedMinePreventDrawnVillainCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                DocsFlaskCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            // Act
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionSelectCards = new[] { GetCardFromHand(GasMaskCardController.Identifier), mdp };
            DecisionYesNo = true;

            GoToPlayCardPhase(doc);
            PlayCardFromHand(doc, ImprovisedMineCardController.Identifier);
            GoToEndOfTurn(doc);

            PlayCard(baron, "BacklashField");


            // Assert

            // Improvised Mine should be in Doc's trash
            AssertInTrash(doc, ImprovisedMineCardController.Identifier);

            // Improvised Mine shouldn't be in play
            Assert.AreEqual(0, FindCardsWhere(c => c.Identifier == ImprovisedMineCardController.Identifier && c.IsInPlay).Count());

            // Backlash Field shouldn't be in play
            Assert.AreEqual(0, FindCardsWhere(c => c.Identifier == "BacklashField" && c.IsInPlay).Count());

            // Mobile Defense Platform -2 HP
            QuickHPCheck(-2);
        }

        [Test]
        public void TestImprovisedMineDontPreventDrawnVillainCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            // Act
            DecisionSelectCard = GetCardFromHand(GasMaskCardController.Identifier);
            DecisionYesNo = false;

            GoToPlayCardPhase(doc);
            Card mine = PlayCardFromHand(doc, ImprovisedMineCardController.Identifier);
            GoToEndOfTurn(doc);

            PlayCard(baron, "BacklashField");


            // Assert

            // Improvised Mine should be in play
            AssertInPlayArea(doc, mine);
            // Backlash Field should be in play
            Assert.AreEqual(1, FindCardsWhere(c => c.Identifier == "BacklashField" && c.IsInPlay).Count());
        }

        [Test]
        public void TestImprovisedMineCantPreventDrawnVillainCard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            // Act
            DecisionSelectCard = GetCardFromHand(GasMaskCardController.Identifier);
            DecisionYesNo = true;



            GoToPlayCardPhase(doc);
            Card mine = PlayCardFromHand(doc, ImprovisedMineCardController.Identifier);

            //make mine indestructible
            MakeIndestructibleStatusEffect makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
            makeIndestructibleStatusEffect.CardsToMakeIndestructible.IsSpecificCard = mine;
            RunCoroutine(base.GameController.AddStatusEffect(makeIndestructibleStatusEffect, true, FindCardController(doc.CharacterCard).GetCardSource()));

            GoToEndOfTurn(doc);

            //will try to destroy itself but will not work because it is indestructible, thus not triggering the rest of the effects
            PlayCard(baron, "BacklashField");


            // Assert

            // Improvised Mine should be in play
            AssertInPlayArea(doc, mine);
            // Backlash Field should be in play
            Assert.AreEqual(1, FindCardsWhere(c => c.Identifier == "BacklashField" && c.IsInPlay).Count());
        }

        [Test]
        public void TestImprovisedMineCanDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            // Act
            DecisionSelectCard = GetCardFromHand(GasMaskCardController.Identifier);
            DecisionYesNo = true;



            GoToPlayCardPhase(doc);
            Card mine = PlayCardFromHand(doc, ImprovisedMineCardController.Identifier);


            // Assert

            // Improvised Mine should be in play
            AssertInPlayArea(doc, mine);

        }

        [Test]
        public void TestImprovisedMineNoDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, BrawlerCardController.Identifier
            });

            StartGame();

            // Act

            GoToPlayCardPhase(doc);
            Card mine = PlayCardFromHand(doc, ImprovisedMineCardController.Identifier);


            AssertInTrash(doc, mine);

        }

        [Test]
        public void TestImprovisedMineNoCardsInHand()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                ImprovisedMineCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, BrawlerCardController.Identifier
            });

            StartGame();

            // Act

            GoToPlayCardPhase(doc);
            DiscardAllCards(doc);
            Card mine = PlayCard(doc, ImprovisedMineCardController.Identifier);


            AssertInTrash(doc, mine);

        }

        [Test]
        public void TestBrawler()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                BrawlerCardController.Identifier, RecklessChargeCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToStartOfTurn(doc);
            //have mdp deal 3 damage earlier in the turn
            DealDamage(mdp, doc.CharacterCard, 3, DamageType.Melee);

            QuickHPStorage(doc.CharacterCard, mdp, bunker.CharacterCard, haka.CharacterCard);

            // Act
            DecisionSelectTarget = mdp;

            GoToPlayCardPhase(doc);
            Card brawler = PlayCardFromHand(doc, BrawlerCardController.Identifier);
            //One non-hero target deals {DocHavoc} 4 melee damage. Then {DocHavoc} deals that target X melee damage, where X is the amount of damage that target dealt {DocHavoc} this turn.
            UsePower(brawler);
            GoToEndOfTurn(doc);

            // Assert
            QuickHPCheck(-4, -7, 0, 0);
        }


        [Test]
        public void TestCauterizeAcceptChangeToHeal_Hero()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
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
            PlayCardFromHand(doc, CauterizeCardController.Identifier);
            GoToPlayCardPhase(doc);
            Card syringe = PlayCardFromHand(doc, SyringeDartsCardController.Identifier);
            UsePower(syringe);

            // Assert
            QuickHPCheck(2);
        }

        [Test]
        public void TestCauterizeAcceptChangeToHeal_Villain()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Haka", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
            {
                CauterizeCardController.Identifier, SyringeDartsCardController.Identifier,
                RecklessChargeCardController.Identifier, GasMaskCardController.Identifier
            });

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 4);

            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTargets = new[] { mdp, null };

            // Act
            PlayCardFromHand(doc, CauterizeCardController.Identifier);
            GoToPlayCardPhase(doc);
            Card syringe = PlayCardFromHand(doc, SyringeDartsCardController.Identifier);
            UsePower(syringe);

            // Assert
            QuickHPCheck(2);
        }

        [Test]
        public void TestCauterizeDeclineChangeToHeal()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");

            MakeCustomHeroHand(doc, new List<string>()
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
            PlayCardFromHand(doc, CauterizeCardController.Identifier);
            GoToPlayCardPhase(doc);
            Card syringe = PlayCardFromHand(doc, SyringeDartsCardController.Identifier);
            UsePower(syringe);

            // Assert
            QuickHPCheck(-2);
        }
        [Test]
        public void TestCauterizeAskOnlyOncePerDamageAccept()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Legacy", "RuinsOfAtlantis");
            StartGame();

            PlayCard("InspiringPresence");
            SetHitPoints(legacy, 10);
            PlayCard("Cauterize");
            DecisionYesNo = true;

            QuickHPStorage(legacy);
            AssertMaxNumberOfDecisions(1);
            DealDamage(doc, legacy, 4, DamageType.Melee);
            QuickHPCheck(5);
        }
        [Test]
        public void TestCauterizeAskOnlyOncePerDamageDecline()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "Legacy", "RuinsOfAtlantis");
            StartGame();

            PlayCard("InspiringPresence");
            SetHitPoints(legacy, 10);
            PlayCard("Cauterize");
            DecisionYesNo = false;

            QuickHPStorage(legacy);
            AssertMaxNumberOfDecisions(1);
            DealDamage(doc, legacy, 4, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_OneShot_IsOneShot([Values("SearchAndRescue", "FieldDressing", "StimShot", "ImmediateEvac", "PhosphorBlast")] string oneshot)
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");
            StartGame();


            GoToPlayCardPhase(doc);

            Card card = PlayCard(oneshot);
            AssertInTrash(card);
            AssertCardHasKeyword(card, "one-shot", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Ongoing_IsOngoing([Values("Painkillers", "RapidRegen", "Brawler", "RecklessCharge", "Cauterize")] string ongoing)
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");
            StartGame();

            GoToPlayCardPhase(doc);

            Card card = PlayCard(ongoing);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "ongoing", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Equipment_IsEquipment([Values("SyringeDarts", "UnstableSerum", "ImprovisedMine", "GasMask", "DocsFlask")] string equipment)
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");
            StartGame();

            //put flask in hand so that mine is not auto destroyed
            PutInHand("DocsFlask");

            GoToPlayCardPhase(doc);

            Card card = PlayCard(equipment);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "equipment", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Limited_IsLimited([Values("SyringeDarts", "RapidRegen", "RecklessCharge", "Cauterize", "GasMask", "DocsFlask")] string limited)
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");
            StartGame();

            GoToPlayCardPhase(doc);

            Card card = PlayCard(limited);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "limited", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Relic_IsRelic([Values("DocsFlask")] string relic)
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Bunker", "RuinsOfAtlantis");
            StartGame();

            GoToPlayCardPhase(doc);

            Card card = PlayCard(relic);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "relic", false);
        }

        [Test]
        public void TestGasMask()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Tachyon", "InsulaPrimalis");
            PutInHand(GasMaskCardController.Identifier);

            StartGame();

            SetHitPoints(baron.CharacterCard, 30);
            SetHitPoints(doc.CharacterCard, 20);
            SetHitPoints(tachyon.CharacterCard, 20);

            Card volcanicEruption = GetCard("ObsidianField");
            PlayCard(volcanicEruption);

            // Act
            GoToPlayCardPhase(doc);

            Card gasMask = GetCardFromHand(GasMaskCardController.Identifier);
            PlayCard(gasMask);
            AssertInPlayArea(doc, gasMask);

            Card blinding = GetCard("BlindingSpeed");
            PutInHand(tachyon, blinding);

            QuickHPStorage(doc, baron, tachyon);

            DecisionSelectCard = volcanicEruption;
            PlayCardFromHand(tachyon, "BlindingSpeed");

            // Assert
            QuickHPCheck(2, 0, 0);
        }


    }
}

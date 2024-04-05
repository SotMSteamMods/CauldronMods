using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.MagnificentMara;

namespace CauldronTests
{
    [TestFixture()]
    public class MagnificentMaraTests : CauldronBaseTest
    {
        #region MaraHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(mara.CharacterCard, 1);
            DealDamage(villain, mara, 2, DamageType.Melee);
        }

        protected DamageType DTM => DamageType.Melee;

        protected Card MDP  { get { return FindCardInPlay("MobileDefensePlatform"); } }
        #endregion maraHelperFunctions

        [Test]
        public void TestMaraLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mara);
            Assert.IsInstanceOf(typeof(MagnificentMaraCharacterCardController), mara.CharacterCardController);

            Assert.AreEqual(27, mara.CharacterCard.HitPoints);
        }
        [Test]
        public void TestMaraPower()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            PlayCard("SurgeOfStrength"); //for distinguishing damage source
            PlayCard("HeavyPlating"); //for testing it doesn't need to do the first damage

            Card mdp = MDP;

            DecisionSelectCards = new Card[] { legacy.CharacterCard, mdp, bunker.CharacterCard, mdp };
            QuickHPStorage(legacy.CharacterCard, mdp, bunker.CharacterCard);

            UsePower(mara);
            QuickHPCheck(-1, -2, 0);

            UsePower(mara);
            QuickHPCheck(0, -1, 0);
        }
        [Test]
        public void TestMaraPowerFirstDamageKills()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            SetHitPoints(legacy, 1);

            QuickHPStorage(MDP);
            DecisionSelectCards = new Card[] { legacy.CharacterCard, MDP };

            UsePower(mara);

            AssertIncapacitated(legacy);
            QuickHPCheck(0);
        }
        [Test]
        public void TestMaraIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();
            SetupIncap(baron);

            Card surge = PutInTrash("SurgeOfStrength");
            UseIncapacitatedAbility(mara, 0);
            AssertOnTopOfDeck(surge);
        }
        [Test]
        public void TestMaraIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();
            SetupIncap(baron);

            Card lff = PlayCard("LivingForceField");
            UseIncapacitatedAbility(mara, 1);
            AssertInTrash(lff);
        }
        [Test]
        public void TestMaraIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "DokThorathCapital");

            StartGame();
            SetupIncap(baron);

            QuickHPStorage(legacy, bunker, scholar);
            Card fighter = PlayCard("FreedomFighters");
            DecisionSelectCard = legacy.CharacterCard;
            UseIncapacitatedAbility(mara, 2);
            DealDamage(fighter, legacy, 2, DTM);
            DealDamage(fighter, bunker, 2, DTM);
            DealDamage(bunker, legacy, 2, DTM);
            QuickHPCheck(-2, -2, 0);

            GoToStartOfTurn(mara);

            DealDamage(fighter, legacy, 2, DTM);
            QuickHPCheck(-2, 0, 0);
            
        }
        [Test]
        public void TestAbracadabraBasic()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card plating = PutInHand("HeavyPlating");

            DecisionYesNo = true;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = plating;

            DestroyCard(surge);

            AssertInTrash(abra);
            AssertInHand(surge);
            AssertIsInPlay(plating);
        }
        [Test]
        public void TestAbracadabraCanReplaySameCard()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card plating = PlayCard("HeavyPlating");

            DecisionYesNo = true;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = plating;

            DestroyCard(plating);
            AssertInTrash(abra);
            AssertIsInPlay(plating);
        }
        [Test]
        public void TestAbracadabraOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card plating = PlayCard("HeavyPlating");

            DecisionYesNo = false;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = plating;

            DestroyCard(plating);

            AssertInTrash(plating);
            AssertIsInPlay(abra);
        }
        [Test]
        public void TestAbracadabraNoTriggerOnSelf()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card kalpak = PlayCard("KalpakOfMysteries");

            AssertNoDecision();
            DestroyCard(kalpak);
            AssertIsInPlay(abra);
        }
        [Test]
        public void TestConvincingDoubleBasic()
        {
            //There are a million things that could go wrong with Convincing Double. 
            //For now I'm just going to make a very basic test to show that the fundamental
            //functionality is there.

            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();
            Card thokk = PutInHand("Thokk");
            Card external = PutInHand("ExternalCombustion");
            Card transmutive = PutInHand("TransmutiveRecovery");

            //needs extra one-shots in hand so we actually make decisions
            PutInHand("BolsterAllies");
            PutInHand("KnowWhenToTurnLoose");
            PutInHand("AdhesiveFoamGrenade");

            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, bunker.TurnTaker, scholar.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { thokk, GetCardInPlay("MobileDefensePlatform"), external, transmutive };
            DecisionAutoDecideIfAble = true;
            SetHitPoints(new TurnTakerController[] { legacy, bunker, scholar }, 15);
            
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);
            AssertNotDamageSource(legacy.CharacterCard);

            for(int i = 0; i < 3; i++)
            {
                DecisionSelectTurnTakersIndex = i;
                PlayCard("ConvincingDouble");
                //first, Legacy hands Thokk to Bunker
                //then Bunker hands External Combustion to Scholar
                //finally Scholar gives Transmutive Recovery to Legacy
            }

            //each one lost a card, Legacy drew 2 from Recovery and Bunker drew 1 from Thokk
            QuickHandCheck(1, 0, -1);

            //Legacy gained two, Bunker gained nothing, Scholar hit himself for two
            QuickHPCheck(2, 0, -2);
        }
        [Test]
        public void TestConvincingDoubleWithSentinels()
        {
            //just to make sure it doesn't break *too* badly.

            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card thokk = PutInHand("Thokk");
            Card transmutive = PutInHand("TransmutiveRecovery");
            Card dichotomy = PutInHand("HorrifyingDichotomy");

            //needs extra one-shots in hand so we actually make decisions
            PutInHand("BolsterAllies");
            PutInHand("KnowWhenToTurnLoose");
            PutInHand("SecondChance");

            //make sure we can distinguish who's playing what
            PlayCard("SurgeOfStrength");

            SetHitPoints(new Card[] { mainstay, writhe, medico, idealist, legacy.CharacterCard, scholar.CharacterCard }, 8);

            DecisionSelectCards = new Card[] { thokk, idealist, mdp };
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, sentinels.TurnTaker, scholar.TurnTaker, sentinels.TurnTaker, sentinels.TurnTaker, legacy.TurnTaker };

            QuickHPStorage(writhe, mdp, scholar.CharacterCard);
            QuickHandStorage(legacy, sentinels, scholar);

            PlayCard("ConvincingDouble");

            //Legacy hands Sentinels Thokk, they pick Idealist to take it
            QuickHPCheck(0, -3, 0);

            QuickHandCheck(-1, 1, 0);

            DecisionSelectCards = new Card[] { transmutive, writhe };
            DecisionSelectCardsIndex = 0;

            PlayCard("ConvincingDouble");

            //Scholar hands Sentinels Transmutive Recovery, they pick Writhe to take it
            QuickHandCheck(0, 2, -1);
            QuickHPCheck(2, 0, 0); 
            //this ABSOLUTELY DOES WORK GOT DANG

            DecisionSelectCards = new Card[] { dichotomy, writhe, mdp };
            DecisionSelectCardsIndex = 0;
            AssertNotDamageSource(writhe);
            AssertNotDamageSource(medico);

            PlayCard("ConvincingDouble");

            //Sentinels hand Legacy Horrifying Dichotomy, who ought to do both of the damages.
            QuickHandCheck(0, -1, 0);
            QuickHPCheck(-4, -4, 0);

            //Assert.Ignore("Pass-to-Sentinels doesn't work, not sure it's fixable. Pass-from-sentinels doesn't, and I'm not sure why.");
        }
        [Test]
        public void TestConvincingDoubleOneOptionMessages()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Megalopolis");
            StartGame();

            DiscardAllCards(mara);
            DiscardAllCards(legacy);

            PutInHand("FlyingSmash");

            AssertNextMessages("Legacy is the only player who can pass a one-shot.",
                                "Flying Smash is the only one-shot in Legacy's hand.",
                                "Magnificent Mara is the only player who can be passed Flying Smash.",
                                "Magnificent Mara puts Flying Smash into play!");
            PlayCard("ConvincingDouble");
        }
        [Test]
        public void TestDowsingCrystalSimple()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            Card crystal = PlayCard("DowsingCrystal");
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(1);
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(2);
            DecisionYesNo = true;

            QuickHPStorage(baron);
            PlayCard("BladeBattalion");

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPCheck(-6);
            AssertInTrash(crystal);
        }
        [Test]
        public void TestDowsingCrystalSaveReload()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            Card crystal = PlayCard("DowsingCrystal");
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(1);
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(2);
            DecisionYesNo = true;
            SaveAndLoad();

            QuickHPStorage(baron);
            PlayCard("BladeBattalion");

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPCheck(-6);
            AssertInTrash("DowsingCrystal");
        }
        [Test]
        public void TestDowsingCrystalOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            PlayCard("DowsingCrystal");
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(1);
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(2);
            DecisionYesNo = false;

            QuickHPStorage(baron);

            PlayCard("BladeBattalion");

            AssertNumberOfStatusEffectsInPlay(2);
            QuickHPCheck(0);
        }
        [Test]
        public void TestDowsingCrystalMaySkipFirstCardPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");
            StartGame();
            Card mdp = DestroyCard("MobileDefensePlatform");
            Card crystal = PlayCard("DowsingCrystal");

            Card batt = GetCard("BladeBattalion");
            Card lash = GetCard("BacklashField");
            DecisionsYesNo = new bool[] { false, true, false };
            DecisionSelectCards = new Card[] { legacy.CharacterCard, batt };

            //expected behavior: Backlash Field enters play, we do nothing.
            //Battalion enters play, we get to punch it
            //MDP enters play, no responses
            UsePower("DowsingCrystal");

            QuickHPStorage(baron, legacy);
            PlayCard(lash);
            QuickHPCheck(0, 0);

            PlayCard(batt);
            QuickHPCheck(0, 0);
            Assert.IsTrue(batt.HitPoints < batt.MaximumHitPoints, "The Blade Battalion has not been damaged.");

            AssertNoDecision();
            PlayCard(mdp);
        }
        [Test]
        public void TestDowsingCrystalMaySkipFirstCardPlaySaveAndReload()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");
            StartGame();
            Card mdp = DestroyCard("MobileDefensePlatform");
            Card crystal = PlayCard("DowsingCrystal");

            Card lash = GetCard("BacklashField");
            DecisionsYesNo = new bool[] { false, true, false };

            //expected behavior: Backlash Field enters play, we do nothing.
            //Battalion enters play, we get to punch it
            //MDP enters play, no responses
            UsePower("DowsingCrystal");

            QuickHPStorage(baron, legacy);
            PlayCard(lash);
            QuickHPCheck(0, 0);

            SaveAndLoad();

            Card batt = GetCard("BladeBattalion");
            DecisionSelectCards = new Card[] { legacy.CharacterCard, batt };
            PlayCard(batt);
            QuickHPCheck(0, 0);
            Assert.IsTrue(batt.HitPoints < batt.MaximumHitPoints, "The Blade Battalion has not been damaged.");

            mdp = GetCard("MobileDefensePlatform");
            AssertNoDecision();
            PlayCard(mdp);
        }
        [Test]
        public void TestDowsingCrystalReactionPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Mordengrad");
            StartGame();
            Card crystal = PlayCard("DowsingCrystal");

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card turret = PlayCard("PoweredRemoteTurret");
            Card batt = PutOnDeck("BladeBattalion");

            Card tank = PlayCard("RemoteWalkingTank");
            SetHitPoints(tank, 1);

            UsePower(crystal); //SE1
            UsePower(crystal); //SE2
            UsePower(crystal); //SE3
            DestroyCard(crystal); //to avoid having to decide whether to destroy it for extra damage

            QuickHPStorage(mdp, turret);

            DecisionsYesNo = new bool[] { false, true, true, true, true };
            DecisionSelectCards = new Card[] { mainstay, tank, mainstay, mdp, mainstay, mdp, mainstay, turret, mainstay, turret, mainstay, turret };
            //once we get more than we "should" we will start hitting the turret

            //Expected behavior: 
            //Backlash Field enters play. SE1 is skipped. SE2 is used to destroy the Tank, causing... 
            //Battalion enters play. SE1 is used to hit the MDP. SE2 recognizes it is being used. SE3 is used to hit the MDP.
            //...and we get back to the Backlash Field triggers. SE2 wraps up, SE3 recognizes it has already been used.
            PlayCard("BacklashField");
            QuickHPCheck(-4, 0);

            //all Dowsing Crytal effects should have expired
            var statusEffects = GameController.StatusEffectManager
                                            .StatusEffectControllers
                                            .Where((StatusEffectController sec) => sec.StatusEffect is OnDealDamageStatusEffect oddse && oddse.MethodToExecute == "DowsingCrystalDamageBoostResponse")
                                            .ToList();
            Assert.AreEqual(0, statusEffects.Count());
        }
        [Test]
        public void TestDowsingCrystalPowerModifiersTrack()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Unity", "TheSentinels", "TheScholar", "Mordengrad");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card crystal = PlayCard("DowsingCrystal");

            DecisionSelectTurnTaker = mara.TurnTaker;
            DecisionSelectPower = crystal;
            DecisionYesNo = true;

            QuickHPStorage(baron);

            PlayCard("HastyAugmentation");
            PlayCard("BladeBattalion");
            // -2 from base, -2 from self-boost, -2 from Hasty Augmentation
            QuickHPCheck(-6);
            AssertInTrash(crystal);
        }
        [Test]
        public void TestDowsingCrystalDoesRemoveTarget()
        {
            SetupGameController("TheEnnead", "Cauldron.MagnificentMara", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card firstDemigod = FindCardsWhere((Card c) => c.IsActiveEnneadCharacter && c.IsInPlayAndHasGameText).FirstOrDefault();
            Card crystal = PlayCard("DowsingCrystal");
            UsePower(crystal);
            DestroyCard(crystal);

            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { ra.CharacterCard, firstDemigod };
            SetHitPoints(firstDemigod, 2);
            PlayCard("PoliceBackup");
            AssertFlipped(firstDemigod);
            Assert.IsFalse(firstDemigod.IsTarget);
        }
        [Test]
        public void TestDowsingCrystalDoesRemoveTargetWhenDestroyingSelf()
        {
            SetupGameController("TheEnnead", "Cauldron.MagnificentMara", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card firstDemigod = FindCardsWhere((Card c) => c.IsActiveEnneadCharacter && c.IsInPlayAndHasGameText).FirstOrDefault();
            Card crystal = PlayCard("DowsingCrystal");
            UsePower(crystal);

            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { ra.CharacterCard, firstDemigod };
            SetHitPoints(firstDemigod, 4);
            PlayCard("PoliceBackup");
            AssertFlipped(firstDemigod);
            Assert.IsFalse(firstDemigod.IsTarget);
        }
        [Test]
        public void TestGlimpse()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card glimpse = PutInHand("GlimpseOfThingsToCome");
            Card crystal = PutInHand("DowsingCrystal");

            DecisionSelectCard = crystal;
            Card topOfDeck = GetTopCardOfDeck(baron);
            QuickHandStorage(mara);
            DecisionYesNo = true;
            PlayCard(glimpse);

            AssertOnTopOfDeck(topOfDeck);
            AssertInTrash(glimpse);
            AssertIsInPlay(crystal);
            QuickHandCheck(-1);
            AssertNumberOfCardsInRevealed(baron, 0);
        }
        [Test]
        public void TestHandIsFasterThanTheEye()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card hand = PlayCard("HandIsFasterThanTheEye");
            Card battalion = PlayCard("BladeBattalion");

            GoToStartOfTurn(mara);

            AssertInTrash(battalion);
            AssertInTrash(hand);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeDestroysOnlyOne()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card battalion1 = PlayCard("BladeBattalion");
            Card battalion2 = PlayCard("BladeBattalion");
            Card hand = PlayCard("HandIsFasterThanTheEye");

            GoToStartOfTurn(mara);

            AssertInTrash(battalion1);
            AssertInTrash(hand);
            AssertIsInPlay(battalion2);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeIgnoresCharacterAndRelic()
        {
            SetupGameController("Apostate", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();
            Card fiend = PlayCard("FiendishPugilist");
            Card sword = GetCardInPlay("Condemnation");
            PlayCard("HandIsFasterThanTheEye");

            GoToEndOfTurn(apostate);

            AssertIsInPlay(sword);
            AssertInTrash(fiend);

        }
        [Test]
        public void TestHandIsFasterThanTheEyeGoesInPlayOrder()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card faster = PlayCard("HandIsFasterThanTheEye");
            Card battalion = PlayCard("BladeBattalion");
            Card turret = PlayCard("PoweredRemoteTurret");

            QuickHPStorage(mara, legacy, scholar);

            GoToStartOfTurn(mara);
            AssertInTrash(battalion);
            AssertInTrash(faster);
            AssertIsInPlay(turret);
            //QuickHPCheck(-3, -3, -3);

            GoToPlayCardPhase(baron);
            PlayCard(faster);
            PlayCard(battalion);
            GoToStartOfTurn(mara);

            AssertInTrash(turret);
            AssertIsInPlay(battalion);
        }
        [Test]
        public void TestAbraDestroysBeforeUhYeahImThatGuy()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Guise", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card ip = PlayCard("InspiringPresence");
            Card ring = PlayCard("TheLegacyRing");
            Card uhyeah = PlayCard("UhYeahImThatGuy");
            Card crystal = PutInHand("DowsingCrystal");

            DecisionYesNo = true;
            DecisionSelectTurnTaker = mara.TurnTaker;
            DecisionSelectCards = new List<Card> { abra, crystal };

            DestroyCard(surge);
            AssertInTrash(abra);
            AssertIsInPlay(ip);
            AssertIsInPlay(ring);
            AssertIsInPlay(uhyeah);

            DestroyCard(ring);
            AssertIsInPlay(ip);
            AssertIsInPlay(uhyeah);
            AssertIsInPlay(crystal);
        }
        [Test]
        public void TestUhYeahImThatGuyDestroysBeforeAbracadabra()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Guise", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card ip = PlayCard("InspiringPresence");
            Card ring = PlayCard("TheLegacyRing");
            Card uhyeah = PlayCard("UhYeahImThatGuy");
            Card crystal = PutInHand("DowsingCrystal");
            Card kalpak = PutInHand("KalpakOfMysteries");

            DecisionsYesNo = new List<bool> { false, true, false, true };
            DecisionSelectTurnTaker = mara.TurnTaker;
            DecisionSelectCards = new List<Card> { abra, abra, crystal, kalpak };

            // - Surge of Strength gets destroyed
            // - Mara declines to save it using Abracadabra! 
            // - Guise chooses to save it using Uh, Yeah, I'm That Guy copying Abracadabra
            // - Mara declines to save Uh, Yeah, I'm That Guy
            DestroyCard(surge);
            AssertIsInPlay(abra);
            AssertIsInPlay(ip);
            AssertIsInPlay(ring);
            AssertInTrash(uhyeah);

            DestroyCard(ring);
            AssertIsInPlay(ip);
            AssertIsInPlay(crystal);
            AssertIsInPlay(kalpak);
            AssertInTrash(abra);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeGetsStartOfTurnTrigger()
        {
            SetupGameController("CitizenDawn", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();
            DestroyNonCharacterVillainCards();
            GoToEndOfTurn(scholar);

            Card eclipse = PlayCard("ChannelTheEclipse");
            Card faster = PlayCard("HandIsFasterThanTheEye");

            GoToStartOfTurn(dawn);

            AssertInTrash(eclipse);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeWhenPlayedMidEndPhase()
        {
            SetupGameController("Apostate", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();
            Card spirit = PlayCard("RelicSpirit");
            Card imp = PlayCard("ImpPilferer");
            Card fiend = PlayCard("FiendishPugilist");
            Card sword = GetCardInPlay("Condemnation");

            SetHitPoints(legacy, 10);
            SetHitPoints(sword, 5);
            QuickHPStorage(sword);
            PlayCard("Abracadabra");
            Card iron = PlayCard("FleshToIron");

            Card faster = PutInHand("HandIsFasterThanTheEye");

            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { iron, faster };
            DecisionSelectTurnTaker = mara.TurnTaker;

            GoToEndOfTurn(apostate);
            //Assert.Ignore("There may be a way to manage this, but it's a difficult problem.");

            QuickHPCheck(1);
            AssertIsInPlay(imp);
            AssertInTrash(fiend);

        }
        [Test]
        public void TestHandIsFasterThanTheEyeNotCancelOnDestruction()
        {
            SetupGameController("BiomancerTeam", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card duplex = PutInTrash("Duplexpatriette");

            GoToStartOfTurn(scholar);
            Card faster = PlayCard("HandIsFasterThanTheEye");
            Card rebirth = PlayCard("MassRebirth");

            SetHitPoints(biomancerTeam, 1);
            QuickHPStorage(biomancerTeam);
            GoToStartOfTurn(biomancerTeam);
            AssertInTrash(faster);
            AssertInTrash(rebirth);
            AssertInTrash(duplex);
            QuickHPCheck(10);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeEndOfWrongTurnDestroyTrigger()
        {
            SetupGameController("AkashBhuta", "Cauldron.MagnificentMara", "Unity", "TheWraith", "Megalopolis");
            StartGame();

            GoToStartOfTurn(mara);
            Card brambles = PlayCard("EnsnaringBrambles");
            Card bot = PlayCard("RaptorBot");
            SetHitPoints(brambles, 1);
            DecisionSelectTarget = brambles;
            Card faster = PlayCard("HandIsFasterThanTheEye");

            GoToEndOfTurn(unity);
            AssertIsInPlay(faster);
        }
        [Test]
        public void TestHandisFasterThanTheEyeStatusEffectExpiry()
        {
            SetupGameController("AkashBhuta", "Cauldron.MagnificentMara", "Unity", "TheWraith", "Megalopolis");
            StartGame();

            Card brambles = PlayCard("EnsnaringBrambles");
            Card bot = PlayCard("RaptorBot");
            GoToStartOfTurn(wraith);

            Card faster = PlayCard("HandIsFasterThanTheEye");

            GoToStartOfTurn(akash);
            AssertIsInPlay(brambles);
            AssertIsInPlay(faster);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeMultipleCopies()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Unity", "TheWraith", "Megalopolis");
            StartGame();

            Card battalion = PlayCard("BladeBattalion");
            Card faster1 = PlayCard("HandIsFasterThanTheEye");
            Card faster2 = PlayCard("HandIsFasterThanTheEye");

            GoToStartOfTurn(mara);
            AssertInTrash(faster1);
            AssertIsInPlay(faster2);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeWithYeahImThatGuy()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Unity", "Guise", "Megalopolis");
            StartGame();

            Card battalion = PlayCard("BladeBattalion");
            Card thatguy = PlayCard("UhYeahImThatGuy");
            Card faster = PlayCard("HandIsFasterThanTheEye");

            GoToStartOfTurn(mara);
            AssertInTrash(thatguy);
            AssertIsInPlay(faster);
        }
        [Test]
        public void TestImprobableEscapeCardDraw()
        {
            SetupGameController("Apostate", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card escape = PlayCard("ImprobableEscape");
            Card enhancement = PlayCard("MysticalEnhancement");

            QuickHandStorage(mara);

            DestroyCard(enhancement);
            QuickHandCheck(1);
            DestroyCard(escape);
            QuickHandCheck(1);
        }
        [Test]
        public void TestImprobableEscapeSurvival()
        {
            SetupGameController("Apostate", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card escape = PlayCard("ImprobableEscape");

            DealDamage(apostate, legacy, 50, DamageType.Melee);

            AssertNotIncapacitatedOrOutOfGame(legacy);
            AssertInTrash(escape);
            Assert.AreEqual(legacy.CharacterCard.HitPoints, 2);
        }
        [Test]
        public void TestKalpakMakeJournal()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");
            StartGame();

            Card kalpak = PlayCard("KalpakOfMysteries");
            UsePower(kalpak);

            PrintJournal();
        }

        [Test]
        public void TestKalpak_DarkMind()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.MagnificentMara", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective", scionIdentifiers: new List<string>() { "DarkMindCharacter" });
            StartGame();


            SwitchBattleZone(mara);
            SwitchBattleZone(legacy);

            AssertBattleZone(mindScion, bzTwo);
            AssertBattleZone(mara, bzTwo);
            AssertBattleZone(legacy, bzTwo);
            Card legacyTop = PutOnDeck("NextEvolution");
            DecisionSelectCard = legacyTop;
            Card kalpak = PlayCard("KalpakOfMysteries");
            UsePower(kalpak);
            AssertUnderCard(mindScion, legacyTop);
        }
        [Test]
        public void TestKalpakDestroysSelf()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DecisionSelectFunction = 0;

            Card kalpak = PlayCard("KalpakOfMysteries");
            UsePower(kalpak);

            AssertInTrash(kalpak);
        }
        [Test]
        public void TestKalpakPlaysNinja()
        {
            SetupGameController("BaronBlade", "Legacy", "Cauldron.MagnificentMara", "TheScholar", "TheTempleOfZhuLong");
            StartGame();

            Card ninja = PlayCard("ShinobiAssassin");
            DestroyCard(ninja);
            AssertOnTopOfDeck(legacy, ninja);

            Card kalpak = PlayCard("KalpakOfMysteries");
            UsePower(kalpak);
            AssertIsInPlay(ninja);
        }
        [Test]
        public void TestLookingForThis()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card surge = PutInTrash("SurgeOfStrength");
            Card lead = PutInTrash("NextEvolution");
            Card ring = PlayCard("TheLegacyRing");

            DecisionSelectTurnTaker = legacy.TurnTaker;

            PlayCard("LookingForThis");
            AssertInHand(ring);
            AssertIsInPlay(surge); //both limited!
        }
        [Test]
        public void TestMesmerPendant()
        {
            Assert.Ignore("Mesmer Pendant is non-functional");
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard(MDP);
            PlayCard("BladeBattalion");
            PlayCard("MesmerPendant");

            GoToStartOfTurn(mara);

        }
        [Test]
        public void TestMesmerPendantNonVillain()
        {
            Assert.Ignore("Mesmer Pendant is non-functional");
            SetupGameController("AkashBhuta", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("LivingRockslide");
            PlayCard("MesmerPendant");

            GoToStartOfTurn(mara);

        }
        [Test]
        public void TestMesmerPendantCryoBot()
        {
            Assert.Ignore("MesmerPendant is non-functional");
            SetupGameController("AkashBhuta", "Cauldron.MagnificentMara", "Legacy", "Unity", "RealmOfDiscord");

            StartGame();
            DestroyNonCharacterVillainCards();

            GoToStartOfTurn(legacy);

            Card bot = PlayCard("CryoBot");
            PlayCard("MesmerPendant");

        }
        [Test]
        public void TestMisdirection()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard(MDP);

            QuickHPStorage(baron);
            PlayCard("SurgeOfStrength");
            PlayCard("Misdirection");
            QuickHPCheck(-5); //1 from mara, 2, from followup +1 for Surge +1 for Nemesis
        }
        [Test]
        public void TestMixItUp()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card surge = PlayCard("SurgeOfStrength");
            PlayCard("FleshToIron"); //need to make a decision
            Card lead = PutOnDeck("LeadFromTheFront");
            Card ring = PutOnDeck("TheLegacyRing");

            DecisionSelectCards = new Card[] { surge, ring };

            PlayCard("MixItUp");
            AssertInTrash(surge, lead);
            AssertIsInPlay(ring);
        }
        [Test]
        public void TestMysticalEnhancementDamageBoost()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DecisionSelectCards = new Card[] { mara.CharacterCard, legacy.CharacterCard, MDP };

            PlayCard("MysticalEnhancement");
            QuickHPStorage(mara.CharacterCard, legacy.CharacterCard, MDP);

            UsePower(mara);

            QuickHPCheck(0, -2, -2);

            DecisionSelectCardsIndex = 1;

            DestroyCard("MysticalEnhancement");

            UsePower(mara);

            QuickHPCheck(0, -1, -1);
        }
        [Test]
        public void TestMysticalEnhancementDamageBoost_DamageEffect()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "VoidGuardMainstay/VoidGuardRoadWarriorMainstay", "TheScholar", "Megalopolis");

            StartGame();

            DecisionSelectCards = new Card[] { voidMainstay.CharacterCard, MDP };

            PlayCard("MysticalEnhancement");
            QuickHPStorage(MDP, voidMainstay.CharacterCard);
            DecisionYesNo = true;
            UsePower(voidMainstay);
            DealDamage(MDP, voidMainstay, 1, DTM);
            QuickHPCheck(-2, -1);
        }
        [Test]
        public void TestMysticalEnhancementDestroyInsteadResponse()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card charge = PlayCard("MotivationalCharge");
            PlayCard("SurgeOfStrength");
            DecisionSelectCard = charge;

            Card enhance = PlayCard("MysticalEnhancement");

            DestroyCard(charge);
            AssertIsInPlay(charge);
            AssertInTrash(enhance);
        }

        [Test]
        public void TestMysticalEnhancementDestroyInsteadResponse_OnCharacter_LessThan0()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DecisionSelectCard = legacy.CharacterCard;

            Card enhance = PlayCard("MysticalEnhancement");

            DealDamage(baron, legacy, 50, DamageType.Infernal);
            AssertIncapacitated(legacy);
            AssertInTrash(enhance);
        }

        [Test]
        public void TestMysticalEnhancementDestroyInsteadResponse_OnCharacter_DestroyedWithEffect()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Tachyon", "Megalopolis");

            StartGame();
            SetHitPoints(legacy, 1);
            DecisionSelectCard = legacy.CharacterCard;

            Card enhance = PlayCard("MysticalEnhancement");

            PlayCard("SuckerPunch");
            AssertNotIncapacitatedOrOutOfGame(legacy);
            AssertInTrash(enhance);
        }
        [Test]
        public void TestPostHypnoticCuePower()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card cue = PlayCard("PostHypnoticCue");

            AssertNumberOfUsablePowers(mara, 1);
            DestroyCard(cue);
            AssertNumberOfUsablePowers(mara, 0);
        }
        [Test]
        public void TestPostHypnoticCueStartOfTurnTrigger()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card cue = PlayCard("PostHypnoticCue");
            DecisionYesNo = true;

            GoToPlayCardPhase(mara);

            AssertInTrash(cue);
            AssertNumberOfUsablePowers(mara, 0);

            PlayCard(cue);
            AssertNoDecision();
            GoToPlayCardPhase(legacy);
        }
        [Test]
        public void TestSmokeAndMirrorsPrevention()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card smoke = PlayCard("SmokeAndMirrors");
            DecisionYesNo = true;
            QuickHPStorage(mara);

            DealDamage(baron, mara, 2, DTM);
            QuickHPCheck(0);
            AssertInTrash(smoke);
        }
        [Test]
        public void TestSmokeAndMirrorsOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card smoke = PlayCard("SmokeAndMirrors");
            DecisionYesNo = false;
            QuickHPStorage(mara);

            DealDamage(baron, mara, 2, DTM);
            QuickHPCheck(-2);
            AssertIsInPlay(smoke);
        }
        [Test]
        public void TestSmokeAndMirrorsMultipleCompetingTriggers()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            Card smoke1 = PlayCard("SmokeAndMirrors");
            Card smoke2 = PlayCard("SmokeAndMirrors");
            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectCard = smoke1;
            QuickHPStorage(mara);

            DealDamage(baron, mara, 2, DTM);
            QuickHPCheck(0);
            AssertInTrash(smoke1);
            AssertIsInPlay(smoke2);
        }
        [Test]
        public void TestSmokeAndMirrorsSpecificMultiCardSetup()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "TheVisionary", "Legacy", "RuinsOfAtlantis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            GoToStartOfTurn(legacy);
            SetHitPoints(legacy, 7);
            Card lev = PlayCard("MassLevitation");
            UsePower(lev);
            Card smoke1 = PlayCard("SmokeAndMirrors");
            Card smoke2 = PlayCard("SmokeAndMirrors");

            PlayCard("TheKraken");

            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectCard = smoke1;
            QuickHPStorage(legacy);

            GoToStartOfTurn(baron);
            AssertInTrash(smoke1);
            AssertIsInPlay(smoke2);
            QuickHPCheck(0);
        }
        [Test]
        public void TestWandOfBanishmentTop()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "VoidGuardMainstay", "TheScholar", "Megalopolis");

            StartGame();

            Card wand = PlayCard("WandOfBanishment");
            Card bike = PlayCard("SweetRhonda");
            DecisionSelectFunction = 0;
            QuickHandStorage(voidMainstay);

            DestroyCard(bike);

            QuickHandCheck(0);
            AssertOnTopOfDeck(bike);
            AssertInTrash(wand);
        }
        [Test]
        public void TestWandOfBanishmentBottom()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "VoidGuardMainstay", "TheScholar", "Megalopolis");

            StartGame();

            Card wand = PlayCard("WandOfBanishment");
            Card bike = PlayCard("SweetRhonda");
            DecisionSelectFunction = 1;
            QuickHandStorage(voidMainstay);

            DestroyCard(bike);

            QuickHandCheck(0);
            AssertOnBottomOfDeck(bike);
            AssertInTrash(wand);
        }
        [Test]
        public void TestWandOfBanishmentSkip()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "VoidGuardMainstay", "TheScholar", "Megalopolis");

            StartGame();

            Card wand = PlayCard("WandOfBanishment");
            Card bike = PlayCard("SweetRhonda");
            DecisionDoNotSelectFunction = true;
            QuickHandStorage(voidMainstay);

            DestroyCard(bike);

            QuickHandCheck(3);
            AssertIsInPlay(wand);
            AssertInTrash(bike);
        }
        [Test]
        public void TestWandOfBanishmentNotOnMaraCard()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "VoidGuardMainstay", "TheScholar", "Megalopolis");

            StartGame();

            Card wand = PlayCard("WandOfBanishment");
            Card crystal = PlayCard("DowsingCrystal");
            DecisionSelectFunction = 1;

            AssertNoDecision();

            DestroyCard(crystal);

            AssertIsInPlay(wand);
            AssertInTrash(crystal);
        }

        [Test]
        public void TestWandOfBanishment_TRexBot()
        {
            SetupGameController(new string[] { "OblivAeon", "Legacy", "Cauldron.MagnificentMara", "Unity", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            IEnumerable<Card> mechGolemsAndEquipment = FindCardsWhere(c => c.Owner == unity.TurnTaker && (IsEquipment(c) || c.IsMechanicalGolem));
            MoveCards(unity, mechGolemsAndEquipment, unity.HeroTurnTaker.Hand);

            Card trex = MoveCard(oblivaeon, "BuildingAKing", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));
            GoToBeforeStartOfTurn(legacy);
            RunActiveTurnPhase();

            List<Card> decisionCards = mechGolemsAndEquipment.ToList();
            decisionCards.Add(null);
            DecisionSelectCards = decisionCards.ToArray();
            DecisionSelectTurnTakers = new TurnTaker[] { unity.TurnTaker, null };

            GoToEndOfTurn(legacy);


            GoToPlayCardPhase(mara);
            Card wandOfBanishment = PlayCard("WandOfBanishment");

            DealDamage(oblivaeon, trex, 20, DamageType.Infernal);

            AssertInTrash(wandOfBanishment);
            AssertOnTopOfDeck(legacy, trex);
        }

        [Test()]
        public void TestBootlegMesmerPendant_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.MagnificentMara", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.FSCContinuanceWanderer", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a non-character target
            //since there are no non-character targets in this battlezone, it should go to the trash
            Card pendant = PlayCard("BootlegMesmerPendant");
            AssertInTrash(pendant);

        }
        [Test]
        public void TestBootlegMesmerPendant()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard(MDP);
            PlayCard("BladeBattalion");
            PlayCard("BootlegMesmerPendant");

            QuickHPStorage(baron);
            GoToStartOfTurn(mara);
            QuickHPCheck(-5);
        }
        [Test]
        public void TestBootlegMesmerPendantOncePer()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard(MDP);
            Card turret = PlayCard("PoweredRemoteTurret");
            PlayCard("BootlegMesmerPendant");

            QuickHPStorage(baron.CharacterCard, turret, mara.CharacterCard, legacy.CharacterCard, scholar.CharacterCard);
            GoToStartOfTurn(mara);
            QuickHPCheck(-2, -2, 0, 0, -2);
        }
        [Test]
        public void TestBootlegMesmerPendantIndirect()
        {
            SetupGameController("Apostate", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            QuickHPStorage(apostate);
            PlayCard("BootlegMesmerPendant");

            GoToStartOfTurn(mara);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestBootlegMesmerPendantSwitchesOngoing()
        {
            SetupGameController("Apostate", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");

            StartGame();

            DestroyNonCharacterVillainCards();

            Card abra = PlayCard("Abracadabra");
            Card apoc = PlayCard("Apocalypse");
            PlayCard("ImpPilferer");
            QuickHPStorage(apostate);
            PlayCard("BootlegMesmerPendant");

            GoToStartOfTurn(mara);
            AssertIsInPlay(abra);
            AssertInTrash(apoc);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeGeneBoundBanshee()
        {
            SetupGameController("GrandWarlordVoss", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card banshee = PlayCard("GeneBoundBanshee");
            Card hiftte = PlayCard("HandIsFasterThanTheEye");
            PlayCard("Fortitude");

            GoToEndOfTurn();
            AssertInTrash(banshee);
            AssertInTrash(hiftte);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeOtherMostCardsInPlay()
        {
            SetupGameController("Ambuscade", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(legacy, 30);
            SetHitPoints(scholar, 29);
            //Card turret = PlayCard("AutomatedTurret");
            Card banshee = PlayCard("CustomHandCannon");
            Card hiftte = PlayCard("HandIsFasterThanTheEye");
            PlayCard("Fortitude");

            GoToEndOfTurn();
            AssertInTrash(banshee);
            AssertInTrash(hiftte);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeOtherTiedDecision()
        {
            SetupGameController("GrandWarlordVoss", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(legacy, 29);
            Card minion = PlayCard("GeneBoundFiresworn");
            Card hiftte = PlayCard("HandIsFasterThanTheEye");
            PlayCard("Fortitude");

            GoToEndOfTurn();
            AssertInTrash(minion);
            AssertInTrash(hiftte);
        }
        [Test]
        public void TestHandIsFasterThanTheEyeNonDamageTurnTakerDecision()
        {
            SetupGameController("MissInformation", "Cauldron.MagnificentMara", "Legacy", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(miss);
            PlayCard("ThreatToThePresident");

            PlayCard("DowsingCrystal");
            PlayCard("Fortitude");
            PlayCard("HandIsFasterThanTheEye");

            GoToEndOfTurn();
        }
    }
}

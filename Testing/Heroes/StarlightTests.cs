using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Starlight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class StarlightTests : BaseTest
    {
        #region StarlightHelperFunctions
        protected HeroTurnTakerController starlight { get { return FindHero("Starlight"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(starlight.CharacterCard, 1);
            DealDamage(villain, starlight, 2, DamageType.Melee);
        }
        #endregion

        [Test()]
        public void TestStarlightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(starlight);
            Assert.IsInstanceOf(typeof(StarlightCharacterCardController), starlight.CharacterCardController);

            Assert.AreEqual(31, starlight.CharacterCard.HitPoints);
        }
        [Test()]
        public void TestStarlightPowerMayDrawCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA");

            DecisionSelectFunction = 0;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(1);
            AssertNumberOfCardsInTrash(starlight, 1);
        }

        [Test()]
        public void TestStarlightPowerMayPlayTrashConstellation()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA");

            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(starlight, 0);
        }
        [Test()]
        public void TestStarlightPowerPlaysOnlyOneConstellation()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA", "AncientConstellationB", "NightloreArmor");

            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            AssertNumberOfCardsInTrash(starlight, 3);
            UsePower(starlight);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(starlight, 2);
        }

        [Test()]
        public void TestStarlightPowerMustDrawWithNoConstellationsInTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("NightloreArmor");

            //if we get a decision, we will try to play from trash
            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(1);
        }
        [Test()]
        public void TestStarlightPowerMustDrawWhenNotAbleToPlayCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("NightloreArmor", "AncientConstellationA");
            PutIntoPlay("HostageSituation");

            //if we get a decision, we will try to play from trash
            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestStarlightPowerMustPlayTrashConstellationWhenNotAbleToDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationB");
            PutIntoPlay("TrafficPileup");

            //if we get a decision we will try to draw
            DecisionSelectFunction = 0;

            AssertNumberOfCardsInTrash(starlight, 1);
            UsePower(starlight);
            AssertNumberOfCardsInTrash(starlight, 0);
            AssertNumberOfCardsInPlay(starlight, 2);
        }
        [Test()]
        public void TestStarlightIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToUseIncapacitatedAbilityPhase(starlight);
            UseIncapacitatedAbility(starlight, 0);

            //as lowest HP target, Mobile Defense Platform should be immune to damage
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            QuickHPCheck(0);

            //But it should not be able to deal damage either.
            QuickHPStorage(haka);
            DealDamage(mdp, haka, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Other targets should be unaffected
            DealDamage(ra, haka, 2, DamageType.Fire);
            QuickHPCheck(-2);

            //Should change which target is affected when lowest HP changes
            PutIntoPlay("BladeBattalion");
            Card battalion = GetCardInPlay("BladeBattalion");
            QuickHPStorage(mdp, battalion);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            DealDamage(haka, battalion, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //Should wear off at start of Starlight's turn
            GoToStartOfTurn(starlight);
            QuickHPStorage(battalion);
            DealDamage(haka, battalion, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
        [Test()]
        public void TestStarlightIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //one hero may use a power - have Haka punch the MDP

            GoToUseIncapacitatedAbilityPhase(starlight);
            QuickHPStorage(mdp);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectTarget = mdp;
            AssertIncapLetsHeroUsePower(starlight, 1, haka);
            QuickHPCheck(-2);

        }
        [Test()]
        public void TestStarlightIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            PutIntoPlay("DecoyProjection");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card decoy = GetCardInPlay("DecoyProjection");

            //give some room for healing
            SetHitPoints(ra, 10);
            SetHitPoints(haka, 10);
            SetHitPoints(mdp, 5);
            SetHitPoints(decoy, 1);

            //should be able to heal both hero characters and non-characters
            DecisionSelectCards = new Card[] { ra.CharacterCard, decoy };
            QuickHPStorage(ra.CharacterCard, decoy);
            UseIncapacitatedAbility(starlight, 2);
            UseIncapacitatedAbility(starlight, 2);
            QuickHPCheck(2, 2);

            //should not be able to heal villain targets - only options are Ra, Haka, Decoy, and Visionary
            DecisionSelectCards = null;
            AssertNumberOfChoicesInNextDecision(4);
            UseIncapacitatedAbility(starlight, 2);
        }


        [Test()]
        public void TestAncientConstellationPlaysNextToTarget()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card consta = GetCard("AncientConstellationA");
            Card constb = GetCard("AncientConstellationB");
            Card constc = GetCard("AncientConstellationC");

            DecisionSelectCards = new Card[] { baron.CharacterCard, haka.CharacterCard, mdp };
            PlayCards(new Card[] { consta, constb, constc });

            //should be able to play constellations next to targets whether hero or villain, character or not
            AssertNextToCard(consta, baron.CharacterCard);
            AssertNextToCard(constb, haka.CharacterCard);
            AssertNextToCard(constc, mdp);
        }

        [Test()]
        public void TestAncientConstellationDestroySelfTrigger()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card consta = GetCard("AncientConstellationA");
            Card constb = GetCard("AncientConstellationB");
            Card constc = GetCard("AncientConstellationC");

            Card decoy = GetCard("DecoyProjection");
            PlayCard(decoy);

            DecisionSelectCards = new Card[] { mdp, haka.CharacterCard, decoy };
            PlayCards(new Card[] { consta, constb, constc });

            //constellations should self-destroy when target by them leaves play
            //whether through destruction-into-trash
            DealDamage(haka, mdp, 12, DamageType.Melee);
            AssertInTrash(consta);
            //character card flipping
            DealDamage(baron, haka, 50, DamageType.Melee);
            AssertInTrash(constb);
            //or simply going somewhere else
            PutInHand(decoy);
            AssertInTrash(constc);
        }
        [Test()]
        public void TestCelestialAuraPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card aura = GetCard("CelestialAura");
            PlayCard(aura);

            DecisionSelectPower = aura;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            QuickHandStorage(starlight);
            UsePower(aura);

            //should deal 1 damage and draw 1 card
            QuickHPCheck(-1);
            QuickHandCheck(1);
        }
        [Test()]
        public void TestCelestialAuraDamageToHeal()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card aura = GetCard("CelestialAura");
            PlayCard(aura);

            //put constellations on things
            Card consta = GetCard("AncientConstellationA");
            Card constb = GetCard("AncientConstellationB");
            Card constc = GetCard("AncientConstellationC");

            DecisionSelectCards = new Card[] { mdp, haka.CharacterCard, ra.CharacterCard };
            PlayCards(new Card[] { consta, constb, constc });

            //room for healing
            SetHitPoints(ra, 10);
            SetHitPoints(mdp, 5);
            SetHitPoints(visionary, 10);

            QuickHPStorage(ra.CharacterCard, mdp, visionary.CharacterCard);

            DealDamage(starlight, ra, 1, DamageType.Radiant);
            DealDamage(starlight, mdp, 1, DamageType.Radiant);
            DealDamage(starlight, visionary, 1, DamageType.Radiant);

            //should heal Ra, but neither the MDP (not hero) nor Visionary (not next to constellation)
            QuickHPCheck(1, -1, -1);

            //should not affect damage from other sources
            QuickHPStorage(ra.CharacterCard, mdp, visionary.CharacterCard);
            DealDamage(haka, ra, 1, DamageType.Radiant);
            DealDamage(haka, mdp, 1, DamageType.Radiant);
            DealDamage(haka, visionary, 1, DamageType.Radiant);
            QuickHPCheck(-1, -1, -1);
        }
        [Test()]
        public void TestEventHorizonNoConstellationsToDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card living = GetCard("LivingForceField");
            PlayCard(living);

            AssertMaxNumberOfDecisions(0);
            PlayCard("EventHorizon");
            AssertIsInPlay(living);
        }
        [Test()]
        public void TestEventHorizonZeroConstellationsDestroyed()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card living = GetCard("LivingForceField");
            PlayCard(living);
            Card constellation = GetCard("AncientConstellationA");
            PlayCard(constellation);

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            AssertMaxNumberOfDecisions(1);

            PlayCard("EventHorizon");
            AssertIsInPlay(living);
            AssertIsInPlay(constellation);
        }
        [Test()]
        public void TestEventHorizonOneConstellationDestroyedAndHitsOngoings()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card living = GetCard("LivingForceField");
            PlayCard(living);
            PlayCards("AncientConstellationA", "AncientConstellationB");

            DecisionSelectCards = new List<Card> { GetCardInPlay("AncientConstellationA"), null, living };

            PlayCard("EventHorizon");
            AssertInTrash(living);
            AssertIsInPlay("AncientConstellationB");
        }
        [Test()]
        public void TestEventHorizonTwoConstellationsDestroyedAndHitsEnvironment()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PlayCards("AncientConstellationA", "AncientConstellationB");
            PlayCards("PoliceBackup", "TargetingInnocents", "TrafficPileup");

            PlayCard("EventHorizon");
            AssertNumberOfCardsInTrash(FindEnvironment(), 2);
            AssertNumberOfCardsInTrash(starlight, 3);

        }
        [Test()]
        public void TestEventHorizonAfterConstellationsNoLongerOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PlayCards("AncientConstellationA", "TaMoko");
            Card tamoko = GetCardInPlay("TaMoko");

            DecisionSelectCard = GetCardInPlay("AncientConstellationA");

            //select first constellation to destroy
            //then we should not get a choice, ta moko is the only ongoing or environment card
            AssertMaxNumberOfDecisions(1);
            PlayCard("EventHorizon");

            //Event Horizon and one Constellation
            AssertNumberOfCardsInTrash(starlight, 2);
            AssertInTrash(tamoko); 
        }

        [Test()]
        public void TestExodusDrawCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card constellation = GetCard("AncientConstellationA");
            PutInHand("AncientConstellationA");

            DecisionSelectFunctionsIndex = 0;
            DecisionSelectCardToPlay = constellation;

            Card exodus = GetCard("Exodus");
            PutInHand(exodus);

            QuickHandStorage(starlight);
            PlayCard(exodus);
            AssertIsInPlay(constellation);
            //-1 for playing Exodus, +1 for drawing, -1 for playing the Constellation
            QuickHandCheck(-1);
        }
        [Test()]
        public void TestExodusSearchesDeck()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card aura = GetCard("CelestialAura");
            Card constB = GetCard("AncientConstellationB");
            PutInHand(aura);
            PutOnDeck(starlight, constB, toBottom: true);

            Card exodus = GetCard("Exodus");
            PutInHand(starlight, exodus);

            DecisionSelectFunction = 1;
            DecisionSelectCards = new List<Card> { constB, baron.CharacterCard, aura };

            QuickShuffleStorage(starlight.TurnTaker.Deck);
            QuickHandStorage(starlight);
            PlayCard(exodus);
            AssertIsInPlay(aura, constB);
            //-1 for playing Exodus, -1 for playing Celestial Aura
            QuickHandCheck(-2);
            //should shuffle deck
            QuickShuffleCheck(1);
        }
        [Test()]
        public void TestExodusSearchesTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();


            Card constE = GetCard("AncientConstellationE");
            IEnumerable<Card> constellations = GameController.FindCardsWhere((Card c) => GameController.DoesCardContainKeyword(c, "constellation"));
            PutInTrash(constellations);

            Card aura = GetCard("CelestialAura");
            Card exodus = GetCard("Exodus");
            PutInHand(starlight, exodus);
            PutInHand(starlight, aura);

            DecisionSelectFunction = 1;
            DecisionSelectCards = new List<Card> { constE, baron.CharacterCard, aura };

            //if we get the chance, look in deck for constellations - would cause test to fail
            DecisionSelectLocation = new LocationChoice(starlight.TurnTaker.Deck);

            QuickShuffleStorage(starlight.TurnTaker.Deck);
            QuickHandStorage(starlight);

            PlayCard(exodus);
            AssertIsInPlay(aura, constE);
            //-1 for playing Exodus, -1 for playing Celestial Aura
            QuickHandCheck(-2);
            //5 constellations and an Exodus
            AssertNumberOfCardsInTrash(starlight, 6);
            //should shuffle even though we didn't look in deck
            QuickShuffleCheck(1);
        }
        [Test()]
        public void TestExodusAlwaysShufflesOnceWhenGivingSearchChoice()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            //set up cards for selections
            Card aura = GetCard("CelestialAura");
            Card halo = GetCard("WarpHalo");
            Card constA = GetCard("AncientConstellationA");
            Card constB = GetCard("AncientConstellationB");
            Card constC = GetCard("AncientConstellationC");
            Card constD = GetCard("AncientConstellationD");
            PutInHand(aura);
            PutInHand(halo);
            PutOnDeck(starlight, constB, toBottom: true);
            PutOnDeck(starlight, constA);
            PutInTrash(constC);
            //PutInTrash(constD);

            QuickShuffleStorage(starlight.TurnTaker.Deck);

            Card exodus = GetCard("Exodus");
            PutInHand(starlight, exodus);

            //set preferred decisions for first play
            DecisionSelectFunction = 1;
            DecisionSelectLocation = new LocationChoice(starlight.TurnTaker.Deck);
            DecisionSelectCards = new List<Card> { constA, baron.CharacterCard, aura };

            PlayCard(exodus);
            AssertIsInPlay(constA, aura);

            QuickShuffleCheck(1);

            //set decisions for second play
            DecisionSelectFunction = 1;
            DecisionSelectLocation = new LocationChoice(starlight.TurnTaker.Trash);
            DecisionSelectCardsIndex = 0;
            //DecisionSelectCards = new List<Card>  { constC, starlight.CharacterCard, halo };
            DecisionSelectCards = new List<Card> { starlight.CharacterCard, halo };

            PlayCard(exodus);
            AssertIsInPlay(constC, halo);

            QuickShuffleCheck(1);
        }
        [Test()]
        public void TestExodusCardPlayIsOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card exodus = GetCard("Exodus");
            PutInHand(exodus);

            DecisionSelectFunction = 0;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            QuickHandStorage(starlight);
            PlayCard(exodus);

            //play Exodus, draw card, play nothing else - should result in:
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(starlight, 1);
            AssertNumberOfCardsInTrash(starlight, 1);

        }
        [Test()]
        public void TestGoldenAstrolabeSelfDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card astrolabe = GetCard("GoldenAstrolabe");
            PlayCard(astrolabe);

            QuickHPStorage(starlight);
            UsePower(astrolabe);
            QuickHPCheck(-2);
        }
        [Test()]
        public void TestGoldenAstrolabeGivesPowerToHeroWithConstellation()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card astrolabe = GetCard("GoldenAstrolabe");
            PlayCard(astrolabe);
            Card constellation = GetCard("AncientConstellationA");
            DecisionSelectCard = haka.CharacterCard;
            PlayCard(constellation);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(astrolabe);
            QuickHPCheck(-2);
            AssertNotUsablePower(haka, haka.CharacterCard);
        }
        [Test()]
        public void TestGoldenAstrolabeGivesOnlyOnePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card astrolabe = GetCard("GoldenAstrolabe");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PlayCard(astrolabe);
            DecisionSelectCards = new List<Card> { haka.CharacterCard, ra.CharacterCard, mdp, mdp };
            PlayCards("AncientConstellationA", "AncientConstellationB");


            DecisionSelectTurnTakers = new List<TurnTaker> { haka.TurnTaker, ra.TurnTaker };


            AssertNextDecisionChoices(included: new List<TurnTaker> { haka.TurnTaker, ra.TurnTaker }, notIncluded: new List<TurnTaker> { starlight.TurnTaker, visionary.TurnTaker });
            QuickHPStorage(mdp);
            UsePower(astrolabe);
            QuickHPCheck(-2);
            AssertNotUsablePower(haka, haka.CharacterCard);
            AssertUsablePower(ra, ra.CharacterCard);
        }
        [Test()]
        public void TestGoldenAstrolabeDoesNotAllowChoosingHeroWithNoPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card astrolabe = GetCard("GoldenAstrolabe");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PlayCard(astrolabe);
            DecisionSelectCards = new List<Card> { haka.CharacterCard, ra.CharacterCard, visionary.CharacterCard, mdp, mdp };
            PlayCards("AncientConstellationA", "AncientConstellationB", "AncientConstellationC");


            DecisionSelectTurnTakers = new List<TurnTaker> { ra.TurnTaker };

            UsePower(haka);
            AssertNotUsablePower(haka, haka.CharacterCard);

            AssertNextDecisionChoices(included: new List<TurnTaker> { visionary.TurnTaker, ra.TurnTaker }, notIncluded: new List<TurnTaker> { starlight.TurnTaker, haka.TurnTaker });
            QuickHPStorage(mdp);
            UsePower(astrolabe);
            QuickHPCheck(-2);
            AssertNotUsablePower(ra, ra.CharacterCard);
        }
        [Test()]
        public void TestGoldenAstrolabeDoesNotShareAcrossMultiCharHeroes()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "TheSentinels", "Megalopolis");
            StartGame();

            Card astrolabe = GetCard("GoldenAstrolabe");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PlayCard(astrolabe);
            DecisionSelectCards = new List<Card> { haka.CharacterCard, mainstay, mdp };
            PlayCards("AncientConstellationA", "AncientConstellationB");

            DecisionSelectTurnTakers = new List<TurnTaker> { sentinels.TurnTaker, sentinels.TurnTaker };
            DecisionSelectPower = idealist;

            QuickHPStorage(mdp);
            UsePower(astrolabe);
            QuickHPCheck(0);
            AssertUsablePower(sentinels, idealist);
            AssertNotUsablePower(sentinels, mainstay);

            //now we will give the Sentinels more constellations and another power and see if Idealist can use it
            DecisionSelectCards = new List<Card> { idealist, writhe, mdp };
            DecisionSelectCardsIndex = 0;
            PlayCards("AncientConstellationC", "AncientConstellationD");

            UsePower(astrolabe);
            QuickHPCheck(-2);
            AssertNotUsablePower(sentinels, idealist);
        }
        //other possible tests: multi-char with power on other card (Kismet's Talisman? Oblivaeon Reward?)
        //multi-char with extra power put on card (Cosmic Weapon) - should not be available unless affecting constellation'd card
        [Test()]
        public void TestWishSimpleSelf()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card constA = GetCard("AncientConstellationA");
            Card aura = GetCard("CelestialAura");
            Card astrolabe = GetCard("GoldenAstrolabe");
            Card armor = GetCard("NightloreArmor");
            Card shield = GetCard("NovaShield");
            Card pillars = GetCard("PillarsOfCreation");

            PutOnDeck(starlight, new List<Card> { constA, aura, astrolabe, armor, shield, pillars });

            DecisionYesNo = true;
            DecisionSelectCards = new List<Card> { pillars, aura, astrolabe, shield, armor };
            AssertOnTopOfDeck(pillars);

            PlayCard("Wish");

            AssertOnTopOfDeck(starlight, constA);
            AssertIsInPlay(pillars);
        }
        [Test()]
        public void TestWishSimpleOther()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            var hakaCards = GetCards("TaMoko", "Mere", "Taiaha", "PunishTheWeak", "EnduringIntercession");
            PutOnDeck(haka, hakaCards);

            DecisionYesNo = true;
            DecisionSelectTurnTaker = haka.TurnTaker;
            AssertNumberOfCardsInPlay(haka, 1);

            PlayCard("Wish");

            AssertNumberOfCardsInPlay(haka, 2);
        }
    }
}
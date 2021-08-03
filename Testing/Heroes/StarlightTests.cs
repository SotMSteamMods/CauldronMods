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
    public class StarlightTests : CauldronBaseTest
    {
        #region StarlightHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(starlight.CharacterCard, 1);
            DealDamage(villain, starlight, 2, DamageType.Melee);
        }

        private void ReviveFromIncap(int revivedHP, Card ritesOfRevival)
        {
            CardController revivalController = base.GameController.FindCardController(ritesOfRevival);
            List<UnincapacitateHeroAction> storedResults = new List<UnincapacitateHeroAction>();
            base.RunCoroutine(base.GameController.UnincapacitateHero(base.GameController.FindCardController(starlight.CharacterCard), revivedHP, 2, storedResults, revivalController.GetCardSource()));
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
        public void TestStarlightDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("constellation", new[]
            {
                "AncientConstellationA",
                "AncientConstellationB",
                "AncientConstellationC",
                "AncientConstellationE",
                "AncientConstellationF",
            });

            AssertHasKeyword("ongoing", new[]
            {
                "AncientConstellationA",
                "AncientConstellationB",
                "AncientConstellationC",
                "AncientConstellationE",
                "AncientConstellationF",
                "PillarsOfCreation",
                "NovaShield",
                "CelestialAura",
                "RetreatIntoTheNebula",
            });

            AssertHasKeyword("equipment", new[]
            {
                "NightloreArmor",
                "WarpHalo",
                "GoldenAstrolabe",
            });

            AssertHasKeyword("limited", new[]
            {
                "NightloreArmor",
                "WarpHalo",
                "NovaShield",
                "CelestialAura",
                "GoldenAstrolabe",
            });

            AssertHasKeyword("one-shot", new[]
            {
                "Redshift",
                "EventHorizon",
                "Exodus",
                "StellarWind",
                "Wish",
            });
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
            Card impendingCasualty = GetCard("ImpendingCasualty");

            GoToUseIncapacitatedAbilityPhase(starlight);
            UseIncapacitatedAbility(starlight, 0);

            //as lowest HP target, Mobile Defense Platform should be immune to damage
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Mobile Defense Platform is also immune to damage from non-target sources
            PlayCard(impendingCasualty);
            DealDamage(impendingCasualty, mdp, 2, DamageType.Energy);
            QuickHPCheck(0);
            DestroyCard(impendingCasualty);

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
        public void TestStarlightIncap1_TiedLowest()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToUseIncapacitatedAbilityPhase(starlight);
            UseIncapacitatedAbility(starlight, 0);

            //MDP and Ra are tied for lowest HP target
            SetHitPoints(mdp, 10);
            SetHitPoints(ra, 10);

            //Choose MDP as lowest HP target, it should be immune to damage
            DecisionsYesNo = new bool[] { true };
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Choose MDP as lowest HP target, it should be unable to deal damage
            base.ResetDecisions();
            DecisionsYesNo = new bool[] { true };
            QuickHPStorage(haka);
            DealDamage(mdp, haka, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Deny MDP as lowest HP target, it should deal damage normally
            base.ResetDecisions();
            DecisionsYesNo = new bool[] { false };
            QuickHPStorage(haka);
            DealDamage(mdp, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Deny MDP as lowest HP target, it should take damage normally
            base.ResetDecisions();
            DecisionsYesNo = new bool[] { false };
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //An unambiguously lowest HP target shouldn't get a choice
            base.ResetDecisions();
            DecisionsYesNo = new bool[] { };
            PutIntoPlay("BladeBattalion");
            Card battalion = GetCardInPlay("BladeBattalion");
            QuickHPStorage(battalion);
            DealDamage(haka, battalion, 2, DamageType.Melee);
            QuickHPCheck(0);
        }
        [Test()]
        public void TestStarlightIncap1_Revived()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheTempleOfZhuLong");
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

            //Revive Starlight
            Card revival = GetCard("RitesOfRevival");
            PlayCard(revival);
            ReviveFromIncap(25, revival);
            AssertHitPoints(starlight.CharacterCard, 25);

            //Incap status effects are cleared when a hero is revived
            //Mobile Defense Platform is no longer immune to damage
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Re-incap Starlight
            SetupIncap(baron);
            AssertIncapacitated(starlight);

            //Mobile Defense Platform still takes damage normally
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
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
        public void TestCelestialAuraPower_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon, ra, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "Starlight" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Starlight";
            RunActiveTurnPhase();

            Card aura = GetCard("CelestialAura");
            PlayCard(aura);

            DecisionSelectPower = aura;
            DecisionSelectTarget = oblivaeon.CharacterCard;

            QuickHPStorage(oblivaeon);
            QuickHandStorage(starlight);
            UsePower(aura);

            //should deal 1 damage and draw 1 card
            QuickHPCheck(-1);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestCelestialAuraPower_Oblivaeon_NightloreCouncilReplacement()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Starlight/NightloreCouncilStarlightCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon.CharacterCard, terra, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            DealDamage(oblivaeon.CharacterCard, asheron, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            DealDamage(oblivaeon.CharacterCard, cryos, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "Starlight" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Starlight";
            RunActiveTurnPhase();

            Card aura = GetCard("CelestialAura");
            PlayCard(aura);

            DecisionSelectPower = aura;
            DecisionSelectTarget = oblivaeon.CharacterCard;

            QuickHPStorage(oblivaeon);
            QuickHandStorage(starlight);
            AssertNoDecision(SelectionType.HeroToDealDamage);

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
        [Test()]
        public void TestGoldenAstrolabeAllowsMultiCharHerosToSelectNonCharacterPower()
        {
            SetupGameController("Kismet", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "TheSentinels", "Megalopolis");
            StartGame();

            //don't want Jinxes messing things up
            DestroyNonCharacterVillainCards();

            Card astrolabe = GetCard("GoldenAstrolabe");
            Card talisman = GetCardInPlay("TheTalisman");
            PlayCard(astrolabe);

            SetHitPoints(new List<TurnTakerController> { starlight, haka, ra, visionary }, 10);

            DecisionSelectCards = new List<Card> { haka.CharacterCard, mainstay };
            PlayCards("AncientConstellationA", "AncientConstellationB");

            DecisionSelectTurnTakers = new List<TurnTaker> { sentinels.TurnTaker, sentinels.TurnTaker };

            DealDamage(mainstay, talisman, 10, DamageType.Melee);

            UsePower(mainstay);

            AssertUsablePower(sentinels, talisman);
            AssertNotUsablePower(sentinels, mainstay);

            DecisionSelectCards = null;

            UsePower(astrolabe);

            AssertNotUsablePower(sentinels, talisman);
        }
        [Test()]
        public void TestGoldenAstrolabeAllowsMultiCharHeroToSelectGrantedPowerButDoesNotShare()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "CaptainCosmic", "TheSentinels", "Megalopolis");
            StartGame();

            Card astrolabe = GetCard("GoldenAstrolabe");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PlayCard(astrolabe);
            UsePower(idealist);
            AssertNotUsablePower(sentinels, idealist);
            DecisionSelectCards = new List<Card> { haka.CharacterCard, mainstay, idealist, mainstay, mdp };

            PlayCards("AncientConstellationA", "AncientConstellationB", "CosmicWeapon");

            AssertUsablePower(sentinels, idealist);

            DecisionSelectTurnTaker = sentinels.TurnTaker;
            DecisionSelectPower = idealist;

            QuickHPStorage(mdp);
            UsePower(astrolabe);
            QuickHPCheck(0);
            AssertUsablePower(sentinels, idealist);
            AssertNotUsablePower(sentinels, mainstay);

            PlayCard("CosmicWeapon");
            AssertUsablePower(sentinels, mainstay);
            UsePower(astrolabe);
            AssertUsablePower(sentinels, idealist);
            AssertNotUsablePower(sentinels, mainstay);
            QuickHPCheck(-3);
        }
        //other possible tests: multi-char with power on other card (Kismet's Talisman? Oblivaeon Reward?)
        //multi-char with extra power put on card (Cosmic Weapon) - should not be available unless affecting constellation'd card
        [Test()]
        public void TestNightloreArmorProtectAlly()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DecisionSelectCard = haka.CharacterCard;
            PlayCards("NightloreArmor", "AncientConstellationA");
            Card constellation = GetCardInPlay("AncientConstellationA");

            DecisionYesNo = true;

            //should be able to prevent the damage
            QuickHPStorage(haka);
            DealDamage(baron, haka, 5, DamageType.Melee);
            QuickHPCheck(0);
            AssertInTrash(constellation);

            //should not prevent a second whack
            QuickHPStorage(haka);
            DealDamage(baron, haka, 5, DamageType.Melee);
            QuickHPCheck(-5);

            //should not need to be on the target protected
            PlayCard("AncientConstellationA");
            QuickHPStorage(ra);
            DealDamage(baron, ra, 5, DamageType.Melee);
            QuickHPCheck(0);

            //should work on non-character targets
            PlayCards("AncientConstellationA", "DecoyProjection");
            Card decoy = GetCardInPlay("DecoyProjection");
            QuickHPStorage(decoy);
            DealDamage(baron, decoy, 2, DamageType.Melee);
            QuickHPCheck(0);

            //should work on friendly fire
            PlayCard("AncientConstellationA");
            DealDamage(haka, decoy, 2, DamageType.Melee);
            QuickHPCheck(0);
        }
        [Test()]
        public void TestNightloreArmorNotProtectSelfOrNonHero()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DecisionSelectCard = haka.CharacterCard;
            PlayCards("NightloreArmor", "AncientConstellationA", "TrafficPileup");
            Card constellation = GetCardInPlay("AncientConstellationA");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card traffic = GetCardInPlay("TrafficPileup");

            DecisionYesNo = true;

            //should not get option to prevent damage to Starlight, the MDP, or the traffic
            QuickHPStorage(starlight.CharacterCard, mdp, traffic, haka.CharacterCard);
            DealDamage(baron, starlight, 5, DamageType.Melee);
            DealDamage(baron, mdp, 5, DamageType.Melee);
            DealDamage(baron, traffic, 5, DamageType.Melee);
            AssertIsInPlay(constellation);
            DealDamage(baron, haka, 5, DamageType.Melee);
            AssertInTrash(constellation);
            QuickHPCheck(-5, -5, -5, 0);
        }
        [Test()]
        public void TestNightloreArmorProtectionOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DecisionSelectCard = haka.CharacterCard;
            PlayCards("NightloreArmor", "AncientConstellationA");
            Card constellation = GetCardInPlay("AncientConstellationA");

            DecisionYesNo = false;
            QuickHPStorage(haka);
            DealDamage(baron, haka, 1, DamageType.Melee);
            QuickHPCheck(-1);
            AssertIsInPlay(constellation);

            DecisionYesNo = true;
            DealDamage(baron, haka, 10, DamageType.Melee);
            QuickHPCheck(0);
            AssertInTrash(constellation);
        }
        [Test()]
        public void TestNightloreArmorDestructionNotOptionalOnceProtecting()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DecisionSelectCard = haka.CharacterCard;
            PlayCards("NightloreArmor", "AncientConstellationA", "AncientConstellationB");
            Card constellation = GetCardInPlay("AncientConstellationA");

            DecisionYesNo = true;
            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            DecisionSelectCard = null;

            QuickHPStorage(haka);
            DealDamage(baron, haka, 5, DamageType.Melee);
            QuickHPCheck(0);
            AssertNumberOfCardsInTrash(starlight, 1);
            //character card, Nightlore Armor, and one Constellation
            AssertNumberOfCardsInPlay(starlight, 3);
        }
        [Test()]
        public void TestNightloreArmorDamageDoesNotPreventIfDestructionPrevented()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "TimeCataclysm");
            StartGame();

            DecisionSelectCard = haka.CharacterCard;
            PlayCards("NightloreArmor", "AncientConstellationA", "FixedPoint");
            Card constellation = GetCardInPlay("AncientConstellationA");

            DecisionYesNo = true;
            QuickHPStorage(haka);
            DealDamage(baron, haka, 5, DamageType.Melee);
            QuickHPCheck(-5);
            AssertIsInPlay(constellation);

            DecisionYesNo = true;
            DealDamage(baron, haka, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }
        [Test()]
        public void TestNovaShieldBasic()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetMobileDefensePlatform().Card;
            PutIntoPlay("NovaShield");

            DecisionSelectCards = new List<Card> { haka.CharacterCard, mdp, baron.CharacterCard, ra.CharacterCard };

            SetHitPoints(starlight, 28);
            QuickHPStorage(starlight.CharacterCard, haka.CharacterCard, mdp, baron.CharacterCard, ra.CharacterCard);

            PlayCard("AncientConstellationA");
            QuickHPCheck(1, -1, 0, 0, 0);

            PlayCard("AncientConstellationB");
            QuickHPCheck(1, 0, -1, 0, 0);

            //healing should still happen if damage can't
            PlayCard("AncientConstellationC");
            QuickHPCheck(1, 0, 0, 0, 0);

            //damage should still happen even if healing can't
            PlayCard("AncientConstellationD");
            QuickHPCheck(0, 0, 0, 0, -1);
        }
        [Test()]
        public void TestNovaShieldHealEvenIfTargetIsDestroyedByEffect()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetMobileDefensePlatform().Card;
            PutIntoPlay("NovaShield");
            DecisionSelectCard = mdp;

            //set it up to die
            SetHitPoints(mdp, 1);
            SetHitPoints(starlight, 15);

            QuickHPStorage(starlight);
            PutIntoPlay("AncientConstellationA");
            AssertInTrash(mdp);
            QuickHPCheck(1);
        }
        [Test()]
        public void TestPillarsOfCreationPlaysFromHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PutInHand("AncientConstellationA");
            PutInHand("AncientConstellationB");
            PutInTrash("WarpHalo");
            PutIntoPlay("PillarsOfCreation");
            QuickHandStorage(starlight);

            GoToStartOfTurn(starlight);

            //with no constellations in trash, shouldn't get to pick
            //and putting it in shouldn't be optional
            AssertNoDecision(SelectionType.SelectFunction);
            DecisionDoNotSelectCard = SelectionType.PutIntoPlay;

            //Pillars of Creation and Starlight's character should be in play, trigger should not have happened yet.
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(starlight, 2);
            GoToPlayCardPhase(starlight);
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(starlight, 3);

            GoToDrawCardPhase(starlight);
            //should not count as card play, so double-draw should be possible
            AssertPhaseActionCount(2);
        }
        [Test()]
        public void TestPillarsOfCreationPlaysFromTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA");
            DiscardAllCards(starlight);
            PutIntoPlay("PillarsOfCreation");

            //needed to avoid skipping play phase
            PutInHand("WarpHalo");

            //given that it should trigger even with play phase skipped...

            int startingCardsInTrash = starlight.TurnTaker.Trash.Cards.Count();

            //with no constellations in hand, shouldn't get to pick
            AssertNoDecision(SelectionType.SelectFunction);
            DecisionDoNotSelectCard = SelectionType.SearchTrash;

            GoToStartOfTurn(starlight);


            //Pillars of Creation and Starlight's character should be in play, trigger should not have happened yet.
            AssertNumberOfCardsInTrash(starlight, startingCardsInTrash);
            AssertNumberOfCardsInPlay(starlight, 2);
            GoToUsePowerPhase(starlight);
            AssertNumberOfCardsInTrash(starlight, startingCardsInTrash - 1);
            AssertNumberOfCardsInPlay(starlight, 3);

            GoToDrawCardPhase(starlight);
            //should not count as card play, so double-draw should be possible
            AssertPhaseActionCount(2);

        }
        [Test()]
        public void TestPillarsOfCreationMayChooseHandOrTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA", "AncientConstellationC");
            PutInHand("AncientConstellationB");
            PutInHand("AncientConstellationD");
            PutIntoPlay("PillarsOfCreation");

            //should have a choice, should not be optional
            //does not seem to be a good way to do that, though...
            //the fact that there's no SelectFunctionDecision in the "only hand" or "only trash" conditions does imply it though
            AssertNextDecisionSelectionType(SelectionType.SelectFunction);
            //hand or trash, no skip option

            int handAndTrash = starlight.NumberOfCardsInHand + starlight.TurnTaker.Trash.Cards.Count();
            AssertNumberOfCardsInPlay(starlight, 2);

            GoToPlayCardPhase(starlight);

            AssertNumberOfCardsInPlay(starlight, 3);

            string failmessage = string.Format("Starlight's hand and trash should have had {0} cards combined, but did not.", handAndTrash - 1);
            Assert.AreEqual(handAndTrash - 1, starlight.NumberOfCardsInHand + starlight.TurnTaker.Trash.Cards.Count(), failmessage);

        }
        [Test()]
        public void TestPillarsOfCreationNoConstellationMessage()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PutIntoPlay("PillarsOfCreation");
            PutOnDeck(starlight, starlight.HeroTurnTaker.Hand.Cards);
            PutInHand("WarpHalo");

            AssertNumberOfCardsInPlay(starlight, 2);
            AssertNextMessage("Starlight had no constellations to put into play from their hand or trash.");
            GoToUsePowerPhase(starlight);
            AssertNumberOfCardsInPlay(starlight, 2);
        }


        [Test()]
        public void TestPillarsOfCreation_Timing_NoCardToPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            var card = PutInTrash("AncientConstellationA");
            PutIntoPlay("PillarsOfCreation");
            PutOnDeck(starlight, starlight.HeroTurnTaker.Hand.Cards);
            GoToEndOfTurn(baron);

            GoToUsePowerPhase(starlight);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestPillarsOfCreation_Timing_CardsCannotBePlayed()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            var card = PutInHand("AncientConstellationA");
            PutInHand("AncientConstellationB");
            PutIntoPlay("PillarsOfCreation");
            PlayCard("HostageSituation");
            GoToEndOfTurn(baron);

            DecisionSelectCards = new[] { card, baron.CharacterCard };

            GoToUsePowerPhase(starlight);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestPillarsOfCreation_Timing_TimeCrawls()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "RealmOfDiscord");
            StartGame();
            var card = PutInHand("AncientConstellationA");
            PutInHand("AncientConstellationB");
            PutIntoPlay("PillarsOfCreation");
            PlayCard("TimeCrawls");
            GoToEndOfTurn(baron);
            
            GoToStartOfTurn(starlight);
            DecisionSelectCards = new[] { card, baron.CharacterCard };

            GoToPlayCardPhase(starlight);
            //To late, a card was played in Play Phase

            QuickHandStorage(starlight);
            GoToUsePowerPhase(starlight);
            RunActiveTurnPhase();
            QuickHandCheck(0);
            
            AssertIsInPlay(card);
        }


        [Test()]
        public void TestPillarsOfCreation_Timing_NeedAWayOutSkipPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Cauldron.Echelon", "Ra", "TheVisionary", "RealmOfDiscord");
            StartGame();
            var card = PutInHand("AncientConstellationA");
            PutInHand("AncientConstellationB");
            PutIntoPlay("PillarsOfCreation");
            PlayCard("NeedAWayOut");
            GoToEndOfTurn(baron);

            DecisionYesNo = true;
            GoToStartOfTurn(starlight);

            DecisionSelectCards = new[] { card, baron.CharacterCard };

            GoToUsePowerPhase(starlight);
            AssertNotInPlay(card);
        }


        [Test()]
        public void TestPillarsOfCreation_Timing_BreakingTheRules()
        {
            SetupGameController("WagerMaster", "Cauldron.Starlight", "Haka", "Ra", "RealmOfDiscord");
            StartGame();
            DecisionAutoDecideIfAble = true;
            DestroyNonCharacterVillainCards();
            //losingtothe odds causes a game over mid test, banish it.
            PutInTrash("LosingToTheOdds", "LosingToTheOdds", "LosingToTheOdds");

            var card = PutInHand("AncientConstellationA");
            PutInHand("AncientConstellationB");
            PutIntoPlay("PillarsOfCreation");

            PlayCard("BreakingTheRules");

            GoToEndOfTurn(wager);

            DecisionSelectCards = new[] { card, wager.CharacterCard };

            GoToEndOfTurn(starlight);
            AssertNotInPlay(card);

            GoToDrawCardPhase(starlight);
            AssertNotInPlay(card);

            GoToUsePowerPhase(starlight);
            AssertNotInPlay(card);

            GoToPlayCardPhase(starlight);
            AssertIsInPlay(card);

            GoToStartOfTurn(starlight);
            AssertIsInPlay(card);

        }


        [Test()]
        public void TestRedshiftDrawsCardAndCanSkipPlays()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DecisionDoNotSelectCard = SelectionType.PlayCard;
            Card redshift = GetCard("Redshift");
            PutInHand(starlight, redshift);

            QuickHandStorage(starlight, haka, ra, visionary);
            AssertNumberOfCardsInTrash(starlight, 0);

            PlayCard(redshift);
            //-1 from card play, +1 from draw
            QuickHandCheckZero();
            AssertNumberOfCardsInTrash(starlight, 1);
        }
        [Test()]
        public void TestRedshiftAllowsAtMostTwoPlays()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card redshift = GetCard("Redshift");
            PutInHand(starlight, redshift);

            var cardsToPlay = new Card[3] { GetCard("TaMoko"), GetCard("FlameBarrier"), GetCard("DecoyProjection") };

            DecisionSelectTurnTakers = new TurnTaker[3] { haka.TurnTaker, ra.TurnTaker, visionary.TurnTaker };
            PutInHand(cardsToPlay);
            DecisionSelectCards = cardsToPlay;

            QuickHandStorage(starlight, haka, ra, visionary);
            PlayCard(redshift);
            QuickHandCheck(0, -1, -1, 0);
            AssertIsInPlay(cardsToPlay[0], cardsToPlay[1]);
            AssertNotInPlay(cardsToPlay[2]);

        }
        [Test()]
        public void TestRedshiftAllowsExactlyOnePlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card redshift = GetCard("Redshift");
            PutInHand(starlight, redshift);

            var cardsInHand = new Card[3] { GetCard("TaMoko"), GetCard("FlameBarrier"), GetCard("DecoyProjection") };
            var cardsToPlay = new Card[2] { cardsInHand[0], null };

            DecisionSelectTurnTakers = new TurnTaker[3] { haka.TurnTaker, ra.TurnTaker, visionary.TurnTaker };
            PutInHand(cardsInHand);
            DecisionSelectCards = cardsToPlay;

            QuickHandStorage(starlight, haka, ra, visionary);
            PlayCard(redshift);
            QuickHandCheck(0, -1, 0, 0);
            AssertNotInPlay(cardsInHand[1]);
        }
        [Test()]
        public void TestRedshiftDoesNotAllowIncapacitatedOrNoPlaysChoice()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card redshift = GetCard("Redshift");
            PutInHand(starlight, redshift);

            DealDamage(baron, visionary, 40, DamageType.Melee);
            DiscardAllCards(ra);

            AssertNextDecisionChoices(new List<TurnTaker> { starlight.TurnTaker, haka.TurnTaker }, new List<TurnTaker> { ra.TurnTaker, visionary.TurnTaker });
            PlayCard(redshift);
        }
        [Test()]
        public void TestRetreatIntoTheNebulaPlaysNormallyWithDefaultStarlight()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PlayCard("RetreatIntoTheNebula");

        }
        [Test()]
        public void TestRetreatIntoTheNebulaReducesDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PlayCard("RetreatIntoTheNebula");
            PlayCard("TaMoko");

            var m = DamageType.Melee;
            QuickHPStorage(starlight, haka);
            DealDamage(baron, starlight, 3, m);
            DealDamage(baron, starlight, 2, m);
            DealDamage(baron, haka, 3, m);
            QuickHPCheck(-1, -2);
        }
        [Test()]
        public void TestRetreatIntoTheNebulaDestroysSelfIfNoConstellations()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card retreat = GetCard("RetreatIntoTheNebula");
            PlayCard(retreat);

            GoToStartOfTurn(starlight);

            AssertInTrash(retreat);
        }
        [Test()]
        public void TestRetreatIntoTheNebulaMayDestroyConstellationToStayInPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card constellation = GetCard("AncientConstellationA");
            Card retreat = GetCard("RetreatIntoTheNebula");
            Card[] cards = new Card[2] { retreat, constellation };
            PlayCards(cards);

            DecisionSelectCard = constellation;
            GoToStartOfTurn(starlight);

            AssertNextDecisionChoices(cards);
            AssertInTrash(constellation);
            AssertIsInPlay(retreat);
        }
        [Test()]
        public void TestRetreatIntoTheNebulaMayDestroySelfEvenIfConstellationExists()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card constellation = GetCard("AncientConstellationA");
            Card retreat = GetCard("RetreatIntoTheNebula");
            Card[] cards = new Card[2] { retreat, constellation };
            PlayCards(cards);

            DecisionSelectCard = retreat;
            GoToStartOfTurn(starlight);

            AssertNextDecisionChoices(cards);
            AssertInTrash(retreat);
            AssertIsInPlay(constellation);
        }
        [Test()]
        public void TestRetreatIntoTheNebulaTriggerHappensBeforePillars()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card retreat = GetCard("RetreatIntoTheNebula");
            Card constellation = GetCard("AncientConstellationA");
            PlayCard(retreat);
            PutInTrash(constellation);
            PlayCard("PillarsOfCreation");

            //so Pillars will definitely play from trash
            PutOnDeck(starlight, starlight.HeroTurnTaker.Hand.Cards);
            PutInHand("WarpHalo");

            //Retreat should destroy itself before Pillars gets a chance to play the constellation
            AssertNextDecisionSelectionType(SelectionType.MoveCardNextToCard);
            GoToPlayCardPhase(starlight);
            AssertIsInPlay(constellation);
            AssertInTrash(retreat);

            DecisionSelectCards = new List<Card> { constellation, starlight.CharacterCard };
            PlayCard(retreat);

            //now it should be able to blow up the constellation which immediately gets replayed
            AssertNextDecisionSelectionType(SelectionType.DestroyCard);
            GoToPlayCardPhase(starlight);
            AssertIsInPlay(retreat);
            AssertIsInPlay(constellation);
        }
        [Test()]
        public void TestStellarWindDrawsTwoCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card wind = GetCard("StellarWind");
            PutInHand(wind);

            QuickHandStorage(starlight);
            PlayCard(wind);
            //-1 for card play, +2 for draws
            QuickHandCheck(1);
        }
        [Test()]
        public void TestStellarWindDealsDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card wind = GetCard("StellarWind");
            PutInHand(wind);

            DecisionSelectCard = mdp;
            DecisionSelectTarget = mdp;
            PlayCard("AncientConstellationA");

            QuickHPStorage(mdp);
            ExpectedDecisionChoiceCount = 1;
            AssertDecisionIsOptional(SelectionType.SelectTarget);
            PlayCard(wind);
            QuickHPCheck(-2);
        }
        [Test()]
        public void TestStellarWindCanDamageMultipleTargetsButOnlyNextToConstellation()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card wind = GetCard("StellarWind");
            PutInHand(wind);

            Card[] targets = new Card[3] { mdp, ra.CharacterCard, haka.CharacterCard };
            DecisionSelectCards = targets;
            PlayCards("AncientConstellationA", "AncientConstellationB", "AncientConstellationC");
            DecisionSelectCardsIndex = 0;

            QuickHPStorage(mdp, ra.CharacterCard, haka.CharacterCard, starlight.CharacterCard);
            AssertDecisionIsOptional(SelectionType.SelectTarget);
            AssertNextDecisionChoices(targets, new List<Card> { starlight.CharacterCard, baron.CharacterCard, visionary.CharacterCard });
            PlayCard(wind);
            QuickHPCheck(-2, -2, -2, 0);
        }
        [Test()]
        public void TestStellarWindCanBeSelective()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card wind = GetCard("StellarWind");
            PutInHand(wind);

            Card[] targets = new Card[3] { mdp, ra.CharacterCard, haka.CharacterCard };
            DecisionSelectCards = targets;
            PlayCards("AncientConstellationA", "AncientConstellationB", "AncientConstellationC");
            DecisionSelectCards = new List<Card> { mdp, null };
            DecisionSelectCardsIndex = 0;

            QuickHPStorage(mdp, ra.CharacterCard, haka.CharacterCard, starlight.CharacterCard);
            AssertDecisionIsOptional(SelectionType.SelectTarget);
            AssertNextDecisionChoices(targets, new List<Card> { starlight.CharacterCard, baron.CharacterCard, visionary.CharacterCard });
            PlayCard(wind);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test]
        public void TestStellarWindCanAutodecideWhenShould()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card wind = GetCard("StellarWind");
            PutInHand(wind);

            DecisionSelectCards = new Card[] { mdp, baron.CharacterCard };
            PlayCards("AncientConstellationA", "AncientConstellationB");

            DecisionAutoDecide = SelectionType.SelectTarget;
            PlayCard(wind);

            DecisionSelectCards = null;
            DecisionSelectCard = haka.CharacterCard;
            PlayCard("AncientConstellationC");
            DecisionSelectCard = null;

            PlayCards("CelestialAura", "StellarWind");
        }
        [Test]
        public void TestStellarWindCannotAutodecideWhenShouldnt()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card wind = GetCard("StellarWind");
            PutInHand(wind);

            DecisionSelectCards = new Card[] { mdp, baron.CharacterCard, haka.CharacterCard };
            PlayCards("AncientConstellationA", "AncientConstellationB", "AncientConstellationC");

            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = null;

            PlayCard(wind);

            AssertRecordedDecisionAnswer("4-1", 0, false, autodecided: false);

            PlayCards("CelestialAura", "StellarWind");

            AssertRecordedDecisionAnswer("5-1", 0, false, true);
        }
        [Test()]
        public void TestWarpHaloIncreasesWhenShould()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            var m = DamageType.Melee;

            PlayCard("WarpHalo");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");
            Card traffic = GetCard("TrafficPileup");
            PlayCard(traffic);
            PlayCard(battalion);
            Card[] constellationed = new Card[5] { mdp, baron.CharacterCard, haka.CharacterCard, ra.CharacterCard, traffic };
            DecisionSelectCards = constellationed;
            PlayCards("AncientConstellationA", "AncientConstellationB", "AncientConstellationC", "AncientConstellationD", "AncientConstellationE");

            QuickHPStorage(starlight.CharacterCard, haka.CharacterCard, ra.CharacterCard, mdp, battalion, traffic);
            //Combos that SHOULD increase damage: 
            //Constellation on hero, constellation on villain
            //Constellation on hero, constellation on environment
            //Damage type shouldn't matter
            DealDamage(haka, mdp, 1, m);
            QuickHPCheck(0, 0, 0, -2, 0, 0);
            DealDamage(ra, traffic, 1, DamageType.Fire);
            QuickHPCheck(0, 0, 0, 0, 0, -2);
        }
        [Test()]
        public void TestWarpHaloDoesNotIncreaseWhenShouldnt()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            var m = DamageType.Melee;

            PlayCard("WarpHalo");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");
            Card traffic = GetCard("TrafficPileup");
            PlayCard(traffic);
            PlayCard(battalion);
            Card[] constellationed = new Card[5] { mdp, baron.CharacterCard, haka.CharacterCard, ra.CharacterCard, traffic };
            DecisionSelectCards = constellationed;
            PlayCards("AncientConstellationA", "AncientConstellationB", "AncientConstellationC", "AncientConstellationD", "AncientConstellationE");

            QuickHPStorage(starlight.CharacterCard, haka.CharacterCard, ra.CharacterCard, mdp, battalion, traffic);
            //Combos that SHOULD NOT increase damage: 
            //No constellation on hero -> constellation on villain
            DealDamage(starlight, mdp, 1, m);
            QuickHPCheck(0, 0, 0, -1, 0, 0);
            //No constellation on hero -> constellation on environment
            DealDamage(starlight, traffic, 1, DamageType.Fire);
            QuickHPCheck(0, 0, 0, 0, 0, -1);
            //Constellation on hero -> no constellation on villain
            DealDamage(haka, battalion, 1, m);
            QuickHPCheck(0, 0, 0, 0, -1, 0);
            //Constellation on villain -> constellation on hero
            DealDamage(baron, haka, 1, m);
            QuickHPCheck(0, -1, 0, 0, 0, 0);
            //Constellation on villain -> no constellation on hero
            DealDamage(baron, starlight, 1, m);
            QuickHPCheck(-1, 0, 0, 0, 0, 0);
            //Constellation on villain -> villain, regardless of constellation status
            DealDamage(baron, mdp, 1, m);
            DealDamage(baron, battalion, 1, m);
            QuickHPCheck(0, 0, 0, -1, -1, 0);
            //Constellation on hero -> hero, regardless of constellation status
            DealDamage(haka, starlight, 1, m);
            DealDamage(haka, ra, 1, m);
            QuickHPCheck(-1, 0, -1, 0, 0, 0);

            //Not completely exhaustive, but this is enough for me.
        }

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
            AssertOnBottomOfDeck(starlight, aura);
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
        [Test()]
        public void TestWishCanOnlySelectActiveHeroAndIsOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DealDamage(baron, ra, 50, DamageType.Melee);

            AssertDecisionIsOptional(SelectionType.RevealCardsFromDeck);
            AssertNextDecisionChoices(new List<TurnTaker> { starlight.TurnTaker, visionary.TurnTaker, haka.TurnTaker },
                                        new List<TurnTaker> { baron.TurnTaker, ra.TurnTaker, FindEnvironment().TurnTaker });
            PlayCard("Wish");
        }
        //There are a bunch of other tests that could be done for Wish, but I'll assume Argent Adept has them covered.
        //...well, I did until I wrote custom logic. Less reliable, more consistent with other versions of the effect
        //Especially the order, which with AA code goes "put on very bottom, put on very bottom again, and again..."
        //but in the typical version goes "very bottom, second from bottom, third from bottom..."
        [Test()]
        [Sequential]
        public void TestWishNCardsLeft([Values(4, 3, 2, 1, 0)] int numCardsLeft)
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            DiscardAllCards(starlight);
            PutInTrash(starlight, starlight.TurnTaker.Deck.Cards);

            Card[] cardsToPut = new Card[4] { GetCard("CelestialAura"), GetCard("WarpHalo"), GetCard("NightloreArmor"), GetCard("PillarsOfCreation") };
            for (int i = 0; i < numCardsLeft; i++)
            {
                PutOnDeck(starlight, cardsToPut[i]);
            }

            AssertNumberOfCardsInDeck(starlight, numCardsLeft);

            DecisionSelectTurnTaker = starlight.TurnTaker;
            DecisionSelectCards = cardsToPut;

            //one to pick TurnTaker, one for each card in the deck except the last

            string expectedMessage = null;
            int maxDecisions = numCardsLeft;
            if (numCardsLeft == 0)
            {
                maxDecisions = 1;
                expectedMessage = "No cards were revealed! There is no further effect.";
            }
            else if (numCardsLeft == 1)
            {
                expectedMessage = "Only one card was revealed! It will automatically be put into play.";
            }
            else
            {
                expectedMessage = String.Format("Only {0} cards were revealed!", numCardsLeft);
            }


            PlayCard("Wish");

            if (numCardsLeft == 0)
            {
                AssertNumberOfCardsInPlay(starlight, 1);
            }
            else
            {
                AssertIsInPlay("CelestialAura");
                AssertNumberOfCardsInDeck(starlight, numCardsLeft - 1);
                if (numCardsLeft != 1)
                {
                    //first card chosen after Celestial Aura should be on bottom of deck
                    AssertOnBottomOfDeck("WarpHalo");
                    //last card chosen should be on top
                    AssertOnTopOfDeck(cardsToPut[numCardsLeft - 1]);
                }
            }
        }
        [Test()]
        public void TestWishNinjaInterruption()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "TheTempleOfZhuLong");
            StartGame();

            Card ninja = GetCard("ShinobiAssassin");
            PutOnDeck(starlight, ninja);
            PutOnDeck("WarpHalo");

            AssertMaxNumberOfDecisions(5);
            AssertNextMessages("Starlight would have revealed Shinobi Assassin! It will be put into play instead, then the next card will be revealed.");

            PlayCard("Wish");

            AssertIsInPlay(ninja);
            AssertIsInPlay("WarpHalo");
        }
        [Test()]
        public void TestWishNinjaVisionaryComparison()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "TheTempleOfZhuLong");
            StartGame();

            Card ninja = GetCard("ShinobiAssassin");
            PutOnDeck(visionary, ninja);
            PutOnDeck("WarpHalo");

            PutIntoPlay("Foresight");

            //just to make sure Wish is dealing with ninjas right, we compare with Visionary
            //Wish still pulls 5 cards, Foresight's power should pull 3
            AssertMaxNumberOfDecisions(2);
            UsePower("Foresight");
        }
        [Test()]
        public void TestWishOnlyUnplayableCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "TheTempleOfZhuLong");
            StartGame();

            string[] limitedCards = new string[5] { "CelestialAura", "GoldenAstrolabe", "NightloreArmor", "NovaShield", "WarpHalo" };
            PutIntoPlay(starlight, limitedCards);
            limitedCards.ForEach((string id) => PutOnDeck(id));
            Card wish = GetCard("Wish");
            PutInHand(wish);

            int deckSize = starlight.TurnTaker.Deck.NumberOfCards;
            AssertNumberOfCardsInPlay(starlight, 6);
            PlayCard(wish);
            AssertNumberOfCardsInPlay(starlight, 6);
            AssertNumberOfCardsInDeck(starlight, deckSize);
        }
    }
}
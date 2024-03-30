using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace CauldronTests
{
    [TestFixture()]
    class FSCContinuanceWandererTests : CauldronBaseTest
    {
        protected TurnTakerController fsc { get { return FindEnvironment(); } }

        private void AddShuffleTrashCounterAttackTrigger(TurnTakerController ttc, TurnTaker turnTakerToReshuffleTrash, Card cardSource)
        {
            Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.Target == ttc.CharacterCard;
            Func<DealDamageAction, IEnumerator> response = (DealDamageAction dd) => this.GameController.ShuffleTrashIntoDeck(this.GameController.FindTurnTakerController(turnTakerToReshuffleTrash));
            this.GameController.AddTrigger<DealDamageAction>(new Trigger<DealDamageAction>(this.GameController, criteria, response, new TriggerType[] { TriggerType.ShuffleTrashIntoDeck }, TriggerTiming.After, this.GameController.FindCardController(cardSource).GetCardSource()));
        }

        [Test()]
        public void TestLoadFSC()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }

        [Test()]
        public void TestFSCDecklist()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            Card borg = GetCard("CombatCyborg");
            AssertIsTarget(borg, 4);
            AssertCardHasKeyword(borg, "time monster", false);

            Card paradox = GetCard("ParadoxIntrusion");
            AssertIsTarget(paradox, 4);
            AssertCardHasKeyword(paradox, "time monster", false);

            Card behemoth = GetCard("PrehistoricBehemoth");
            AssertIsTarget(behemoth, 8);
            AssertCardHasKeyword(behemoth, "time monster", false);

            Card glitch = GetCard("VortexGlitch");
            AssertCardHasKeyword(glitch, "time vortex", false);

            Card interference = GetCard("VortexInterference");
            AssertCardHasKeyword(interference, "time vortex", false);

            Card surge = GetCard("VortexSurge");
            AssertCardHasKeyword(surge, "time vortex", false);
        }

        [Test()]
        public void TestCombatCyborgEndDamageVillain()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            int mdpHitpoints = mdp.HitPoints ?? default;
            QuickHPStorage(ra, legacy, haka, baron);
            PlayCard("CombatCyborg");
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            GoToEndOfTurn(env);
            QuickHPCheck(0, 0, 0, 0);
            Assert.AreEqual(mdpHitpoints - 1, mdp.HitPoints);
        }

        [Test()]
        public void TestCombatCyborgEndDamageHero()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            GoToPlayCardPhase(env);
            QuickHPStorage(ra, legacy, haka, spite);
            PlayCard("CombatCyborg");
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            GoToEndOfTurn(env);
            QuickHPCheck(-1, 0, 0, 0);
        }

        [Test()]
        public void TestCombatCyborgEndNotDamageEnvironment()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card intrusion = GetCard("ParadoxIntrusion");
            GoToPlayCardPhase(env);
            PlayCard("CombatCyborg");
            PlayCard(intrusion);
            QuickHPStorage(intrusion);
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            GoToEndOfTurn(env);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestCombatCyborgReduceDamage()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card intrusion = GetCard("ParadoxIntrusion");
            Card borg = GetCard("CombatCyborg");
            PlayCard(borg);
            PlayCard(intrusion);
            //Reduce damage dealt to environment targets by 2.
            QuickHPStorage(intrusion);
            DealDamage(ra, intrusion, 3, DamageType.Fire);
            QuickHPCheck(-1);
            //Reduce damage dealt to environment targets by 2.
            QuickHPStorage(borg);
            DealDamage(ra, borg, 3, DamageType.Fire);
            QuickHPCheck(-1);
            //check only environment targets
            QuickHPStorage(haka);
            DealDamage(ra, haka, 3, DamageType.Fire);
            QuickHPCheck(-3);
        }
        [Test()]
        public void TestHeartOfTheWandererDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card heart = PlayCard("HeartOfTheWanderer");
            //stack spite's deck to not play lab raid
            PutOnDeck("GoodSamaritan");
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, heart);
            GoToEndOfTurn(env);
            AssertInTrash(env, heart);
        }

        [Test()]
        public void TestHeartOfTheWandererDiscard()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            Card lab = GetCard("LabRaid");
            PutOnDeck(spite, lab);
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(spite.TurnTaker.Trash),
                new MoveCardDestination(legacy.TurnTaker.Trash),
                new MoveCardDestination(ra.TurnTaker.Trash),
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(fsc.TurnTaker.Trash)
            };
            PlayCard(heart);
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            AssertNumberOfCardsInTrash(spite, 1);
            AssertNumberOfCardsInTrash(legacy, 1);
            AssertNumberOfCardsInTrash(ra, 1);
            AssertNumberOfCardsInTrash(haka, 1);
            AssertNumberOfCardsInTrash(fsc, 1);

            AssertNumberOfCardsInRevealed(spite, 0);
            AssertNumberOfCardsInRevealed(legacy, 0);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(haka, 0);
            AssertNumberOfCardsInRevealed(fsc, 0);
        }

        [Test()]
        public void TestHeartOfTheWandererReturn()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(spite.TurnTaker.Deck),
                new MoveCardDestination(legacy.TurnTaker.Deck),
                new MoveCardDestination(ra.TurnTaker.Deck),
                new MoveCardDestination(haka.TurnTaker.Deck),
                new MoveCardDestination(fsc.TurnTaker.Deck)
            };
            PlayCard(heart);
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            AssertNumberOfCardsInTrash(spite, 0);
            AssertNumberOfCardsInTrash(legacy, 0);
            AssertNumberOfCardsInTrash(ra, 0);
            AssertNumberOfCardsInTrash(haka, 0);
            AssertNumberOfCardsInTrash(fsc, 0);

            AssertNumberOfCardsInRevealed(spite, 0);
            AssertNumberOfCardsInRevealed(legacy, 0);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(haka, 0);
            AssertNumberOfCardsInRevealed(fsc, 0);
        }

        [Test]
        public void TestHeartOfWandererOblivAeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket", "Legacy", "Haka", "Cauldron.FSCContinuanceWanderer", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            Card expectedMission = oblivaeon.TurnTaker.FindSubDeck("MissionDeck").TopCard;
            Card aeonWarrior = GetCard("AeonWarrior");
            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: envOne.BattleZone.FindScion().PlayArea);
            PlayCard(oblivaeon, borrScion, overridePlayLocation: envOne.BattleZone.FindScion().PlayArea);
            GoToStartOfTurn(envOne);
            PlayCard("HeartOfTheWanderer");
            Card actualMission = oblivaeon.TurnTaker.FindSubDeck("MissionDeck").TopCard;

            Assert.AreEqual(expectedMission, actualMission, $"Expected {expectedMission.Title} on the top of the Mission Deck. Instead it was {actualMission.Title}");
            
        }
        [Test()]
        public void TestHeartOfTheWandererTeamVillainDiscard()
        {
            SetupGameController("ErmineTeam", "Legacy", "BiomancerTeam", "Ra", "FrictionTeam", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(ermineTeam.TurnTaker.Trash),
                new MoveCardDestination(legacy.TurnTaker.Trash),
                new MoveCardDestination(biomancerTeam.TurnTaker.Trash),
                new MoveCardDestination(ra.TurnTaker.Trash),
                new MoveCardDestination(frictionTeam.TurnTaker.Trash),
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(fsc.TurnTaker.Trash)
            };
            PlayCard(heart);
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            AssertNumberOfCardsInTrash(ermineTeam, 1);
            AssertNumberOfCardsInTrash(legacy, 1);
            AssertNumberOfCardsInTrash(biomancerTeam, 1);
            AssertNumberOfCardsInTrash(ra, 1);
            AssertNumberOfCardsInTrash(frictionTeam, 1);
            AssertNumberOfCardsInTrash(haka, 1);
            AssertNumberOfCardsInTrash(fsc, 1);

            AssertNumberOfCardsInRevealed(ermineTeam, 0);
            AssertNumberOfCardsInRevealed(legacy, 0);
            AssertNumberOfCardsInRevealed(biomancerTeam, 0);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(frictionTeam, 0);
            AssertNumberOfCardsInRevealed(haka, 0);
            AssertNumberOfCardsInRevealed(fsc, 0);





        }

        [Test()]
        public void TestParadoxIntrusionEndTurnDamage0Vortex()
        {
            SetupGameController("Cauldron.FSCContinuanceWanderer", "Spite", "Guise", "Parse", "Haka");
            StartGame();
            GoToPlayCardPhase(env);
            PlayCard("ParadoxIntrusion");
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            QuickHPStorage(haka, spite);
            GoToEndOfTurn(env);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestParadoxIntrusionEndTurnDamage2Vortex()
        {
            SetupGameController("Cauldron.FSCContinuanceWanderer", "LaCapitan", "Guise", "Parse", "Haka");
            StartGame();
            Card boat = GetCardInPlay("LaParadojaMagnifica");
            PutInTrash("VortexSurge", "VortexGlitch");
            GoToPlayCardPhase(env);
            PlayCard("ParadoxIntrusion");
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            QuickHPStorage(haka.CharacterCard, capitan.CharacterCard, boat);
            GoToEndOfTurn(env);
            QuickHPCheck(-3, -2, -2);
        }

        [Test()]
        public void TestParadoxIntrusionEndTurnDamage_Dynamic()
        {
            SetupGameController("Cauldron.FSCContinuanceWanderer", "LaCapitan", "Guise", "Parse", "Haka");
            StartGame();
            Card boat = GetCardInPlay("LaParadojaMagnifica");
            PutInTrash("VortexSurge", "VortexGlitch");
            GoToPlayCardPhase(env);
            PlayCard("ParadoxIntrusion");
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            QuickHPStorage(haka.CharacterCard, capitan.CharacterCard, boat);
            AddShuffleTrashCounterAttackTrigger(capitan, fsc.TurnTaker, capitan.CharacterCard);
            GoToEndOfTurn(env);
            //trash should be reshuffled so should no longer have enough targets to also damage boat
            QuickHPCheck(-3, -2, 0);
        }

        [Test()]
        public void TestPrehistoricBehemothEndDamage()
        {
            //This card is immune to damage dealt by targets with less than 10HP.
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Tachyon", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            DestroyCard("LaParadojaMagnifica");
            GoToPlayCardPhase(env);
            PlayCard("PrehistoricBehemoth");
            //At the end of the environment turn, this card deals the {H - 2} targets 2 melee damage each.
            QuickHPStorage(haka, parse, guise, capitan);
            GoToEndOfTurn(env);
            QuickHPCheck(-2, 0, 0, -2);
        }

        [Test()]
        public void TestPrehistoricBehemothImmune()
        {
            //This card is immune to damage dealt by targets with less than 10HP.
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card behemoth = GetCard("PrehistoricBehemoth");
            PlayCard(behemoth);
            //Source HP > 10
            QuickHPStorage(behemoth);
            DealDamage(haka, behemoth, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Source HP < 10
            SetHitPoints(haka, 8);
            QuickHPStorage(behemoth);
            DealDamage(haka, behemoth, 2, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSuperimposedRealitiesTargetNoChange()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            DecisionSelectCard = legacy.CharacterCard;
            PlayCard("SuperimposedRealities");
            //The target should have no change to its actions
            PlayCard("Thokk");
            AssertInTrash("Thokk");

            QuickHandStorage(legacy);
            DrawCard(legacy);
            QuickHandCheck(1);

            UsePower(legacy);
            QuickHPStorage(ra);
            DealDamage(legacy, ra, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestSuperimposedRealitiesPlayAction()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card thokk = GetCard("Thokk");
            Card blast = GetCard("FireBlast");
            PutInHand(thokk);
            PutInHand(blast);
            //Going to Legacy to prevent suprise Select Card Decisions from villain
            GoToEndOfTurn(legacy);
            DecisionSelectCard = legacy.CharacterCard;
            PlayCard("SuperimposedRealities");

            DecisionSelectCard = thokk;
            PlayCard(blast);
            AssertInTrash(thokk);
            AssertInHand(blast);
        }
        [Test()]
        public void TestSuperimposedRealitiesPlayActionPutIntoPlay()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "OmnitronX", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(omnix);
            Card plating = PutInHand("AblativeCoating");
            Card blast = PutOnDeck("FireBlast");

            DecisionSelectCard = omnix.CharacterCard;
            PlayCard("SuperimposedRealities");
            DecisionSelectCard = null;

            DecisionSelectLocation = new LocationChoice(ra.TurnTaker.Deck);
            DecisionYesNo = true;

            UsePower(omnix);
            AssertInTrash(blast);
            AssertInHand(plating);
        }

        [Test()]
        public void TestSuperimposedRealitiesPowerAction()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card thokk = GetCard("Thokk");
            PutInHand(thokk);
            //Going to Legacy to prevent suprise Decisions from villain
            GoToEndOfTurn(legacy);
            DecisionSelectCard = legacy.CharacterCard;
            PlayCard("SuperimposedRealities");
            DecisionSelectTarget = spite.CharacterCard;
            UsePower(ra);
            //checking Spite because Ra's power should fail and deal no damage
            QuickHPStorage(haka, spite);
            DealDamage(legacy, haka, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }


        [Test()]
        public void TestSuperimposedRealitiesDrawAction()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            PutOnDeck("MobileDefensePlatform");
            StartGame();
            DecisionSelectCard = legacy.CharacterCard;
            PlayCard("SuperimposedRealities");
            Card bolster = GetCard("BolsterAllies");
            PutInHand(bolster);

            var t1 = GetTopCardOfDeck(ra);
            var t2 = GetTopCardOfDeck(haka);

            var d1 = GetTopCardOfDeck(legacy, 0);
            var d2 = GetTopCardOfDeck(legacy, 1);
            var d3 = GetTopCardOfDeck(legacy, 2);
            
            QuickHandStorage(legacy, ra, haka);
            PlayCard(bolster);
            //Draw one per hero, but played Bolster Allies
            QuickHandCheck(2, 0, 0);
            AssertOnTopOfDeck(ra, t1);
            AssertOnTopOfDeck(haka, t2);
            AssertInHand(legacy, d1);
            AssertInHand(legacy, d2);
            AssertInHand(legacy, d3);
        }

        [Test()]
        public void TestSuperimposedRealitiesDestroySelf()
        {
            SetupGameController(new string[] { "Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer" });
            StartGame();
            GoToStartOfTurn(haka);
            //stack spites deck to not get lab raid
            PutOnDeck("GoodSamaritan");
            DecisionSelectCard = legacy.CharacterCard;
            Card reality = PlayCard("SuperimposedRealities");
            //At the start of the environment turn, destroy this card.
            GoToEndOfTurn(haka);
            AssertNextToCard(reality, legacy.CharacterCard);
            GoToStartOfTurn(env);
            AssertInTrash(env, reality);
        }

        [Test()]
        public void TestSuperimposedRealities_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.FSCContinuanceWanderer", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash
            Card reality = PlayCard("SuperimposedRealities");
            AssertInTrash(reality);

        }

        [Test()]
        public void TestTemporalAccelerationDestroySelf()
        {
            SetupGameController(new string[] { "Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer" });
            StartGame();
            GoToStartOfTurn(haka);
            //stack spites deck to not get lab raid
            PutOnDeck("GoodSamaritan");
            Card accel = PlayCard("TemporalAcceleration");
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, accel);
            GoToEndOfTurn(env);
            AssertInTrash(env, accel);
        }

        [Test()]
        public void TestTemporalAccelerationPlayCards()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card field = PutOnDeck("BacklashField");
            Card ring = PutOnDeck("TheLegacyRing");
            Card staff = PutOnDeck("TheStaffOfRa");
            Card mere = PutOnDeck("Mere");

            //When this card enters play, play the top card of the villain deck. Then, play the top card of each hero deck in turn order.
            PlayCard(GetCard("TemporalAcceleration"));
            AssertInPlayArea(baron, field);
            AssertInPlayArea(legacy, ring);
            AssertInPlayArea(ra, staff);
            AssertInPlayArea(haka, mere);
        }

        [Test()]
        public void TestTemporalResetDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack spites deck to not get lab raid
            PutOnDeck("GoodSamaritan");
            GoToStartOfTurn(haka);
            Card reset = PlayCard("TemporalReset");
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, reset);
            GoToEndOfTurn(env);
            AssertInTrash(env, reset);
        }

        [Test()]
        public void TestTemporalReset()
        {
            SetupGameController("LaCapitan", "Ra", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            foreach(TurnTakerController ttc in base.GameController.TurnTakerControllers)
            {
                if(ttc != fsc)
                {
                    IEnumerable<Card> toTrash = FindCardsWhere((Card c) => ttc.TurnTaker.Deck.Cards.Contains(c)).Take(2);
                    PutInTrash(toTrash);
                }      
            }

            Card pi0 = PlayCard("ParadoxIntrusion", 0);
            Card pi1 = PlayCard("ParadoxIntrusion", 1);
            Card boat = GetCardInPlay("LaParadojaMagnifica");
            SetHitPoints(boat, 2);
            //When this card enters play, destroy all other environment cards. Then shuffle 2 cards from each trash pile back into their deck, and each non-character target regains {H} HP.
            QuickShuffleStorage(capitan, ra, parse, haka, env);
            QuickHPStorage(boat);
            Card reset = PlayCard("TemporalReset");
            QuickShuffleCheck(1,1,1,1,1);
            AssertInDeck(env, pi0);
            AssertInDeck(env, pi1);
            AssertNumberOfCardsInTrash(capitan, 0);
            AssertNumberOfCardsInTrash(ra, 0);
            AssertNumberOfCardsInTrash(parse, 0);
            AssertNumberOfCardsInTrash(haka, 0);

            QuickHPCheck(3);
        }

        [Test()]
        public void TestTemporalReversal()
        {
            SetupGameController("LaCapitan", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //When this card enters play, place 1 card in play from each other deck back on top of that deck.
            IEnumerable<Card> inDeck = GetCards("Fortitude", "FleshOfTheSunGod", "Mere");
            PlayCards(inDeck);
            IEnumerable<Card> inPlay = GetCards("TheLegacyRing", "TheStaffOfRa", "TaMoko", "Trueshot");
            PlayCards(inPlay);
            Card rev = GetCard("TemporalReversal");
            PlayCard(rev);
            AssertIsInPlay(inPlay);
            AssertInDeck(capitan, GetCard("LaParadojaMagnifica"));
            AssertInDeck(inDeck);
        }

        [Test()]
        public void TestTemporalReversalDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack spites deck to not get lab raid
            PutOnDeck("GoodSamaritan");
            GoToStartOfTurn(haka);
            Card rev = PlayCard("TemporalReversal");
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, rev);
            GoToEndOfTurn(env);
            AssertInTrash(env, rev);
        }

        [Test()]
        public void TestTemporalSlipstream()
        {
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card slip = GetCard("TemporalSlipstream");
            //give haka more cards in hand
            DrawCard(haka, 4);
            int guiseTrash = guise.TurnTaker.Trash.NumberOfCards;
            int parseTrash = parse.TurnTaker.Trash.NumberOfCards;
            int hakaTrash = haka.TurnTaker.Trash.NumberOfCards;
            int guiseHand = guise.NumberOfCardsInHand;
            int parseHand = parse.NumberOfCardsInHand;
            int hakaHand = haka.NumberOfCardsInHand;
            QuickHandStorage(guise, parse, haka);
            //When this card enters play, each player discards their hand and draws that many cards.
            PlayCard(slip);
            AssertNumberOfCardsInTrash(guise, guiseHand + guiseTrash);
            AssertNumberOfCardsInTrash(parse, parseHand + parseTrash);
            AssertNumberOfCardsInTrash(haka, hakaHand + hakaTrash);
            QuickHandCheck(0, 0, 0);
        }

        [Test()]
        public void TestTemporalSlipstreamDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card slip = GetCard("TemporalSlipstream");
            GoToStartOfTurn(haka);
            //stack spites deck to not get lab raid
            PutOnDeck("GoodSamaritan");
            PlayCard(slip);
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, slip);
            GoToEndOfTurn(env);
            AssertInTrash(env, slip);
        }

        [Test()]
        public void TestTimeFreezeNotTriggerPhase()
        {
            Game game = new Game(new string[] { "LaCapitan", "CaptainCosmic", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer" });
            SetupGameController(game);
            StartGame();
            DestroyNonCharacterVillainCards();

            Card slip = GetCard("TimeFreeze");
            Card crest = GetCard("CosmicCrest");
            PlayCard(crest);
            DecisionSelectCard = cosmic.CharacterCard;
            GoToPlayCardPhase(capitan);
            //Play this card next to a hero.
            PlayCard(slip);
            //That hero skips their turns...
            this.RunCoroutine(GameController.EnterNextTurnPhase());
            AssertTurnPhaseDetails(game.ActiveTurnPhase, capitan, Phase.End);
            this.RunCoroutine(GameController.EnterNextTurnPhase());
            AssertTurnPhaseDetails(game.ActiveTurnPhase, parse, Phase.Start);
            //...and targets in their play are are immune to damage.
            QuickHPStorage(cosmic.CharacterCard, crest, parse.CharacterCard);
            DealDamage(capitan, cosmic, 2, DamageType.Melee);
            DealDamage(capitan, crest, 2, DamageType.Melee);
            DealDamage(capitan, parse, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestTimeFreezeTriggerPhase()
        {
            Game game = new Game(new string[] { "LaCapitan", "CaptainCosmic", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer" });
            SetupGameController(game);
            StartGame();
            DestroyNonCharacterVillainCards();

            Card slip = GetCard("TimeFreeze");
            Card crest = GetCard("CosmicCrest");
            PlayCard(crest);
            DecisionSelectCard = cosmic.CharacterCard;
            GoToEndOfTurn(capitan);
            //Play this card next to a hero.
            PlayCard(slip);
            //That hero skips their turns...
            AssertTurnPhaseDetails(game.ActiveTurnPhase, capitan, Phase.End);
            this.RunCoroutine(GameController.EnterNextTurnPhase());
            AssertTurnPhaseDetails(game.ActiveTurnPhase, parse, Phase.Start);
            //...and targets in their play are are immune to damage.
            QuickHPStorage(cosmic.CharacterCard, crest, parse.CharacterCard);
            DealDamage(capitan, cosmic, 2, DamageType.Melee);
            DealDamage(capitan, crest, 2, DamageType.Melee);
            DealDamage(capitan, parse, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestTimeFreezeDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card freeze = GetCard("TimeFreeze");
            GoToStartOfTurn(haka);
                      //stack spites deck to not get lab raid
            PutOnDeck("GoodSamaritan");
            DecisionSelectCard = legacy.CharacterCard;
            PlayCard(freeze);
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInTrash(env, freeze);
        }
        [Test()]
        public void TestTimeFreezeWithTurnOrderReversed()
        {
            SetupGameController("WagerMaster", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();

            var conditions = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsCondition);
            foreach (Card startingCondition in conditions)
            {
                FlipCard(startingCondition);
            }

            Card dice = GetCard("PlayingDiceWithTheCosmos");
            if(dice.IsInPlay)
            {
                FlipCard(dice);
                MoveCard(wager, dice, wager.TurnTaker.Deck, overrideIndestructible: true);
            }
            PlayCard(dice);


            GoToEndOfTurn(ra);
            Card takedown = PlayCard("TakeDown");
            PlayCard("TimeFreeze");
            RunCoroutine(GameController.EnterNextTurnPhase());
            RunCoroutine(GameController.EnterNextTurnPhase());

            AssertIsInPlay(takedown);
            Assert.IsTrue(GameController.ActiveTurnTaker.IsEnvironment);
        }

        [Test()]
        public void TestTimeFreeze_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.FSCContinuanceWanderer", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash
            Card timeFreeze = PlayCard("TimeFreeze");
            AssertInTrash(timeFreeze);

        }
        [Test()]
        public void TestTimeFreezeWithTurnAndPhaseOrderReversed()
        {
            SetupGameController("WagerMaster", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();

            var conditions = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsCondition);
            foreach (Card startingCondition in conditions)
            {
                FlipCard(startingCondition);
            }

            Card dice = GetCard("PlayingDiceWithTheCosmos");
            if (dice.IsInPlay)
            {
                FlipCard(dice);
                MoveCard(wager, dice, wager.TurnTaker.Deck, overrideIndestructible: true);
            }
            DecisionSelectCard = ra.CharacterCard;
            PlayCard("TimeFreeze");
            DecisionSelectCard = null;

            PlayCard(dice);
            PlayCard("BreakingTheRules");


            GoToStartOfTurn(haka);
            Card takedown = PlayCard("TakeDown");
            RunCoroutine(GameController.EnterNextTurnPhase());
            RunCoroutine(GameController.EnterNextTurnPhase());

            AssertIsInPlay(takedown);
            Assert.IsTrue(GameController.ActiveTurnTaker == legacy.TurnTaker);
        }
        [Test()]
        public void TestTimeFreezeWithPhaseOrderReversed()
        {
            SetupGameController("WagerMaster", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();

            var conditions = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsCondition);
            foreach (Card startingCondition in conditions)
            {
                FlipCard(startingCondition);
            }

            GoToEndOfTurn(wager);
            PlayCard("TimeFreeze");
            PlayCard("BreakingTheRules");
            Card flare = PlayCard("SolarFlare");

            DecisionYesNo = false;

            RunCoroutine(GameController.EnterNextTurnPhase());
            RunCoroutine(GameController.EnterNextTurnPhase());
            AssertInTrash(flare);
            Assert.IsTrue(GameController.ActiveTurnTaker == ra.TurnTaker);
        }

        [Test()]
        public void TestVortexGlitch() //This Test is known to fail
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            Card glitch = GetCard("VortexGlitch");
            //Players may not play one-shots.
            PlayCard(glitch);
            GoToPlayCardPhase(legacy);
            PlayCard("Thokk");
            AssertNotInTrash(legacy, "Thokk");
            //Currently the CannotPlayCards function doesn't only prevent certain cards. It's all or nothing
            IEnumerable<Card> cards = GetCards("Fortitude", "TheLegacyRing");
            PlayCards(cards);
            AssertInPlayArea(legacy, cards);
        }

        [Test()]
        public void TestVortexGlitchDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            Card glitch = GetCard("VortexGlitch");
            GoToStartOfTurn(haka);

            PlayCard(glitch);
            GoToStartOfTurn(env);
            AssertInPlayArea(env, glitch);
            GoToEndOfTurn(env);
            AssertInPlayArea(env, glitch);
            //When another environment card enters play, destroy this card.
            PlayCard("PrehistoricBehemoth");
            AssertInTrash(env, glitch);
        }

        [Test()]
        public void TestVortexInterferenceDestroyEquipment()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            PlayCard("VortexInterference");
            Card staff = GetCard("TheStaffOfRa");
            Card ring = GetCard("TheLegacyRing");
            PlayCards(staff, ring);
            //Whenever a hero uses a power, destroy 1 hero ongoing or equipment card.
            DecisionSelectCard = staff;
            UsePower(legacy);
            AssertInPlayArea(legacy, ring);
            AssertInTrash(staff);
        }

        [Test()]
        public void TestVortexInterferenceDestroyOngoing()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            PlayCard("VortexInterference");
            Card fort = GetCard("Fortitude");
            Card moko = GetCard("TaMoko");
            PlayCards(fort, moko);
            //Whenever a hero uses a power, destroy 1 hero ongoing or equipment card.
            UsePower(legacy);
            AssertInPlayArea(haka, moko);
            AssertInTrash(fort);
        }

        [Test()]
        public void TestVortexInterferenceDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            Card interference = GetCard("VortexInterference");
            GoToStartOfTurn(haka);
            PlayCard(interference);
            GoToStartOfTurn(env);
            AssertInPlayArea(env, interference);
            GoToEndOfTurn(env);
            AssertInPlayArea(env, interference);
            //When another environment card enters play, destroy this card.
            PlayCard("PrehistoricBehemoth");
            AssertInTrash(env, interference);
        }

        [Test()]
        public void TestVortexSurge()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            PlayCard("VortexSurge");
            //Whenever a hero card is drawn, 1 player must discard a card.
            QuickHandStorage(ra, legacy);
            DecisionSelectCard = legacy.HeroTurnTaker.Hand.Cards.First();
            DrawCard(ra);
            QuickHandCheck(1, -1);
        }

        [Test()]
        public void TestVortexSurgeDestroySelf()
        {
            SetupGameController("Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            //stack deck to not have lab raid
            PutOnDeck("GoodSamaritan");
            Card surge = GetCard("VortexSurge");
            GoToStartOfTurn(haka);
            PlayCard(surge);
            GoToStartOfTurn(env);
            AssertInPlayArea(env, surge);
            GoToEndOfTurn(env);
            AssertInPlayArea(env, surge);
            //When another environment card enters play, destroy this card.
            PlayCard("PrehistoricBehemoth");
            AssertInTrash(env, surge);
        }
    }
}

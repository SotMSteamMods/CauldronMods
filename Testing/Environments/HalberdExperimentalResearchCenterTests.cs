using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class HalberdExperimentalResearchCenterTests : BaseTest
    {

        #region HalberdExperimentalResearchCenterHelperFunctions

        protected TurnTakerController halberd { get { return FindEnvironment(); } }

        

        #endregion

        [Test()]
        public void TestHalberdWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
            
        }
        [Test()]
        public void TestEmergencyProtocolsPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();
            //stack the deck so we know what the played card will be
            Card prophet = GetCard("HalberdProphet");
            PutOnDeck(halberd, prophet);
            GoToPlayCardPhase(halberd);
            AssertInDeck(prophet);
            //At the end of the environment turn, play the top card of the environment deck.

            PlayCard("EmergencyReleaseProtocol");
            GoToEndOfTurn(halberd);
            //since the top card of the deck was played, prophet should be in play
            AssertIsInPlay(prophet);

        }

        [Test()]
        public void TestEmergencyProtocolsDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            Card emergency = GetCard("EmergencyReleaseProtocol");
            PlayCard(emergency);
            AssertIsInPlay(emergency);

            //At the start of their turn, a player may skip the rest of their turn to destroy this card.
            //yes we want the player to skip their turn
            DecisionYesNo = true;
            GoToStartOfTurn(ra);
            AssertInTrash(emergency);

        }

        [Test()]
        public void TestHalberdAlpha_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //destroy mdp to make baron vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            //put battalion to make sure only hits highest hp
            Card battalion = GetCard("BladeBattalion");
            PlayCard(battalion);

            GoToPlayCardPhase(halberd);

            //we play out alpha
            Card alpha = GetCard("HalberdAlpha");
            PlayCard(alpha);
            AssertIsInPlay(alpha);

            //put another test subject in play
            //zephyr just causes hp gain, so doesn't impact this test
            Card zephyr = GetCard("HalberdZephyr");
            PlayCard(zephyr);
            AssertIsInPlay(zephyr);

            //At the end of the environment turn, if there are no Chemical Triggers in play, each Test Subject deals the villain target with the highest HP 1 melee damage. 
            QuickHPStorage(baron.CharacterCard, battalion);
            GoToEndOfTurn(halberd);
            //there are 2 test subjects in play, each deal baron 1 damage, 2 damage total
            //battalion should not have been dealt damage
            QuickHPCheck(-2, 0);

        }

        [Test()]
        public void TestHalberdOmega_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //destroy mdp to make baron vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            //put battalion in play to have multiple villain targets
            Card battalion = GetCard("BladeBattalion");
            PlayCard(battalion);

            GoToPlayCardPhase(halberd);

            //we play out alpha
            Card omega = GetCard("HalberdOmega");
            PlayCard(omega);
            AssertIsInPlay(omega);


            //At the end of the environment turn, if there are no Chemical Triggers in play, this cards deals each villain target 2 infernal damage.
            QuickHPStorage(baron.CharacterCard, battalion);
            GoToEndOfTurn(halberd);
            //deal baron and battalion 2 damage
            QuickHPCheck(-2, -2);

        }

        [Test()]
        public void TestHalberdAlpha_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            GoToPlayCardPhase(halberd);

            Card alpha = GetCard("HalberdAlpha");
            PlayCard(alpha);
            AssertIsInPlay(alpha);

            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            //destroy it
            Card omega = GetCardInPlay("HalberdOmega");
            DestroyCard(omega, baron.CharacterCard);

            //put another test subject in play
            //zephyr just causes hp gain, so doesn't impact this test
            Card zephyr = GetCard("HalberdZephyr");
            PlayCard(zephyr);
            AssertIsInPlay(zephyr);


            //Otherwise, each Test Subject deals the hero target with the highest HP 1 melee damage.
            //ra is highest hp,  2 damage to it, 1 for each test subject in play
            QuickHPStorage(ra, legacy, haka);
            GoToEndOfTurn(halberd);
            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestHalberdOmega_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            GoToPlayCardPhase(halberd);


            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            Card omega = GetCardInPlay("HalberdOmega");
            AssertIsInPlay(omega);


            //Omega: Otherwise, this cards deals each hero target 2 infernal damage.
            //2 damage to all heroes
            QuickHPStorage(ra, legacy, haka);
            GoToEndOfTurn(halberd);
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestHrCombatPheromones_StartOfTurnIndestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //go to haka's end of turn to prime environment
            GoToEndOfTurn(haka);


            Card combat = GetCard("HrCombatPheromones");
            PlayCard(combat);
            AssertIsInPlay(combat);

            //when this was played, omega entered play

            //This card is indestructible if at least 1 Test Subject is in play. 
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(halberd);

            //since there is a test subject in play, should not be destroyed
            AssertIsInPlay(combat);

        }

        [Test()]
        public void TestHrCombatPheromones_StartOfTurnCanBeDestroye()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            //go to haka's end of turn to prime environment
            GoToEndOfTurn(haka);


            Card combat = GetCard("HrCombatPheromones");
            PlayCard(combat);
            AssertIsInPlay(combat);

            //Destroy omega so there are no test subjects in play
            Card omega = GetCardInPlay("HalberdOmega");
            DestroyCard(omega, baron.CharacterCard);

            //This card is indestructible if at least 1 Test Subject is in play. 
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(halberd);
            //since no test subjects in play, this card will destroy itself
            AssertInTrash(combat);

        }

        [Test()]
        public void TestHrCombatPheromones_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            Card omega = GetCard("HalberdOmega");
            Card combat = GetCard("HrCombatPheromones");

            AssertInDeck(omega);

            //When this card enters play, search the environment deck and trash for Halberd-12: Omega and put it into play, then shuffle the deck.
            QuickShuffleStorage(halberd.TurnTaker.Deck);
            PlayCard(combat);
            AssertIsInPlay(omega);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestHrCombatPheromones_Reduce()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            Card combat = GetCard("HrCombatPheromones");
            PlayCard(combat);
            AssertIsInPlay(combat);

            //grab the omega test subject that was put into play
            Card omega = GetCardInPlay("HalberdOmega");
            AssertIsInPlay(omega);

            //Reduce damage dealt to Test Subjects by 1.
            QuickHPStorage(omega);
            DealDamage(ra.CharacterCard, omega, 3, DamageType.Fire);
            QuickHPCheck(-2);



        }

        [Test()]
        public void TestHtAggressionStimulant_StartOfTurnIndestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //go to haka's end of turn to prime environment
            GoToEndOfTurn(haka);


            Card combat = GetCard("HtAggressionStimulant");
            PlayCard(combat);
            AssertIsInPlay(combat);

            //when this was played, alpha entered play

            //This card is indestructible if at least 1 Test Subject is in play. 
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(halberd);

            //since there is a test subject in play, should not be destroyed
            AssertIsInPlay(combat);

        }

        [Test()]
        public void TestHtAggressionStimulant_StartOfTurnCanBeDestroye()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            //go to haka's end of turn to prime environment
            GoToEndOfTurn(haka);


            Card stimulant = GetCard("HtAggressionStimulant");
            PlayCard(stimulant);
            AssertIsInPlay(stimulant);

            //Destroy alpha so there are no test subjects in play
            Card alpha = GetCardInPlay("HalberdAlpha");
            DestroyCard(alpha, baron.CharacterCard);

            //This card is indestructible if at least 1 Test Subject is in play. 
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(halberd);
            //since no test subjects in play, this card will destroy itself
            AssertInTrash(stimulant);

        }

        [Test()]
        public void TestHtAggressionStimulant_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            Card alpha = GetCard("HalberdAlpha");
            Card stimulant = GetCard("HtAggressionStimulant");

            AssertInDeck(alpha);

            //When this card enters play, search the environment deck and trash for Halberd-04: Alpha and put it into play, then shuffle the deck.
            QuickShuffleStorage(halberd.TurnTaker.Deck);
            PlayCard(stimulant);
            AssertIsInPlay(alpha);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestHtAggressionStimulant_Increase()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            Card stimulant = GetCard("HtAggressionStimulant");
            PlayCard(stimulant);
            AssertIsInPlay(stimulant);

            //grab the alpha test subject that was put into play
            Card alpha = GetCardInPlay("HalberdAlpha");
            AssertIsInPlay(alpha);

            //Increase damage dealt by Test Subjects by 1.
            QuickHPStorage(ra);
            DealDamage(alpha, ra.CharacterCard, 3, DamageType.Fire);
            QuickHPCheck(-4);

        }

        [Test()]
        public void TestHcBSwarmingAgent_StartOfTurnIndestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //go to haka's end of turn to prime environment
            GoToEndOfTurn(haka);

            Card alpha = GetCard("HalberdAlpha");
            Card omega = GetCard("HalberdOmega");

            PutOnDeck("EmergencyReleaseProtocol");
            PutOnDeck(halberd, alpha);
            PutOnDeck(halberd, omega);
            PutOnDeck("SubjectRecyclingProject");


            Card agent = GetCard("HcBSwarmingAgent");
            PlayCard(agent);
            AssertIsInPlay(agent);

            //when this was played, alpha and omega should have entered play

            //This card is indestructible if at least 1 Test Subject is in play. 
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(halberd);

            //since there are 2 test subjects in play, should not be destroyed
            AssertIsInPlay(agent);

        }

        [Test()]
        public void TestHcBSwarmingAgent_StartOfTurnCanBeDestroye()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            //go to haka's end of turn to prime environment
            GoToEndOfTurn(haka);

            Card alpha = GetCard("HalberdAlpha");
            Card omega = GetCard("HalberdOmega");

            PutOnDeck("EmergencyReleaseProtocol");
            PutOnDeck(halberd, alpha);
            PutOnDeck(halberd, omega);
            PutOnDeck("SubjectRecyclingProject");

            Card agent = GetCard("HcBSwarmingAgent");
            PlayCard(agent);
            AssertIsInPlay(agent);

            //Destroy alpha and omega so there are no test subjects in play
            
            DestroyCard(alpha, baron.CharacterCard);
            DestroyCard(omega, baron.CharacterCard);

            //This card is indestructible if at least 1 Test Subject is in play. 
            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(halberd);
            //since no test subjects in play, this card will destroy itself
            AssertInTrash(agent);

        }

        [Test()]
        public void TestHcBSwarmingAgent_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            Card agent = GetCard("HcBSwarmingAgent");
            Card alpha = GetCard("HalberdAlpha");
            Card omega = GetCard("HalberdOmega");

            PutOnDeck("EmergencyReleaseProtocol");
            PutOnDeck(halberd, alpha);
            PutOnDeck(halberd, omega);
            PutOnDeck("SubjectRecyclingProject");

            //When this card enters play, reveal cards from the top of the environment deck until 2 Test Subjects have been revealed. Put them into play and shuffle the remaining cards into the deck.
            QuickShuffleStorage(halberd.TurnTaker.Deck);
            AssertInDeck(alpha);
            AssertInDeck(omega);
            PlayCard(agent);
            QuickShuffleCheck(1);
            AssertIsInPlay(alpha);
            AssertIsInPlay(omega);
            AssertNumberOfCardsInRevealed(halberd, 0);

        }


        [Test()]
        public void TestHalberdProphet_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();



            GoToPlayCardPhase(halberd);

            //we play out prophet
            Card prophet = GetCard("HalberdProphet");
            PlayCard(prophet);
            AssertIsInPlay(prophet);

            Card solar = GetCard("SolarFlare");
            PutOnDeck(ra, solar);
            Card evolution = GetCard("NextEvolution");
            PutOnDeck(legacy, evolution);
            Card mere = GetCard("Mere");
            PutOnDeck(haka, mere);

            //we want to put cards on the bottom of the deck
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(ra.TurnTaker.Deck, true), new MoveCardDestination(legacy.TurnTaker.Deck, true), new MoveCardDestination(haka.TurnTaker.Deck, true) };
            AssertOnTopOfDeck(solar);
            AssertOnTopOfDeck(evolution);
            AssertOnTopOfDeck(mere);
            //At the end of the environment turn, if there are no Chemical Triggers in play, each player may look at the top card of their deck, and put it back on either the top or bottom of their deck.
            GoToEndOfTurn(halberd);
            AssertOnBottomOfDeck(solar);
            AssertOnBottomOfDeck(evolution);
            AssertOnBottomOfDeck(mere);

        }

        [Test()]
        public void TestHalberdProphet_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

           
            GoToPlayCardPhase(halberd);

            //play halberd prophet
            Card prophet = GetCard("HalberdProphet");
            PlayCard(prophet);
            AssertIsInPlay(prophet);

            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            //omega dealing damage does not impact this test
            Card omega = GetCardInPlay("HalberdOmega");
            AssertIsInPlay(omega);

            //stack top of deck
            Card battalion = GetCard("BladeBattalion");
            PutOnDeck(baron, battalion);

            //Prophet: Otherwise, play the top card of the villain deck.
            AssertInDeck(battalion);
            GoToEndOfTurn(halberd);
            AssertIsInPlay(battalion);
        }

        [Test()]
        public void TestHalberdSiren_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            //destroy mdp so baron blade is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            

            GoToPlayCardPhase(halberd);

            //we play out siren
            Card siren = GetCard("HalberdSiren");
            PlayCard(siren);
            AssertIsInPlay(siren);

            //If there are no Chemical Triggers in play, the first time the hero target with the highest HP would be dealt damage each turn, redirect that damage to the villain target with the highest HP. 
            //ra is highest hp hero
            //baron is highest hp villain
            QuickHPStorage(baron, ra);
            DealDamage(haka, ra, 3, DamageType.Melee);
            QuickHPCheck(-3, 0);

            //should only happen for the first damage
            QuickHPStorage(baron, ra);
            DealDamage(haka, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

            GoToNextTurn();

            //should be reset for the next turn
            QuickHPStorage(baron, ra);
            DealDamage(haka, ra, 3, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestHalberdSiren_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(halberd);

            //play halberd siren
            Card siren = GetCard("HalberdSiren");
            PlayCard(siren);
            AssertIsInPlay(siren);

            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            //destroy omega
            Card omega = GetCardInPlay("HalberdOmega");
            DestroyCard(omega);

            //Otherwise, the first time a non-hero target would be dealt damage each turn, redirect that damage to the hero target with the highest HP.
            //hero with highest hp is ra
            QuickHPStorage(mdp, ra.CharacterCard);
            DealDamage(haka.CharacterCard, mdp, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

            //only first a turn
            QuickHPStorage(mdp, ra.CharacterCard);
            DealDamage(haka.CharacterCard, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //reset for next turn
            GoToNextTurn();
            QuickHPStorage(mdp, ra.CharacterCard);
            DealDamage(haka.CharacterCard, mdp, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

        }

        [Test()]
        public void TestHalberdFoamcore_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToPlayCardPhase(halberd);

            //we play out foamcore
            Card foamcore = GetCard("HalberdFoamcore");
            PlayCard(foamcore);
            AssertIsInPlay(foamcore);

            //If there are no Chemical Triggers in play, reduce damage dealt to the hero target with the lowest HP by 1.
            //haka has the lowest hitpoints
            QuickHPStorage(haka);
            DealDamage(baron, haka, 4, DamageType.Melee);
            //damage should have been reduced by 1
            QuickHPCheck(-3);

            //check that it is only for the lowest hp
            QuickHPStorage(ra);
            DealDamage(baron, ra, 4, DamageType.Melee);
            //damage should not have been reduced
            QuickHPCheck(-4);

            //check that the other trigger isn't in effect
            QuickHPStorage(mdp);
            DealDamage(haka.CharacterCard, mdp, 4, DamageType.Melee);
            //damage should not have been reduced
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestHalberdFoamcore_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToPlayCardPhase(halberd);

            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            //destroy omega
            Card omega = GetCardInPlay("HalberdOmega");
            DestroyCard(omega);

            //we play out foamcore
            Card foamcore = GetCard("HalberdFoamcore");
            PlayCard(foamcore);
            AssertIsInPlay(foamcore);

            //Otherwise, Reduce damage dealt to villain targets by 1.

            QuickHPStorage(mdp);
            DealDamage(haka.CharacterCard, mdp, 4, DamageType.Melee);
            //damage should  have been reduced by 1
            QuickHPCheck(-3);

            ////check that the other trigger isn't in effect
            //haka has the lowest hitpoints
            QuickHPStorage(haka);
            DealDamage(baron, haka, 4, DamageType.Melee);
            //damage should have not been reduced
            QuickHPCheck(-4);

        }

        [Test()]
        public void TestHalberdPlasma_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            //destroy mdp so baron blade is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            GoToPlayCardPhase(halberd);

            //we play out plasma
            Card plasma = GetCard("HalberdPlasma");
            PlayCard(plasma);
            AssertIsInPlay(plasma);

            //At the end of the environment turn, if there are no Chemical Triggers in play, this card deals the villain target with highest HP {H} energy damage.
            //highest hp hero is ra, highest villain is baron blade
            //H is 3
            QuickHPStorage(ra, baron);
            GoToEndOfTurn(halberd);
            //check that highest villain got dealt damage and highest hero did not
            QuickHPCheck(0, -3);

        }

        [Test()]
        public void TestHalberdPlasma_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            //destroy mdp so baron blade is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            GoToPlayCardPhase(halberd);

            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            //destroy omega
            Card omega = GetCardInPlay("HalberdOmega");
            DestroyCard(omega);

            //we play out plasma
            Card plasma = GetCard("HalberdPlasma");
            PlayCard(plasma);
            AssertIsInPlay(plasma);

            //Otherwise, this card deals the hero target with the highest HP {H} energy damage.
            //highest hp hero is ra, highest villain is baron blade
            //H is 3
            QuickHPStorage(ra, baron);
            GoToEndOfTurn(halberd);
            //check that highest hero got dealt damage and highest villain did not
            QuickHPCheck(-3, 0);

        }

        [Test()]
        public void TestHalberdZephyr_NoChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            //Set hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 10);

            //destroy mdp so baron blade is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            GoToPlayCardPhase(halberd);

            //we play out zephyr
            Card zephyr = GetCard("HalberdZephyr");
            PlayCard(zephyr);
            AssertIsInPlay(zephyr);

            //we play out siren to have another test subject
            //siren's redirection does not impact this test
            Card siren = GetCard("HalberdSiren");
            PlayCard(siren);
            AssertIsInPlay(siren);

            //set test subjects hitpoints so there is room to gain
            SetHitPoints(zephyr, 4);
            SetHitPoints(siren, 4);

            //At the end of the environment turn, each Test Subject regains 1HP.
            //If there are no Chemical Triggers in play, the hero with the lowest HP regains 3HP
            //lowest hero is haka
            QuickHPStorage(zephyr, siren, haka.CharacterCard, ra.CharacterCard);
            GoToEndOfTurn(halberd);
            QuickHPCheck(1, 1, 3, 0);
        }

        [Test()]
        public void TestHalberdZephyr_WithChemicalTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();


            Card mdp = GetCardInPlay("MobileDefensePlatform");
            //Set hitpoints to start
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(mdp, 5);

            GoToPlayCardPhase(halberd);

            //we put a chem trigger in play
            //this chem trigger plays omega
            //this chem trigger reduces damage dealt to subjects, so it doesn't impact this test
            Card chem = GetCard("HrCombatPheromones");
            PlayCard(chem);
            AssertIsInPlay(chem);

            //the chem trigger plays omega
            //destroy omega as it would impact this test
            Card omega = GetCardInPlay("HalberdOmega");
            DestroyCard(omega, baron.CharacterCard);

            //we play out zephyr
            Card zephyr = GetCard("HalberdZephyr");
            PlayCard(zephyr);
            AssertIsInPlay(zephyr);

            //we play out siren to have another test subject
            //siren's redirection does not impact this test
            Card siren = GetCard("HalberdSiren");
            PlayCard(siren);
            AssertIsInPlay(siren);

            //set test subjects hitpoints so there is room to gain
            SetHitPoints(zephyr, 4);
            SetHitPoints(siren, 4);

            //At the end of the environment turn, each Test Subject regains 1HP.
            //Otherwise, the villain target with the lowest HP regains 3HP.
            //lowest villain is mdp
            QuickHPStorage(zephyr, siren, mdp, baron.CharacterCard);
            GoToEndOfTurn(halberd);
            QuickHPCheck(1, 1, 3, 0);


        }

    }
}

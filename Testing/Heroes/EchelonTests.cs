using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cauldron.Echelon;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class EchelonTests : CauldronBaseTest
    {
        #region echelonhelperfunctions

        private const string DeckNamespace = "Cauldron.Echelon";

        private readonly DamageType DTM = DamageType.Melee;
        private Card MDP => GetCardInPlay("MobileDefensePlatform");

        #endregion
        [Test]
        public void TestEchelonLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(echelon);
            Assert.IsInstanceOf(typeof(EchelonCharacterCardController), echelon.CharacterCardController);

            Assert.AreEqual(27, echelon.CharacterCard.HitPoints);
        }
        [Test]
        public void TestEchelonDecklist()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            AssertHasKeyword("tactic", new[]
            {
                "AdvanceAndRegroup",
                "BreakThrough",
                "KnowYourEnemy",
                "PracticedTeamwork",
                "RemoteObservation",
                "RuthlessIntimidation",
                "StaggeredAssault",
                "SurpriseAttack"
            });

            AssertHasKeyword("ongoing", new[]
            {
                "AdvanceAndRegroup",
                "BreakThrough",
                "FindAWayIn",
                "KnowYourEnemy",
                "NeedAWayOut",
                "Overwatch",
                "PracticedTeamwork",
                "RemoteObservation",
                "RuthlessIntimidation",
                "StaggeredAssault",
                "SurpriseAttack"
            });

            AssertHasKeyword("equipment", new[]
            {
                "DatabaseUplink",
                "TeslaKnuckles",
                "TheKestrelMarkII"
            });

            AssertHasKeyword("limited", new[]
            {
                "FindAWayIn",
                "NeedAWayOut",
                "Overwatch",
                "TeslaKnuckles",
                "TheKestrelMarkII"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "CommandAndConquer",
                "FirstResponder",
                "StrategicDeployment"
            });
        }
        [Test]
        public void TestExtensibleTacticKeep([Values("KnowYourEnemy",
                                            "PracticedTeamwork",
                                            "RemoteObservation",
                                            "RuthlessIntimidation",
                                            "StaggeredAssault",
                                            "SurpriseAttack")] string tacticID)
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card tactic = PlayCard(tacticID);

            QuickHandStorage(echelon);
            GoToStartOfTurn(echelon);
            AssertIsInPlay(tactic);
            QuickHandCheck(-1);
        }
        [Test]
        public void TestExtensibleTacticDrop([Values("KnowYourEnemy",
                                            "PracticedTeamwork",
                                            "RemoteObservation",
                                            "RuthlessIntimidation",
                                            "StaggeredAssault",
                                            "SurpriseAttack")] string tacticID)
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //Practiced Teamwork is the only extensible tactic that doesn't draw when dropped 
            //(it gives you the draw up-front instead)
            int gain = tacticID == "PracticedTeamwork" ? 0 : 1;
            Card tactic = PlayCard(tacticID);

            DecisionDoNotSelectCard = SelectionType.DiscardCard;

            QuickHandStorage(echelon);

            GoToStartOfTurn(echelon);
            AssertInTrash(tactic);
            QuickHandCheck(gain);
        }

        [Test]
        public void TestNonExtensibleTacticSelfDestroy([Values("AdvanceAndRegroup", "BreakThrough")] string tacticID)
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card tactic = PlayCard(tacticID);

            AssertNoDecision();
            GoToStartOfTurn(echelon);
            AssertInTrash(tactic);
        }
        [Test]
        public void TestEchelonPower()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            QuickHPStorage(baron);
            UsePower(echelon);
            QuickHPCheck(-1);

            PlayCard("LivingForceField");
            UsePower(echelon);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestEchelonIncap1()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card flesh = PlayCard("FleshOfTheSunGod");
            UseIncapacitatedAbility(echelon, 0);
            AssertInTrash(flesh);
        }
        [Test]
        public void TestEchelonIncap2()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card punch = PutInHand("SuckerPunch");
            Card assault = PutInHand("AcceleratedAssault");
            Card goggles = PutOnDeck("HUDGoggles");
            Card limit = PutOnDeck("PushingTheLimits");
            DecisionYesNo = true;
            DecisionSelectTurnTaker = tachyon.TurnTaker;
            DecisionSelectCards = new Card[] { punch, goggles };

            QuickHandStorage(tachyon, ra);
            UseIncapacitatedAbility(echelon, 1);
            QuickHandCheckZero();
            AssertInTrash(punch, goggles);
            AssertInHand(assault, limit);
        }
        [Test]
        public void TestEchelonIncap2Optional()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card punch = PutInHand("SuckerPunch");
            Card assault = PutInHand("AcceleratedAssault");
            DecisionSelectTurnTaker = tachyon.TurnTaker;

            Card goggles = PutOnDeck("HUDGoggles");
            Card limit = PutOnDeck("PushingTheLimits");
            DecisionYesNo = false;

            QuickHandStorage(tachyon, ra);
            UseIncapacitatedAbility(echelon, 1);
            QuickHandCheckZero();
            AssertInHand(punch, assault);
            AssertOnTopOfDeck(limit);
            AssertOnTopOfDeck(goggles, 1);
        }
        [Test]
        public void TestEchelonIncap3()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card topOfDeck = GetTopCardOfDeck(baron);
            UseIncapacitatedAbility(echelon, 2);
            AssertOnBottomOfDeck(topOfDeck);
        }
        [Test]
        public void TestAdvanceAndRegroupEffect()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            PlayCard("AdvanceAndRegroup");
            SetHitPoints(echelon, 20);
            SetHitPoints(ra, 20);

            QuickHPStorage(ra, echelon);
            DecisionSelectCards = new Card[] { echelon.CharacterCard, ra.CharacterCard };

            DestroyCard(MDP);
            QuickHPCheck(0, 2);

            Card batt = PlayCard("BladeBattalion");
            DestroyCard(batt);
            QuickHPCheck(2, 0);

            Card traffic = PlayCard("TrafficPileup");
            DestroyCard(traffic);
            QuickHPCheck(2, 0);
        }
        [Test]
        public void TestBreakThroughEffect()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            DecisionYesNo = true;
            GoToPlayCardPhaseAndPlayCard(echelon, "BreakThrough");

            QuickHPStorage(baron);
            //must be owning player's turn
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-1);

            DealDamage(echelon, baron, 1, DTM);
            QuickHPCheck(-3);

            //only once
            DealDamage(echelon, baron, 1, DTM);
            QuickHPCheck(-1);

            //valid sources change with turn
            GoToStartOfTurn(ra);
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-3);

            //also works with non-character targets
            Card decoy = PlayCard("DecoyProjection");
            GoToStartOfTurn(visionary);
            DealDamage(decoy, baron, 1, DTM);
            QuickHPCheck(-3);
            DealDamage(visionary, baron, 1, DTM);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestCommandAndConquer()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            Card advance = PutOnDeck("AdvanceAndRegroup");
            Card tesla = PutOnDeck("TeslaKnuckles");
            Card bt = PutOnDeck("BreakThrough");

            QuickShuffleStorage(echelon);
            DecisionSelectCards = new Card[] { advance, baron.CharacterCard };

            //does not seem to be a way to distinguish top/bottom of same deck in a SelectLocationDecision
            //DecisionSelectLocation = new LocationChoice(echelon.TurnTaker.Deck, toBottom: true);
            PlayCard("CommandAndConquer");

            AssertIsInPlay(advance);
            AssertOnTopOfDeck(bt);
            QuickShuffleCheck(1);
            AssertNumberOfCardsAtLocation(echelon.TurnTaker.Revealed, 0);
        }
        [Test]
        public void TestDatabaseUplinkEndOfTurn()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PutOnDeck(echelon, echelon.HeroTurnTaker.Hand.Cards);

            Card advance = PutInHand("AdvanceAndRegroup");
            Card bt = PutInHand("BreakThrough");
            Card command = PutInHand("CommandAndConquer");

            Card kestrel = PutOnDeck("TheKestrelMarkII");
            PlayCard("DatabaseUplink");
            QuickHandStorage(echelon);
            DecisionSelectCard = advance;

            AssertNextDecisionChoices(new Card[] { advance, bt }, new Card[] { command });

            GoToEndOfTurn(echelon);
            QuickHandCheckZero();
            AssertInHand(kestrel);
            AssertInTrash(advance);
        }
        [Test]
        public void TestDatabaseUplinkEndOfTurnOptional()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            PutOnDeck(echelon, echelon.HeroTurnTaker.Hand.Cards);

            Card advance = PutInHand("AdvanceAndRegroup");
            Card bt = PutInHand("BreakThrough");
            Card command = PutInHand("CommandAndConquer");

            Card kestrel = PutOnDeck("TheKestrelMarkII");
            PlayCard("DatabaseUplink");
            QuickHandStorage(echelon);
            DecisionSelectCards = new Card[] { null, advance };

            AssertNextDecisionChoices(new Card[] { advance, bt }, new Card[] { command });

            GoToEndOfTurn(echelon);
            QuickHandCheckZero();
            AssertInHand(advance);
            AssertOnTopOfDeck(kestrel);
        }
        [Test]
        public void TestDatabaseUplinkPower()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card uplink = PlayCard("DatabaseUplink");

            //should be "put into play" effect
            PlayCard("HostageSituation");

            Card advance = PutInHand("AdvanceAndRegroup");
            Card bt = PutInHand("BreakThrough");
            Card command = PutInHand("CommandAndConquer");

            DecisionSelectCard = advance;

            AssertNextDecisionChoices(new Card[] { advance, bt }, new Card[] { command });

            UsePower(uplink);
            AssertIsInPlay(advance);
        }
        [Test]
        public void TestFindAWayInPhaseShifting()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card findAWayIn = GetCard(FindAWayInCardController.Identifier);

            GoToPlayCardPhase(echelon);
            PlayCard(findAWayIn);

            DecisionYesNo = true;

            GoToStartOfTurn(ra);
            GoToPlayCardPhase(ra);
            AssertPhaseActionCount(2);
            GoToUsePowerPhase(ra);
            AssertPhaseActionCount(null);
        }
        [Test]
        public void TestFindAWayInDestroyAndHeal()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card decoy = PlayCard("DecoyProjection");
            var interestingCards = new Card[] { echelon.CharacterCard, ra.CharacterCard, visionary.CharacterCard, decoy, MDP };
            SetHitPoints(interestingCards, 3);
            QuickHPStorage(interestingCards);

            Card wayin = PlayCard("FindAWayIn");

            AssertNoDecision();

            GoToStartOfTurn(echelon);
            AssertInTrash(wayin);
            QuickHPCheck(1, 1, 1, 1, 0);
        }

        [Test]
        public void TestFirstResponderReturnTactics()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card firstRespond = GetCard(FirstResponderCardController.Identifier);

            Card advance = PutInTrash("AdvanceAndRegroup");
            Card bt = PutInTrash("BreakThrough");
            Card command = PutInHand("CommandAndConquer");
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { command, advance, bt };

            GoToPlayCardPhase(echelon);
            PlayCard(firstRespond);
            AssertInTrash(command);
            AssertIsInPlay(advance, bt);
        }
        [Test]
        public void TestFirstResponderRedrawHand()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card advance = PutInTrash("AdvanceAndRegroup");
            Card bt = PutInTrash("BreakThrough");
            Card command = PutInHand("CommandAndConquer");
            DecisionSelectCards = new Card[] { advance, bt };
            DecisionsYesNo = new bool[] { false, true };
            int startingHand = GetNumberOfCardsInHand(echelon);

            PlayCard("FirstResponder");
            //the two tactics put there, the First Responder, and her hand
            AssertNumberOfCardsInTrash(echelon, startingHand + 3);

            AssertNumberOfCardsInHand(echelon, 4);

        }
        [Test]
        public void TestFirstResponderDoNothing()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card advance = PutInTrash("AdvanceAndRegroup");
            Card bt = PutInTrash("BreakThrough");
            Card command = PutInHand("CommandAndConquer");
            DecisionSelectCards = new Card[] { null, advance, bt };
            DecisionYesNo = false;
            Card respond = PutOnDeck("FirstResponder");
            QuickHandStorage(echelon);

            PlayCard(respond);
            QuickHandCheckZero();
            AssertInTrash(advance, bt, respond);
        }
        [Test]
        public void TestKnowYourEnemy()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            GoToPlayCardPhaseAndPlayCard(echelon, "KnowYourEnemy");
            QuickHandStorage(echelon);
            Card mdp = MDP;

            DealDamage(echelon, mdp, 12, DTM);
            QuickHandCheck(1);

            //only once per turn
            PlayCard(mdp);
            DealDamage(ra, mdp, 12, DTM);
            QuickHandCheckZero();

            GoToStartOfTurn(ra);

            //not by villains
            PlayCard(mdp);
            DealDamage(baron, mdp, 12, DTM);
            QuickHandCheckZero();

            //non-damage destroy counts
            PlayCard(mdp);
            Card gaze = PlayCard("WrathfulGaze");
            SetHitPoints(mdp, 1);
            UsePower(gaze);
            QuickHandCheck(1);
        }
        [Test]
        public void TestNeedAWayOutPhaseShifting()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card wayout = GetCard("NeedAWayOut");

            GoToPlayCardPhase(echelon);
            PlayCard(wayout);

            DecisionYesNo = true;

            GoToStartOfTurn(ra);
            GoToPlayCardPhase(ra);
            AssertPhaseActionCount(null);
            GoToUsePowerPhase(ra);
            AssertPhaseActionCount(2);
        }
        [Test]
        public void TestNeedAWayOutDestroyAndHeal()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card decoy = PlayCard("DecoyProjection");
            var interestingCards = new Card[] { echelon.CharacterCard, ra.CharacterCard, visionary.CharacterCard, decoy, MDP };
            SetHitPoints(interestingCards, 3);
            QuickHPStorage(interestingCards);

            Card wayout = PlayCard("NeedAWayOut");

            AssertNoDecision();

            GoToStartOfTurn(echelon);
            AssertInTrash(wayout);
            QuickHPCheck(1, 1, 1, 1, 0);
        }
        [Test]
        public void TestOverwatch()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();

            Card overwatch = PlayCard("Overwatch");
            DecisionsYesNo = new bool[] { false, true, true };
            PlayCard("Fortitude");

            UsePower(overwatch);
            QuickHPStorage(ra.CharacterCard, legacy.CharacterCard, MDP);

            //is optional
            DealDamage(MDP, ra, 1, DTM);
            QuickHPCheck(-1, 0, 0);

            //must deal damage
            DealDamage(MDP, legacy, 1, DTM);
            QuickHPCheckZero();

            //effect happens
            DealDamage(MDP, ra, 1, DTM);
            QuickHPCheck(-1, 0, -3);

            //only once
            DealDamage(MDP, ra, 1, DTM);
            QuickHPCheck(-1, 0, 0);
        }
        [Test]
        public void TestOverwatch_NonHeroDamageSource()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card overwatch = PlayCard("Overwatch");
            UsePower(overwatch);

            //Only if Non-Hero deals the damage
            QuickHPStorage(ra, legacy, baron);
            DealDamage(legacy, ra, 1, DTM);
            QuickHPCheck(-1, 0, 0);
        }
        [Test]
        public void TestOverwatch_DamageWhileDestroyed()
        {
            SetupGameController("TheEnnead", DeckNamespace, "Ra", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card firstDemigod = FindCardsWhere((Card c) => c.IsActiveEnneadCharacter && c.IsInPlayAndHasGameText).FirstOrDefault();
            Card overwatch = PlayCard("Overwatch");
            UsePower(overwatch);
            DestroyCard(overwatch);

            DecisionYesNo = true;
            SetHitPoints(firstDemigod, 2);
            DealDamage(firstDemigod, echelon, 1, DamageType.Melee);
            AssertFlipped(firstDemigod);
            Assert.IsFalse(firstDemigod.IsTarget);
        }
        [Test]
        public void TestPracticedTeamwork()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            Card practice = PutOnDeck("PracticedTeamwork");
            QuickHandStorage(echelon, ra, haka);
            QuickHPStorage(baron, echelon, ra, haka);
            PlayCard(practice);

            //Each player draws on entry
            QuickHandCheck(1, 1, 1);

            DealDamage(echelon, echelon, 1, DTM);
            DealDamage(ra, echelon, 1, DTM);
            DealDamage(haka, ra, 1, DTM);
            QuickHPCheckZero();

            DealDamage(echelon, baron, 1, DTM);
            DealDamage(baron, echelon, 1, DTM);
            QuickHPCheck(-1, -1, 0, 0);
        }
        [Test]
        public void TestRemoteObservation()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToPlayCardPhaseAndPlayCard(echelon, "RemoteObservation");

            GoToDrawCardPhase(echelon);
            AssertPhaseActionCount(2);
            QuickHandStorage(echelon);
            GoToEndOfTurn(echelon);
            QuickHandCheck(-1);

            GoToDrawCardPhase(ra);
            AssertPhaseActionCount(3);
            QuickHandStorage(ra);
            GoToEndOfTurn(ra);
            QuickHandCheck(-1);
        }
        [Test]
        public void TestRuthlessIntimidation()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Haka", "Megalopolis");
            StartGame();

            PlayCard("RuthlessIntimidation");
            Card mdp = MDP;

            //boost damage to lowest HP
            QuickHPStorage(mdp);
            DealDamage(echelon, mdp, 1, DTM);
            QuickHPCheck(-2);

            //regardless of source
            Card traffic = PlayCard("TrafficPileup");
            DealDamage(traffic, mdp, 1, DTM);
            QuickHPCheck(-2);

            DestroyCard(mdp);

            Card redist = PlayCard("ElementalRedistributor");
            QuickHPStorage(redist);

            //allows deciding on tie
            DecisionYesNo = false;
            DealDamage(echelon, redist, 1, DTM);
            QuickHPCheck(-1);

            DealDamage(echelon, redist, 1, DTM);
            QuickHPCheck(-2);

            //counts non-villain
            SetHitPoints(traffic, 4);
            DealDamage(echelon, redist, 1, DTM);
            QuickHPCheck(-1);

            //does not count non-villain
            SetHitPoints(haka, 1);
            DestroyCard(traffic);
            DealDamage(echelon, redist, 1, DTM);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestStaggeredAssault()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Haka", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            GoToPlayCardPhaseAndPlayCard(echelon, "StaggeredAssault");
            QuickHPStorage(baron);
            DealDamage(ra, baron, 2, DTM);
            QuickHPCheck(-3);

            //once per turn
            QuickHPStorage(baron);
            DealDamage(ra, baron, 2, DTM);
            QuickHPCheck(-2);

            GoToStartOfTurn(ra);

            //does not trigger on less-than-two
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-1);
            DealDamage(ra, baron, 2, DTM);
            QuickHPCheck(-3);

            //does not trigger if damage was prevented
            GoToStartOfTurn(haka);
            AddImmuneToNextDamageEffect(baron, true, false);
            QuickHPUpdate();
            DealDamage(haka, baron, 3, DTM);
            QuickHPCheckZero();

            DecisionSelectTargets = new Card[] { null, baron.CharacterCard };

            GoToStartOfTurn(visionary);
            Card decoy = PlayCard("DecoyProjection");

            //optional, and trigger on non-character
            DealDamage(decoy, baron, 2, DTM);
            QuickHPCheck(-2);
            DealDamage(decoy, baron, 2, DTM);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestStrategicDeployment()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            Card obs = PutOnDeck("RemoteObservation");
            DecisionSelectCard = obs;

            QuickShuffleStorage(echelon);
            PlayCard("StrategicDeployment");
            AssertIsInPlay(obs);
            QuickShuffleCheck(1);
        }
        [Test]
        public void TestSurpriseAttackBasic()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            PlayCard("SurpriseAttack");

            QuickHPStorage(baron, visionary);
            UsePower(ra);
            QuickHPCheck(-3, 0);

            DecisionSelectDamageTypes = new DamageType?[] { DamageType.Psychic, DamageType.Fire };

            PlayCard("ElementalRedistributor");

            //when picking psychic, shouldn't get redirected
            UsePower(ra);
            QuickHPCheck(-3, 0);

            //now that we are picking fire, should redirect the damage to Visionary
            UsePower(ra);
            QuickHPCheck(0, -3);
        }
        [Test]
        public void TestSurpriseAttackLastingStatusEffect()
        {
            SetupGameController("GrandWarlordVoss", DeckNamespace, "Ra", "VoidGuardMainstay/VoidGuardRoadWarriorMainstayCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card weaver = PlayCard("GeneBoundPsiWeaver");
            Card spaceship = PlayCard("TcfConqueror");
            PlayCard("SurpriseAttack");

            DecisionYesNo = true;
            QuickHPStorage(weaver, spaceship);

            UsePower(voidMainstay);
            //damage should be psychic

            DealDamage(spaceship, voidMainstay, 1, DTM);
            QuickHPCheck(0, -2);
            DealDamage(weaver, voidMainstay, 1, DTM);
            QuickHPCheckZero();

            DecisionYesNo = false;
            //can choose to let it be melee, but only at power-use time
            UsePower(voidMainstay);
            DecisionYesNo = true;

            DealDamage(spaceship, voidMainstay, 1, DTM);
            DealDamage(weaver, voidMainstay, 1, DTM);
            QuickHPCheck(-2, -2);

            //both effects should continue dealing damage
            DestroyCard(weaver);
            DestroyCard(spaceship);
            QuickHPStorage(voss);
            DealDamage(voss, voidMainstay, 1, DTM);
            QuickHPCheck(-4);
        }
        [Test]
        public void TestSurpriseAttackMultipleTypeChangers()
        {
            SetupGameController("GrandWarlordVoss", DeckNamespace, "Ra", "Tempest", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("SurpriseAttack");
            PlayCard("ImbuedFire");

            UsePower(tempest);
        }
        [Test]
        public void TestSurpriseAttackStacksWithSelfNicely()
        {
            SetupGameController("GrandWarlordVoss", "Cauldron.Echelon", "Ra", "Tempest", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("SurpriseAttack");
            PlayCard("SurpriseAttack");
            DecisionSelectDamageType = DamageType.Projectile;
            Card weaver = PlayCard("GeneBoundPsiWeaver");

            UsePower(tempest);
            AssertInTrash(weaver);
        }
        [Test]
        public void TestSurpriseAttackInterruptedByOtherPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Echelon", "Tempest", "ChronoRanger", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card turret = PlayCard("PoweredRemoteTurret");
            Card traffic = PlayCard("TrafficPileup");
            PlayCard("BacklashField");
            PlayCard("TheUltimateTarget");
            PlayCard("SurpriseAttack");
            QuickHPStorage(baron.CharacterCard, turret, traffic);

            //Tempest hits Blade for 1 + 1; Backlash Field hits Tempest; 
            //Ultimate Target lets Chrono shoot Blade for 1 + 1 (Surprise) + 1 (Target)
            //Tempest continues to Turret and Traffic for 1 + 1 each
            UsePower(tempest);
            QuickHPCheck(-5, -2, -2);
        }
        [Test()]
        public void TestSurpriseAttackWithVoidBelter()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "VoidGuardMainstay", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("SurpriseAttack");
            Card belter = PlayCard("VoidBelter");

            QuickHPStorage(baron);
            UsePower(belter);

            //3 + 1 from power, 2 + 2 from on destruction
            QuickHPCheck(-8);
        }
        [Test]
        public void TestTeslaKnucklesPower()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            Card turret = PlayCard("PoweredRemoteTurret");
            Card traffic = PlayCard("TrafficPileup");
            Card batt = PlayCard("BladeBattalion");
            Card knuckles = PlayCard("TeslaKnuckles");

            QuickHPStorage(baron.CharacterCard, turret, batt, traffic, echelon.CharacterCard, ra.CharacterCard, visionary.CharacterCard);
            UsePower(knuckles);
            QuickHPCheck(-1, -1, -1, -1, 0, 0, 0);
        }
        [Test]
        public void TestTeslaKnucklesEndOfTurn([Values(-1, 0, 1, 2, 3, 4)] int numDestroys)
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            Card advance = GetCard("AdvanceAndRegroup");
            GoToPlayCardPhase(echelon);
            PlayCard("TeslaKnuckles");

            int damage = numDestroys;
            //-1 tests that the damage is optional
            if (numDestroys == -1)
            {
                DecisionSelectCards = new Card[] { null, echelon.CharacterCard };
                damage = 0;
                numDestroys = 1;
            }

            for (int i = 0; i < numDestroys; i++)
            {
                PlayCard(advance);
                DestroyCard(advance);
            }

            QuickHPStorage(baron);
            GoToEndOfTurn(echelon);
            QuickHPCheck(-damage);
        }
        [Test]
        public void TestTheKestrelMarkIIPower1()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            Card kestrel = PlayCard("TheKestrelMarkII");
            SetHitPoints(echelon, 20);
            QuickHPStorage(baron, echelon);
            UsePower(kestrel, 0);
            QuickHPCheck(-1, 2);
        }
        [Test]
        public void TestTheKestrelMarkIIPower2()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            Card kestrel = PlayCard("TheKestrelMarkII");

            QuickHandStorage(echelon, ra, visionary);
            DecisionSelectTurnTaker = ra.TurnTaker;
            UsePower(kestrel, 1);
            AssertInTrash(kestrel);
            QuickHandCheck(0, 2, 0);
        }
    }
}

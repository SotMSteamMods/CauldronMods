using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Cricket;

namespace CauldronTests
{
    [TestFixture()]
    public class CricketVariantTests : CauldronBaseTest
    {
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(cricket.CharacterCard, 1);
            DealDamage(villain, cricket, 2, DamageType.Melee);
        }

        [Test()]
        public void TestLoadFirstResponseCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(FirstResponseCricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(28, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestFRCricketInnatePower()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //{Cricket} deals 2 targets 1 sonic damage each. You may use a power now.
            //Second power will be GrasshopperKick
            var card = PlayCard("GrasshopperKick");
            QuickHPStorage(choke, cricket, legacy);
            DecisionSelectCards = new Card[] { choke.CharacterCard, cricket.CharacterCard, legacy.CharacterCard };

            UsePower(cricket);
            QuickHPCheck(-1, -1, -2);
        }

        [Test()]
        public void TestFRCricketIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            Card ring = PutInHand("TheLegacyRing");
            DecisionSelectCard = ring;

            //One player may play a card now.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(ring);
        }

        [Test()]
        public void TestFRCricketIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            PutInTrash("PlummetingMonorail");
            PutInTrash("HostageSituation");
            PutInTrash("RooftopCombat");

            //Shuffle the environment trash into the environment deck.
            UseIncapacitatedAbility(cricket, 1);
            AssertNumberOfCardsInTrash(env, 0);
            AssertNumberOfCardsInDeck(env, 15);
        }

        [Test()]
        public void TestFRCricketIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/FirstResponseCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            SetHitPoints(apostate, 17);
            SetHitPoints(legacy, 17);
            //1 hero target regains 2 HP.
            QuickHPStorage(apostate, legacy);
            UseIncapacitatedAbility(cricket, 2);
            QuickHPCheck(0, 2);
        }

        [Test()]
        public void TestLoadRenegadeCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(RenegadeCricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(26, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestRenegadeCricketInnatePower()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card staff = PutOnDeck("TelescopingStaff");
            Card ring = PutOnDeck("TheLegacyRing");
            //Reveal the top card of a hero deck. You may discard a card to put it into play, otherwise put it into that player's hand.
            QuickHandStorage(cricket);
            UsePower(cricket);
            QuickHandCheck(-1);
            AssertIsInPlay(staff);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            QuickHandStorage(cricket);
            UsePower(cricket);
            QuickHandCheck(0);
            AssertInHand(legacy, ring);


        }

        [Test()]
        public void TestRenegadeCricketInnatePower_EnhancedSenses()
        {
            SetupGameController("Cauldron.Anathema", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("EnhancedSenses");

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            Card ring = PutOnDeck("TheLegacyRing");
            //Reveal the top card of a hero deck. You may discard a card to put it into play, otherwise put it into that player's hand.
            QuickHandStorage(cricket);
            QuickHPStorage(cricket, legacy);
            UsePower(cricket);
            QuickHandCheck(-1);
            AssertIsInPlay(ring);
            QuickHPCheck(0, -1);

           


        }

        [Test()]
        public void TestRenegadeCricketInnatePower_DarkMind()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective", scionIdentifiers: new List<string>() { "DarkMindCharacter" });
            StartGame();


            SwitchBattleZone(cricket);
            SwitchBattleZone(legacy);

            AssertBattleZone(mindScion, bzTwo);
            AssertBattleZone(cricket, bzTwo);
            AssertBattleZone(legacy, bzTwo);


            Card hearing = PutOnDeck("EnhancedHearing");
            Card evolution = PutOnDeck("NextEvolution");
            //Reveal the top card of a hero deck. You may discard a card to put it into play, otherwise put it into that player's hand.
            QuickHandStorage(cricket);
            UsePower(cricket);
            QuickHandCheck(-1);
            AssertIsInPlay(hearing);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            QuickHandStorage(cricket);
            UsePower(cricket);
            QuickHandCheck(0);
            AssertInHand(legacy, evolution);

            ResetDecisions();
            DecisionSelectLocation = new LocationChoice(cricket.TurnTaker.Deck);
            MoveAllCards(cricket, cricket.TurnTaker.Deck, cricket.TurnTaker.Trash);
            UsePower(cricket);


        }

        [Test()]
        public void TestRenegadeCricketIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(baron);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(baron.TurnTaker.Deck), new LocationChoice(legacy.TurnTaker.Deck), new LocationChoice(env.TurnTaker.Deck) };

            Card battalion = PutOnDeck("BladeBattalion");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(battalion);

            Card ring = PutOnDeck("TheLegacyRing");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(ring);

            Card defender = PutOnDeck("SeismicDefender");
            //Select a deck and put its top card into play.
            UseIncapacitatedAbility(cricket, 0);
            AssertIsInPlay(defender);
        }

        [Test()]
        public void TestRenegadeCricketIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //Discard the top 2 cards of the villain deck.
            UseIncapacitatedAbility(cricket, 1);
            AssertNumberOfCardsInTrash(apostate, 2);
        }

        [Test()]
        public void TestRenegadeCricketIncap2_MultipleVillainDecks()
        {
            SetupGameController("KaargraWarfang", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(warfang);

            //Discard the top 2 cards of the villain deck.
            UseIncapacitatedAbility(cricket, 1);
            AssertNumberOfCardsInTrash(warfang, 2);
        }

        [Test()]
        public void TestRenegadeCricketIncap3()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/RenegadeCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            Card phlange = PutInTrash("ArborealPhalanges");
            Card ring = PutInTrash("TheLegacyRing");
            Card defender = PutInTrash("SeismicDefender");

            //Shuffle 1 card from a trash back into its deck.
            QuickShuffleStorage(akash.TurnTaker.Deck);
            UseIncapacitatedAbility(cricket, 2);
            QuickShuffleCheck(0);//this adds however much is in the shuffle storage, so this is actually checking that there has been 1 shuffle
            QuickShuffleStorage(legacy.TurnTaker.Deck);
            UseIncapacitatedAbility(cricket, 2);
            QuickShuffleCheck(0);
            QuickShuffleStorage(env.TurnTaker.Deck);
            UseIncapacitatedAbility(cricket, 2);
            QuickShuffleCheck(0);

            AssertInDeck(new Card[] { phlange, ring, defender });
        }

        [Test()]
        public void TestLoadWastelandRoninCricket()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(WastelandRoninCricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(28, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestWastelandRoninCricketInnatePower()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Increase damage dealt by {Cricket} during your next turn by 1. {Cricket} may deal 1 target 1 sonic damage.

            //{Cricket} may deal 1 target 1 sonic damage.
            QuickHPStorage(choke);
            UsePower(cricket);
            QuickHPCheck(-1);

            //{Cricket} may deal 1 target 1 sonic damage.
            //Increase damage dealt by {Cricket} during your next turn by 1.
            GoToStartOfTurn(cricket);
            QuickHPStorage(choke, cricket);
            DealDamage(cricket, choke, 1, DamageType.Sonic);
            QuickHPCheck(-2, 0);

            //only cricket
            DealDamage(choke, cricket, 1, DamageType.Sonic);
            QuickHPCheck(0, -1);

            //only the next turn
            GoToStartOfTurn(cricket);
            QuickHPStorage(choke);
            DealDamage(cricket, choke, 1, DamageType.Sonic);
            QuickHPCheck(-1);
        }


        [Test()]
        public void TestWastelandRoninCricketInnatePowerStacks()
        {
            SetupGameController("Chokepoint", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Increase damage dealt by {Cricket} during your next turn by 1. {Cricket} may deal 1 target 1 sonic damage.

            //{Cricket} may deal 1 target 1 sonic damage.
            QuickHPStorage(choke);
            UsePower(cricket);
            QuickHPCheck(-1);

            QuickHPStorage(choke);
            UsePower(cricket);
            QuickHPCheck(-1);

            //{Cricket} may deal 1 target 1 sonic damage.
            //Increase damage dealt by {Cricket} during your next turn by 1 x 2.
            GoToStartOfTurn(cricket);
            QuickHPStorage(choke, cricket);
            DealDamage(cricket, choke, 1, DamageType.Sonic);
            QuickHPCheck(-3, 0);

            //only cricket
            DealDamage(choke, cricket, 1, DamageType.Sonic);
            QuickHPCheck(0, -1);

            //only the next turn
            GoToStartOfTurn(cricket);
            QuickHPStorage(choke);
            DealDamage(cricket, choke, 1, DamageType.Sonic);
            QuickHPCheck(-1);
        }


        [Test()]
        public void TestWastelandRoninCricketInnatePower_BreakingTheRules()
        {
            SetupGameController("WagerMaster", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
                        
            DestroyNonCharacterVillainCards();
            //losingtothe odds causes a game over mid test, banish it.
            PutInTrash("LosingToTheOdds", "LosingToTheOdds", "LosingToTheOdds");

            PlayCard("BreakingTheRules");

            //Increase damage dealt by {Cricket} during your next turn by 1. {Cricket} may deal 1 target 1 sonic damage.

            //{Cricket} may deal 1 target 1 sonic damage.
            DecisionDoNotSelectCard = SelectionType.SelectTarget;
            UsePower(cricket);

            //{Cricket} may deal 1 target 1 sonic damage.
            //Increase damage dealt by {Cricket} during your next turn by 1.
            GoToEndOfTurn(cricket);
            QuickHPStorage(bunker);
            DealDamage(cricket, bunker, 1, DamageType.Sonic);
            QuickHPCheck(-2);



            //only the next turn
            GoToEndOfTurn(cricket);
            QuickHPStorage(bunker);
            DealDamage(cricket, bunker, 1, DamageType.Sonic);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap1()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            DecisionSelectTurnTakers = new TurnTaker[] { bunker.TurnTaker, scholar.TurnTaker };
            SetHitPoints(scholar, 17);

            //One hero may use a power now.
            QuickHandStorage(bunker);
            QuickHPStorage(scholar);
            UseIncapacitatedAbility(cricket, 0);
            UseIncapacitatedAbility(cricket, 0);
            QuickHandCheck(1);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Bunker", "Legacy", "MrFixer", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            UseIncapacitatedAbility(cricket, 1);
            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_EdgeCase4()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Bunker", "Legacy", "MrFixer", "TombOfAnubis");
            StartGame();
            SetupIncap(apostate);

            SetHitPoints(bunker.CharacterCard, 10);

            Card idol = PlayCard("IdolOfAnput");
            Card trial = PlayCard("SpikeTrap");
            DestroyCard(trial, legacy.CharacterCard);

            DecisionSelectCard = idol;
            UseIncapacitatedAbility(cricket, 1);
            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            QuickHPStorage(bunker);
            UsePower(idol);
            QuickHPCheck(4);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_EdgeCase3()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Ra", "VoidGuardDrMedico", "MrFixer", "TombOfAnubis");
            StartGame();
            SetupIncap(apostate);

            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(fixer.CharacterCard, 10);

            PlayCard("SecondOpinion");

            Card idol = PlayCard("IdolOfAnput");
            Card trial = PlayCard("SpikeTrap");
            Card trial2 = PlayCard("SwarmOfScarabs");

            SetHitPoints(trial2, 1);
            DecisionSelectCards = new Card[] { ra.CharacterCard, idol, trial2 };
            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, fixer.TurnTaker };
            DecisionYesNo = true;
            DestroyCard(trial, fixer.CharacterCard);

            UsePower(voidMedico);

            UseIncapacitatedAbility(cricket, 1);
            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.

            QuickHPStorage(ra, fixer);
            UsePower(idol);
            QuickHPCheck(0, 2);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_EdgeCase5()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "AbsoluteZero", "Legacy", "MrFixer", "TombOfAnubis");
            StartGame();
            SetupIncap(apostate);

            SetHitPoints(az.CharacterCard, 1);

            DecisionSelectCard = az.CharacterCard;
            UseIncapacitatedAbility(cricket, 1);

            UsePower(az);
            AssertIncapacitated(az);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_EdgeCase6()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "AbsoluteZero", "Luminary", "MrFixer", "TombOfAnubis");
            StartGame();
            SetupIncap(apostate);

            Card turret = PlayCard("RegressionTurret");
            DecisionSelectCard = turret;
            UseIncapacitatedAbility(cricket, 1);

            DestroyCard(turret, luminary.CharacterCard);
            PlayCard(turret);

            DecisionSelectTargets = new Card[] { fixer.CharacterCard, null, fixer.CharacterCard, null };
            QuickHPStorage(fixer);
            UsePower(turret);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");

            StartGame();
            SetupIncap(oblivaeon);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "Bunker" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Bunker";
            RunActiveTurnPhase();

            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 2;
            DecisionIncapacitatedAbilityIndex = 1;
            DecisionSelectCard = bunker.CharacterCard;
            RunActiveTurnPhase();

            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(2);

        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_Oblivaeon_EdgeCase1()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");

            StartGame();
            SetupIncap(oblivaeon);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "Bunker" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Bunker";
            RunActiveTurnPhase();

            Card flakCannon = PlayCard("FlakCannon");

            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 2;
            DecisionIncapacitatedAbilityIndex = 1;
            DecisionSelectCard = flakCannon;
            RunActiveTurnPhase();

            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            DecisionSelectTarget = legacy.CharacterCard;
            QuickHPStorage(legacy);
            UsePower(flakCannon);
            QuickHPCheck(-6);

        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_Oblivaeon_EdgeCase2()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");

            StartGame();
            SetupIncap(oblivaeon);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "Bunker" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Bunker";
            RunActiveTurnPhase();

            MoveCard(oblivaeon, "CreateContraption", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));
            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 0;
            RunActiveTurnPhase();
            Card hairdryer = GetCardInPlay("CreateContraption");
            FlipCard(hairdryer);
            

            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 2;
            DecisionIncapacitatedAbilityIndex = 1;
            DecisionSelectCard = hairdryer;
            RunActiveTurnPhase();

            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            DecisionSelectTargets = new Card[] { legacy.CharacterCard, haka.CharacterCard, legacy.CharacterCard, haka.CharacterCard };
            QuickHPStorage(legacy, haka);
            UsePower(hairdryer);
            QuickHPCheck(-12, -12);

        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_Oblivaeon_EdgeCase6()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");

            StartGame();
            SetupIncap(oblivaeon);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "Bunker" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Bunker";
            RunActiveTurnPhase();

            MoveCard(oblivaeon, "CreateContraption", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));
            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 0;
            RunActiveTurnPhase();
            Card hairdryer = GetCardInPlay("CreateContraption");
            FlipCard(hairdryer);


            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 2;
            DecisionIncapacitatedAbilityIndex = 1;
            DecisionSelectCard = hairdryer;
            RunActiveTurnPhase();

            DealDamage(oblivaeon, bunker, 10000, DamageType.Fire, isIrreducible: true);

            GoToAfterEndOfTurn();
            DecisionSelectFromBoxIdentifiers = new string[] { "AbsoluteZero" };
            DecisionSelectFromBoxTurnTakerIdentifier = "AbsoluteZero";
            RunActiveTurnPhase();

            ResetDecisions();

            GoToBeforeStartOfTurn(az);
            DecisionSelectFunction = 3;
            RunActiveTurnPhase();

            PlayCard(hairdryer);
            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            DecisionSelectTargets = new Card[] { legacy.CharacterCard, haka.CharacterCard, legacy.CharacterCard, haka.CharacterCard };
            QuickHPStorage(legacy, haka);
            UsePower(hairdryer);
            QuickHPCheck(-12, -12);

        }

        [Test()]
        public void TestWastelandRoninCricketIncap2_Oblivaeon_Rewind()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");

            StartGame();
            Card crick = cricket.CharacterCard;
            SetupIncap(oblivaeon);
            MoveCard(oblivaeon, "ScatterSlaughter", oblivaeon.TurnTaker.FindSubDeck("ScionDeck"));
            MoveCards(oblivaeon, FindCardsWhere((Card c) => c.Identifier == "AeonThrall"), oblivaeon.TurnTaker.FindSubDeck("AeonMenDeck"));

            //Replace Cricket with Bunker
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "BunkerCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Bunker";
            RunActiveTurnPhase();

            //Use incap ability and select Motivational Charge
            ResetDecisions();
            Card charge = PlayCard("MotivationalCharge");
            DecisionSelectCard = charge;
            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 2;
            DecisionIncapacitatedAbilityIndex = 1;
            RunActiveTurnPhase();

            //Check that Motivational Charge gets used twice
            DecisionSelectTarget = oblivaeon.CharacterCard;
            QuickHPStorage(oblivaeon);
            UsePower(charge);
            QuickHPCheck(-4);

            GoToEndOfTurn(oblivaeon);

            //Reload the game
            SaveAndLoad(GameController);

            //This time, select Mere
            ResetDecisions();
            Card mere = PlayCard("Mere");
            DecisionSelectCard = mere;
            GoToBeforeStartOfTurn(bunker);
            DecisionSelectFunction = 2;
            DecisionIncapacitatedAbilityIndex = 1;
            RunActiveTurnPhase();

            //Check that Mere gets used twice
            DecisionSelectTarget = oblivaeon.CharacterCard;
            QuickHPStorage(oblivaeon);
            UsePower(mere);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap2CosmicWeapon()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "CaptainCosmic", "Legacy", "MrFixer", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            DecisionNextToCard = cosmic.CharacterCard;
            var weapon = PlayCard("CosmicWeapon");

            DecisionSelectCard = cosmic.CharacterCard;
            DecisionSelectPower = cosmic.CharacterCard;
            DecisionSelectPowerIndex = 1;
            UseIncapacitatedAbility(cricket, 1);
            //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
            QuickHPStorage(apostate);
            UsePower(cosmic, 1);
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestWastelandRoninCricketIncap3()
        {
            SetupGameController("AkashBhuta", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(akash);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(akash.TurnTaker.Deck), new LocationChoice(legacy.TurnTaker.Deck), new LocationChoice(env.TurnTaker.Deck) };

            Card akashBottom = GetBottomCardOfDeck(akash);
            Card legacyBottom = GetBottomCardOfDeck(legacy);
            Card envBottom = GetBottomCardOfDeck(env);
            //Move the bottom card of a deck to the top.
            UseIncapacitatedAbility(cricket, 2);
            UseIncapacitatedAbility(cricket, 2);
            UseIncapacitatedAbility(cricket, 2);

            AssertOnTopOfDeck(akashBottom);
            AssertOnTopOfDeck(legacyBottom);
            AssertOnTopOfDeck(envBottom);
        }
    }
}

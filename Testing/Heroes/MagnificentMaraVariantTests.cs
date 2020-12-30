using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.MagnificentMara;
using System.Collections;

namespace CauldronTests
{
    [TestFixture()]
    public class MagnificentMaraVariantTests : BaseTest
    {
        #region MaraHelperFunctions
        protected HeroTurnTakerController mara { get { return FindHero("MagnificentMara"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(mara.CharacterCard, 1);
            DealDamage(villain, mara, 2, DamageType.Melee);
        }

        protected DamageType DTM => DamageType.Melee;

        protected Card MDP { get { return FindCardInPlay("MobileDefensePlatform"); } }

        private string MessageTerminator = "There should have been no other messages.";
        protected void CheckFinalMessage()
        {
            GameController.ExhaustCoroutine(GameController.SendMessageAction(MessageTerminator, Priority.High, null));
        }
        #endregion maraHelperFunctions

        [Test]
        public void TestMOSSMaraLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mara);
            Assert.IsInstanceOf(typeof(MinistryOfStrategicScienceMagnificentMaraCharacterCardController), mara.CharacterCardController);

            Assert.AreEqual(25, mara.CharacterCard.HitPoints);
        }
        [Test]
        public void TestMOSSMaraPower()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectTargets = new Card[] { bunker.CharacterCard, baron.CharacterCard };

            QuickHPStorage(baron.CharacterCard, bunker.CharacterCard, MDP);

            DealDamage(bunker, MDP, 1, DTM);
            QuickHPCheck(0, 0, -1);

            UsePower(mara);
            DealDamage(bunker, MDP, 1, DTM);
            //bunker takes 2 from power, MDP takes 1 + 1 boost
            QuickHPCheck(0, -2, -2);

            UsePower(mara);
            DealDamage(baron, bunker, 1, DTM);
            //should not require damage to go through for the boost to happen
            QuickHPCheck(0, -2, 0);

            //wears off on Mara's turn
            GoToStartOfTurn(mara);
            DealDamage(bunker, MDP, 1, DTM);
            DealDamage(baron, bunker, 1, DTM);
            QuickHPCheck(0, -1, -1);
        }
        [Test]
        public void TestMOSSMaraIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            AssertIncapLetsHeroDrawCard(mara, 0, legacy, 1);
        }

        [Test]
        public void TestMOSSMaraIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            DestroyCard(MDP);

            QuickHPStorage(bunker, baron);
            UseIncapacitatedAbility(mara, 1);
            //does not affect villains
            DealDamage(bunker, baron, 3, DTM);
            QuickHPCheck(0, -3);

            DealDamage(baron, bunker, 3, DTM);
            QuickHPCheck(-1, 0);

            //only happens once
            DealDamage(baron, bunker, 3, DTM);
            QuickHPCheck(-3, 0);
        }

        [Test]
        public void TestMOSSMaraIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            Card fort = PlayCard("Fortitude");
            Card lash = PlayCard("BacklashField");
            
            AssertNumberOfStatusEffectsInPlay(0);
            UseIncapacitatedAbility(mara, 2);
            AssertNumberOfStatusEffectsInPlay(1);

            //does not affect villain cards
            DestroyCard(lash);
            AssertInTrash(lash);

            DestroyCard(fort);
            AssertInHand(fort);

            AssertNumberOfStatusEffectsInPlay(0);

            //only happens once
            PlayCard(fort);
            DestroyCard(fort);
            AssertInTrash(fort);
        }


        [Test]
        public void TestMOSSMaraIncap3Stacking()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            Card fort = PlayCard("Fortitude");
            Card lash = PlayCard("BacklashField");

            AssertNumberOfStatusEffectsInPlay(0);
            UseIncapacitatedAbility(mara, 2);
            AssertNumberOfStatusEffectsInPlay(1);
            UseIncapacitatedAbility(mara, 2);
            AssertNumberOfStatusEffectsInPlay(1);
            
            DestroyCard(fort);
            AssertInHand(fort);

            AssertNumberOfStatusEffectsInPlay(0);

            //only happens once
            PlayCard(fort);
            DestroyCard(fort);
            AssertInTrash(fort);
        }

        [Test]
        public void TestPastMaraLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mara);
            Assert.IsInstanceOf(typeof(PastMagnificentMaraCharacterCardController), mara.CharacterCardController);

            Assert.AreEqual(24, mara.CharacterCard.HitPoints);
        }
        [Test]
        public void TestPastMaraPowerNoChoicesPossible()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);

            AssertNextMessages($"No heroes could play relics or return relics from their trash, so {mara.TurnTaker.Name} will draw a card.", MessageTerminator);
            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(1);

            CheckFinalMessage();
        }
        [Test]
        public void TestPastMaraPowerTakeBothActions()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);
            PutOnDeck(fanatic, fanatic.HeroTurnTaker.Hand.Cards);

            Card sword = PutInHand("Absolution");
            Card crystal = PutInTrash("DowsingCrystal");

            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(0);
            AssertIsInPlay(sword);
            AssertOnTopOfDeck(crystal);
        }
        [Test]
        public void TestPastMaraPowerSkipBothActions()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);
            PutOnDeck(fanatic, fanatic.HeroTurnTaker.Hand.Cards);

            Card sword = PutInHand("Absolution");
            Card crystal = PutInTrash("DowsingCrystal");
            Card form = PutOnDeck("MortalFormToEnergy");

            DecisionSelectCards = new Card[] {form, null, null, null};
            PlayCard("KeepMoving");

            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(1);
            AssertInHand(sword);
            AssertInTrash(crystal);
        }
        [Test]
        public void TestPastMaraPowerOnlyPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);
            PutOnDeck(fanatic, fanatic.HeroTurnTaker.Hand.Cards);

            Card sword = PutInHand("Absolution");
            Card crystal = PutInTrash("DowsingCrystal");
            Card form = PutOnDeck("MortalFormToEnergy");

            DecisionSelectCards = new Card[] { form, null, sword, null };
            PlayCard("KeepMoving");

            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(0);
            AssertIsInPlay(sword);
            AssertInTrash(crystal);
        }
        [Test]
        public void TestPastMaraPowerOnlyReturn()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);
            PutOnDeck(fanatic, fanatic.HeroTurnTaker.Hand.Cards);

            Card sword = PutInHand("Absolution");
            Card crystal = PutInTrash("DowsingCrystal");
            Card form = PutOnDeck("MortalFormToEnergy");

            DecisionSelectCards = new Card[] { form, null, null, crystal };
            PlayCard("KeepMoving");

            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(0);
            AssertInHand(sword);
            AssertOnTopOfDeck(crystal);
        }
        [Test]
        public void TestPastMaraPowerCannotBothBeSameHero()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);
            PutOnDeck(fanatic, fanatic.HeroTurnTaker.Hand.Cards);

            Card kalpak = PutInHand("KalpakOfMysteries");
            Card crystal = PutInTrash("DowsingCrystal");

            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(-1);
            AssertIsInPlay(kalpak);
            AssertInTrash(crystal);
        }
        [Test]
        public void TestPastMaraPowerSameHeroCanSkipPlayToReturn()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            PutOnDeck(mara, mara.HeroTurnTaker.Hand.Cards);
            PutOnDeck(fanatic, fanatic.HeroTurnTaker.Hand.Cards);

            Card kalpak = PutInHand("KalpakOfMysteries");
            Card crystal = PutInTrash("DowsingCrystal");

            DecisionSelectCards = new Card[] { null, crystal };
            QuickHandStorage(mara);
            UsePower(mara);
            QuickHandCheck(0);
            AssertInHand(kalpak);
            AssertOnTopOfDeck(crystal);
        }
        [Test]
        public void TestPastMaraIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            PlayCard("TurretMode");

            DecisionSelectCards = new Card[] { fanatic.CharacterCard, MDP, bunker.CharacterCard, MDP };

            QuickHPStorage(MDP);
            UseIncapacitatedAbility(mara, 0);
            QuickHPCheck(-1);
            UseIncapacitatedAbility(mara, 0);
            QuickHPCheck(-2);

        }
        [Test]
        public void TestPastMaraIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            Card sword = PutInTrash("Absolution");
            UseIncapacitatedAbility(mara, 1);
            AssertOnBottomOfDeck(sword);

            Card mdp = MDP;

            PutInTrash(sword);
            DestroyCard(mdp);
            UseIncapacitatedAbility(mara, 1);
            AssertInTrash(sword);
            AssertOnBottomOfDeck(mdp);
        }
        [Test]
        public void TestPastMaraIncap3MakeLog()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            UseIncapacitatedAbility(mara, 2);
        }
        [Test]
        public void TestPastMaraIncap3TooFewCards()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara/PastMagnificentMaraCharacter", "Fanatic", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);

            IEnumerator coroutine = GameController.BulkMoveCards(mara, fanatic.TurnTaker.Deck.Cards, fanatic.TurnTaker.Trash);
            GameController.ExhaustCoroutine(coroutine);

            Card sword = PutOnDeck("Absolution");
            Card dive = PutOnDeck("FinalDive");

            UseIncapacitatedAbility(mara, 2);
            AssertInHand(sword);
            AssertInTrash(dive);
        }
    }
}

using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.Anathema;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class AnathemaTests : BaseTest
    {
        #region AnathemaHelperFunctions

        protected TurnTakerController anathema { get { return FindVillain("Anathema"); } }

        protected void AssertNumberOfArmsInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsArm(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected List<Card> GetListOfArmsInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsArm(c));
            List<Card> listOfHead = new List<Card>();
            foreach (Card c in cardsInPlay)
            {
                listOfHead.Add(c);
            }
            return listOfHead;
        }

        protected void AssertNumberOfHeadInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsHead(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected List<Card> GetListOfHeadsInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsHead(c));
            List<Card> listOfHead = new List<Card>();
            foreach(Card c in cardsInPlay)
            {
                listOfHead.Add(c);
            }
            return listOfHead;
        }

        protected void AssertNumberOfBodyInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsBody(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected List<Card> GetListOfBodyInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsBody(c));
            List<Card> listOfHead = new List<Card>();
            foreach (Card c in cardsInPlay)
            {
                listOfHead.Add(c);
            }
            return listOfHead;
        }

        private bool IsArm(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "arm", false, false);
        }

        private bool IsHead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "head", false, false);
        }

        private bool IsBody(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "body", false, false);
        }

        private bool IsArmHeadOrBody(Card c)
        {
            return IsArm(c) || IsHead(c) || IsBody(c);
        }


        private void ResetAnathemaDeck()
        {
            //Destroy all arms, heads, and body in play
            DestroyCards((Card c) => this.IsArmHeadOrBody(c));
            //Shuffle all arms, heads, and body back into the deck
            ShuffleTrashIntoDeck(anathema);
        }
        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AssertDamageTypeChanged(HeroTurnTakerController httc, Card source, Card target, int amount, DamageType initialDamageType, DamageType expectedDamageType)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            this.RunCoroutine(this.GameController.DealDamage(httc, source, (Card c) => c == target, amount, initialDamageType, false, false, storedResults, null, null, false, null, null, false, false, new CardSource(GetCardController(source))));
            
            if(storedResults != null)
            {
                DealDamageAction dd = storedResults.FirstOrDefault<DealDamageAction>();
                DamageType actualDamageType = dd.DamageType;
                Assert.AreEqual(expectedDamageType, actualDamageType, $"Expected damage type: {expectedDamageType}. Actual damage type: {actualDamageType}");
            }
            else
            {
                Assert.Fail("storedResults was null");
            }

        }
        #endregion

        [Test()]
        public void TestAnathemaLoadedProperly()
        {
            SetupGameController("Cauldron.Anathema", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(anathema);
            Assert.IsInstanceOf(typeof(AnathemaCharacterCardController), anathema.CharacterCardController);

            Assert.AreEqual(45, anathema.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestAnathemaStartGame()
        {
            SetupGameController("Cauldron.Anathema", "Legacy", "Megalopolis");

            StartGame();

            //check that there are 5 cards in play total (anathema, 2 arms, 1 head, and 1 body)
            AssertNumberOfCardsInPlay(anathema, 5);

            //check that there are 2 arms in play
            AssertNumberOfArmsInPlay(anathema, 2);

            //check that there is 1 body in play
            AssertNumberOfBodyInPlay(anathema, 1);

            //check that there is 1 head in play
            AssertNumberOfHeadInPlay(anathema, 1);
            
        }

        [Test()]
        public void TestAnathemaImmuneWhen4OtherVillainTargets()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(ra);

            // ra tries to deal 2 irreducible damage to anathema
            QuickHPStorage(anathema);
            DecisionSelectTarget = anathema.CharacterCard;

            DealDamage(ra, anathema, 2, DamageType.Fire, true);
            //anathema should be immune since there are 4 other villain targets in play
            QuickHPCheck(0);

        }

        [Test()]
        public void TestAnathemaNotImmuneWhenLessThan4OtherVillainTargets()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(ra);

            List<Card> arms = GetListOfArmsInPlay(anathema);

            //destroy an arm to have less than 4 villain targets
            DestroyCard(arms[0], ra.CharacterCard);

            //verify that there is only 1 arm in play now
            AssertNumberOfArmsInPlay(anathema, 1);
            // ra tries to deal 2 irreducible damage to anathema
            QuickHPStorage(anathema);
            DecisionSelectTarget = anathema.CharacterCard;
            DealDamage(ra, anathema, 2, DamageType.Fire, true);
            //anathema should be dealt since there are only 3 other villain targets in play
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAnathemaGainsHPWhenVillainDestroysArm()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            //set anathema hp to 30 to give room to heal
            SetHitPoints(anathema.CharacterCard, 30);
            QuickHPStorage(anathema);

            List<Card> arms = GetListOfArmsInPlay(anathema);
            //have anathema destroy the arm to trigger healing
            DestroyCard(arms[0], anathema.CharacterCard);
            
            QuickHPCheck(2);
        }

        [Test()]
        public void TestAnathemaGainsHPWhenVillainDestroysBody()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            //set anathema hp to 30 to give room to heal
            SetHitPoints(anathema.CharacterCard, 30);
            QuickHPStorage(anathema);

            List<Card> body = GetListOfBodyInPlay(anathema);
            //have anathema destroy the body to trigger healing
            DestroyCard(body[0], anathema.CharacterCard);

            QuickHPCheck(2);
        }

        [Test()]
        public void TestAnathemaGainsHPWhenVillainDestroysHead()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            //set anathema hp to 30 to give room to heal
            SetHitPoints(anathema.CharacterCard, 30);
            QuickHPStorage(anathema);

            List<Card> heads = GetListOfHeadsInPlay(anathema);
            //have anathema destroy the head to trigger healing
            DestroyCard(heads[0], anathema.CharacterCard);

            QuickHPCheck(2);
        }


        [Test()]
        public void TestAnathemaGainsHPWhenVillainKillsArm()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            //set anathema hp to 30 to give room to heal
            SetHitPoints(anathema.CharacterCard, 30);
            QuickHPStorage(anathema);

            List<Card> arms = GetListOfArmsInPlay(anathema);
            //have anathema destroy the arm to trigger healing
            DealDamage(anathema.CharacterCard, arms[0], 99, DamageType.Psychic);

            QuickHPCheck(2);
        }

        [Test()]
        public void TestAnathemaGainsHPWhenVillainKillsBody()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            //set anathema hp to 30 to give room to heal
            SetHitPoints(anathema.CharacterCard, 30);
            QuickHPStorage(anathema);

            List<Card> body = GetListOfBodyInPlay(anathema);
            //have anathema destroy the body to trigger healing
            DealDamage(anathema.CharacterCard, body[0], 99, DamageType.Psychic);

            QuickHPCheck(2);
        }

        [Test()]
        public void TestAnathemaGainsHPWhenVillainKillsHead()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();

            //set anathema hp to 30 to give room to heal
            SetHitPoints(anathema.CharacterCard, 30);
            QuickHPStorage(anathema);

            List<Card> heads = GetListOfHeadsInPlay(anathema);
            //have anathema destroy the head to trigger healing
            DealDamage(anathema.CharacterCard, heads[0], 99, DamageType.Psychic);

            QuickHPCheck(2);
        }


        [Test()]
        public void TestAnathemaFlipsWhen0VillainTargets()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");
           
            StartGame();
            GoToPlayCardPhase(anathema);

            //Destroy all arms, heads, and body in play
            DestroyCards((Card c) => this.IsArmHeadOrBody(c));

            AssertNotFlipped(anathema);
            GoToStartOfTurn(anathema);
            AssertFlipped(anathema);
        }

        [Test()]
        public void TestAnathemaAdvancedImmuneWhen3Targets()
        {
            SetupGameController(new string[] { "Cauldron.Anathema", "Ra", "Megalopolis" }, advanced: true,advancedIdentifiers: new string[] { "Cauldron.Anathema" });
            
            StartGame();
           
            GoToUsePowerPhase(ra);

            List<Card> arms = (anathema.TurnTaker.PlayArea.Cards.Where(c => this.IsArm(c))).ToList();


            //destroy an arm to have 3 villain targets
            DestroyCard(arms[0], ra.CharacterCard);

            //verify that there is only 1 arm in play now
            AssertNumberOfArmsInPlay(anathema, 1);
            // ra tries to deal 2 irreducible damage to anathema
            QuickHPStorage(anathema);
            DecisionSelectTarget = anathema.CharacterCard;
            DealDamage(ra, anathema, 2, DamageType.Fire, true);
            //anathema should not be dealt damage since there are 3 other villain targets in play
            QuickHPCheck(0);

        }
        [Test()]
        public void TestAnathemaAdvancedNotImmuneWhenLessThan3Targets()
        {
            SetupGameController(new string[] { "Cauldron.Anathema", "Ra", "Megalopolis" }, true, null, null, new string[] { "Cauldron.Anathema" }, false, null, null, null);

            StartGame();

            GoToUsePowerPhase(ra);

            List<Card> arms = GetListOfArmsInPlay(anathema);

            //destroy both arms in play to have 2 villain targets left in play
            DestroyCards(arms);
           

            //verify that there are no arms in play now
            AssertNumberOfArmsInPlay(anathema, 0);
            // ra tries to deal 2 irreducible damage to anathema
            QuickHPStorage(anathema);
            DecisionSelectTarget = anathema.CharacterCard;
            DealDamage(ra, anathema, 2, DamageType.Fire, true);
            //anathema should  be dealt damage since there are 2 other villain targets in play
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestAnathemaFlippedStartOfTurn()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();
            GoToPlayCardPhase(anathema);
            //Destroy all arms, heads, and body in play
            DestroyCards((Card c) => this.IsArmHeadOrBody(c));

            //Flip anathema to his backside
            FlipCard(anathema.CharacterCardController);
            AssertFlipped(anathema);

            //Get the number of cards in the deck before start of turn
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(anathema);

            //At start of the turn, anathema should play an extra card
            GoToStartOfTurn(anathema);

            //Get the number of cards in the deck after start of turn
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(anathema);

            //number of cards in deck should be less than before start of turn
            Assert.IsTrue(numCardsInDeckBefore > numCardsInDeckAfter);

        }

        [Test()]
        public void TestAnathemaFlippedEndOfTurnDiscard()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            GoToPlayCardPhase(anathema);
            //Destroy all arms, heads, and body in play
            DestroyCards((Card c) => this.IsArmHeadOrBody(c));

            //Flip anathema to his backside
            FlipCard(anathema.CharacterCardController);
            AssertFlipped(anathema);

            //Get the number of cards in ra and legacy's hand before end of turn
            int numCardsInHandBefore = GetNumberOfCardsInHand(ra) + GetNumberOfCardsInHand(legacy);

            //At start of the turn, anathema should play an extra card
            GoToEndOfTurn(anathema);

            //Get the number of cards in ra and legacy's hand after end of turn
            int numCardsInHandAfter = GetNumberOfCardsInHand(ra) + GetNumberOfCardsInHand(legacy);

            //number of cards in ra and legacy's hand should be 2 less than before end of turn
            Assert.AreEqual(numCardsInHandBefore - 2, numCardsInHandAfter);
        }

        [Test()]
        public void TestAnathemaFlippedEndOfTurnFlipBack()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            GoToPlayCardPhase(anathema);
            //Destroy all arms, heads, and body in play
            DestroyCards((Card c) => this.IsArmHeadOrBody(c));

            //Flip anathema to his backside
            FlipCard(anathema.CharacterCardController);
            AssertFlipped(anathema);

            //Put a target in play to trigger the flip
            PlayCard("ReflexBooster");

            AssertFlipped(anathema);
            GoToEndOfTurn(anathema);
            AssertNotFlipped(anathema);
        }

        [Test()]
        public void TestAnathemaAdvancedFlippedStartOfTurnDiscard()
        {
            SetupGameController(new string[] { "Cauldron.Anathema", "Ra", "Legacy","Megalopolis" }, true, null, null, new string[] { "Cauldron.Anathema" }, false, null, null, null);


            StartGame();
            GoToPlayCardPhase(anathema);
            //Destroy all arms, heads, and body in play
            DestroyCards((Card c) => this.IsArmHeadOrBody(c));

            //Flip anathema to his backside
            FlipCard(anathema.CharacterCardController);
            AssertFlipped(anathema);

            GoToPlayCardPhase(ra);
            //Get the number of cards in ra and legacy's hand before end of turn
            int numCardsInHandBefore = GetNumberOfCardsInHand(ra) + GetNumberOfCardsInHand(legacy);

            //At start of the turn, anathema should cause all heroes to discard a card
            GoToStartOfTurn(anathema);

            //Get the number of cards in ra and legacy's hand after end of turn
            int numCardsInHandAfter = GetNumberOfCardsInHand(ra) + GetNumberOfCardsInHand(legacy);

            //number of cards in ra and legacy's hand should be 2 less than before end of turn
            Assert.AreEqual(numCardsInHandBefore - 2, numCardsInHandAfter);
        }

        [Test()]
        public void TestBoneCleaverDestroyOtherArm()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
           
            //Put 2 arms in play
            PutIntoPlay("KnuckleDragger");
            PutIntoPlay("KnuckleDragger");

            //check that there are 2 in play
            AssertNumberOfArmsInPlay(anathema, 2);

            //Put Bone Cleaver in play. This should destroy an arm currently in play
            PutIntoPlay("BoneCleaver");
            AssertNumberOfArmsInPlay(anathema, 2);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestBoneCleaverEndOfTurnSuccessfulDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);

            //Put Bone Cleaver in play. 
            PutIntoPlay("BoneCleaver");

            QuickHPStorage(legacy);
            //at the end of turn, anathema deals the hero target with the lowest HP {H-2} melee damage
            //lowest target is legacy, H-2 = 1
            GoToEndOfTurn(anathema);
            QuickHPCheck(-1);

            //legacy should now be unable to deal damage
            QuickHPStorage(anathema);
            DealDamage(legacy.CharacterCard, (Card c) => c == anathema.CharacterCard, 5, DamageType.Melee);
            QuickHPCheck(0);
        }
        [Test()]
        public void TestBoneCleaverEndOfTurnNoDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);

            AddImmuneToDamageTrigger(haka, true, false);

            //Put Bone Cleaver in play. 
            PutIntoPlay("BoneCleaver");
            QuickHPStorage(legacy);
            //at the end of turn, anathema deals the hero target with the lowest HP {H-2} melee damage
            //legacy should be immune to damage, so no damage should be taken
            GoToEndOfTurn(anathema);
            QuickHPCheck(0);

            //legacy should be immune to damage, so he should still be able to deal damage 
            QuickHPStorage(anathema);
            DealDamage(legacy, anathema, 5, DamageType.Melee,true);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestWhipTendrilDestroyOtherArm()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 2 arms in play
            PutIntoPlay("KnuckleDragger");
            PutIntoPlay("KnuckleDragger");

            //check that there are 2 in play
            AssertNumberOfArmsInPlay(anathema, 2);

            //Put Whip Tendril in play. This should destroy an arm currently in play
            PutIntoPlay("WhipTendril");
            AssertNumberOfArmsInPlay(anathema, 2);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestWhipTendrilEndOfTurnSuccessfulDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);

            //Put some equipment in play for  each hero
            PutIntoPlay("TheLegacyRing");
            PutIntoPlay("TheStaffOfRa");
            PutIntoPlay("Taiaha");

            //Put Whip Tendril in play. 
            PutIntoPlay("WhipTendril");

            int numCardInPlayBefore = GetNumberOfCardsInPlay(ra) + GetNumberOfCardsInPlay(legacy) + GetNumberOfCardsInPlay(haka);
            int?[] beforeHps = { ra.CharacterCard.HitPoints, legacy.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };
            //at the end of turn, anathema deals each Hero target 2 projectile damage.
            GoToEndOfTurn(anathema);
            int numCardInPlayAfter = GetNumberOfCardsInPlay(ra) + GetNumberOfCardsInPlay(legacy) + GetNumberOfCardsInPlay(haka);
            int?[] afterHps = { ra.CharacterCard.HitPoints, legacy.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };
            
            Assert.True(beforeHps[0] - 2 == afterHps[0] && beforeHps[1] - 2 == afterHps[1] && beforeHps[2] - 2 == afterHps[2], $"Expected hitpoints of {beforeHps[0] - 2}, {beforeHps[1] - 2}, {beforeHps[2] - 2}. Actual: {afterHps[0]}, {afterHps[1]}, {afterHps[2]}");
            //since damage was dealt, each hero should have destroyed their equipment in play, meaning 3 fewer cards in play
            Assert.AreEqual(numCardInPlayBefore - 3,numCardInPlayAfter);
        }

        [Test()]
        public void TestWhipTendrilEndOfTurnNoDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);

            AddImmuneToDamageTrigger(haka, true, false);

            //Put some equipment in play for  each hero
            PutIntoPlay("TheLegacyRing");
            PutIntoPlay("TheStaffOfRa");
            PutIntoPlay("Taiaha");

            //Put Whip Tendril in play. 
            PutIntoPlay("WhipTendril");

            int numCardInPlayBefore = GetNumberOfCardsInPlay(ra) + GetNumberOfCardsInPlay(legacy) + GetNumberOfCardsInPlay(haka);
            int?[] beforeHps = { ra.CharacterCard.HitPoints, legacy.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };
            //at the end of turn, anathema deals each Hero target 2 projectile damage.
            //all heroes should be immune, so no damage will be dealt
            GoToEndOfTurn(anathema);
            int numCardInPlayAfter = GetNumberOfCardsInPlay(ra) + GetNumberOfCardsInPlay(legacy) + GetNumberOfCardsInPlay(haka);
            int?[] afterHps = { ra.CharacterCard.HitPoints, legacy.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };

            Assert.True(beforeHps[0]  == afterHps[0] && beforeHps[1] == afterHps[1] && beforeHps[2]  == afterHps[2], $"Expected hitpoints of {beforeHps[0]}, {beforeHps[1]}, {beforeHps[2]}. Actual: {afterHps[0]}, {afterHps[1]}, {afterHps[2]}");
            //since no damage was dealt, no equipment should have been destroyed
            Assert.AreEqual(numCardInPlayBefore, numCardInPlayAfter);
        }
        [Test()]
        public void TestKnuckleDraggerDestroyOtherArm()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 2 arms in play
            PutIntoPlay("WhipTendril");
            PutIntoPlay("WhipTendril");

            //check that there are 2 in play
            AssertNumberOfArmsInPlay(anathema, 2);

            //Put Knuckle Dragger in play. This should destroy an arm currently in play
            PutIntoPlay("KnuckleDragger");
            AssertNumberOfArmsInPlay(anathema, 2);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestKnuckleDraggerEndOfTurnSuccessfulDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);

            //put an ongoing in play for haka to destroy
            PutIntoPlay("SavageMana");

            //Put Knuckle Dragger in play. 
            PutIntoPlay("KnuckleDragger");

            int? numCardsInPlayBefore = GetNumberOfCardsInPlay(haka);

            QuickHPStorage(haka);
            //at the end of turn, anathema deals the Hero character with the highest HP {H+1} melee damage.
            //highest target is haka, H+1 = 4
            GoToEndOfTurn(anathema);
            QuickHPCheck(-4);

            int? numCardsInPlayAfter = GetNumberOfCardsInPlay(haka);

            //haka should have had to destroy 1 ongoing card
            Assert.AreEqual(numCardsInPlayBefore - 1, numCardsInPlayAfter);
        }

        [Test()]
        public void TestKnuckleDraggerEndOfTurnNoDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);

            AddImmuneToDamageTrigger(haka, true, false);

            //put an ongoing in play for haka to destroy
            PutIntoPlay("SavageMana");

            //Put Knuckle Dragger in play. 
            PutIntoPlay("KnuckleDragger");

            int? numCardsInPlayBefore = GetNumberOfCardsInPlay(haka);

            QuickHPStorage(haka);
            //at the end of turn, anathema deals the Hero character with the highest HP {H+1} melee damage.
            //since immune trigger is up, no damage should have been dealt
            GoToEndOfTurn(anathema);
            QuickHPCheck(0);

            int? numCardsInPlayAfter = GetNumberOfCardsInPlay(haka);

            //no damage was dealt, so haka should not have destroyed any cards
            Assert.AreEqual(numCardsInPlayBefore, numCardsInPlayAfter);
        }

        [Test()]
        public void TestKnuckleDraggerEndOfTurn_CheckForOriginalTarget()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Luminary", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(luminary.CharacterCard, 25);


            //put an ongoing in play for haka to destroy
            Card plan = PutIntoPlay("AllAccordingToPlan");
            Card defender = PutIntoPlay("DisposableDefender");

            //Put Knuckle Dragger in play. 
            PutIntoPlay("KnuckleDragger");

            int? numCardsInPlayBefore = GetNumberOfCardsInPlay(luminary);

            QuickHPStorage(luminary.CharacterCard, defender);
            //at the end of turn, anathema deals the Hero character with the highest HP {H+1} melee damage.
            //since disposable defender is in, no damage should be dealt to original target
            GoToEndOfTurn(anathema);
            QuickHPCheck(0, -4);

            int? numCardsInPlayAfter = GetNumberOfCardsInPlay(luminary);

            //no damage was dealt, so haka should not have destroyed any cards

            Assert.AreEqual(numCardsInPlayBefore, numCardsInPlayAfter);
            AssertInPlayArea(luminary, plan);
        }

        [Test()]
        public void TestThresherClawDestroyOtherArm()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 2 arms in play
            PutIntoPlay("WhipTendril");
            PutIntoPlay("WhipTendril");

            //check that there are 2 in play
            AssertNumberOfArmsInPlay(anathema, 2);

            //Put Thresher Claw in play. This should destroy an arm currently in play
            PutIntoPlay("ThresherClaw");
            AssertNumberOfArmsInPlay(anathema, 2);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestThresherClawEndOfTurnSuccessfulDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Bunker", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);
            SetHitPoints(bunker.CharacterCard, 26);


            //Put Thresher Claw in play. 
            PutIntoPlay("ThresherClaw");

            int? numCardsInPlayBefore = GetNumberOfCardsInPlay(haka);

            int?[] beforeHps = { haka.CharacterCard.HitPoints, bunker.CharacterCard.HitPoints };

            //at the end of turn, anathema deals the {H-2} heroes with the highest HP 3 toxic damage each.
            //highest target are bunker and haka, H-2 = 2
            GoToEndOfTurn(anathema);
            int?[] afterHps = { haka.CharacterCard.HitPoints, bunker.CharacterCard.HitPoints };

            Assert.True(beforeHps[0] - 3 == afterHps[0] && beforeHps[1] - 3 == afterHps[1], $"Expected hitpoints of {beforeHps[0]}, {beforeHps[1]}. Actual: {afterHps[0]}, {afterHps[1]}");
        }
        [Test()]
        public void TestRazorScalesDestroyOtherBody()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 1 body in play
            PutIntoPlay("MetabolicArmor");

            //check that there are 2 in play
            AssertNumberOfBodyInPlay(anathema, 1);

            //Put Razor Scales in play. This should destroy a body currently in play
            PutIntoPlay("RazorScales");
            AssertNumberOfBodyInPlay(anathema, 1);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestRazorScalesEndOfTurnSuccessfulDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);


            //Put Razor Scales in play
            PutIntoPlay("RazorScales");

            //The first time a Villain target is dealt damage each turn, this card deals the source of that damage 2 melee damage
            QuickHPStorage(haka);
            DealDamage(haka, anathema, 5, DamageType.Melee, true);
            QuickHPCheck(-2);

            //Since Razor Scales has already been used this turn, no counter damage should occur
            QuickHPStorage(ra);
            DealDamage(ra, anathema, 5, DamageType.Melee, true);
            QuickHPCheck(0);

            GoToPlayCardPhase(haka);
            //since it is a new turn, Razor scales should deal the counter damage
            QuickHPStorage(haka);
            DealDamage(haka, anathema, 5, DamageType.Melee, true);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestMetabolicArmorDestroyOtherBody()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 1 body in play
            PutIntoPlay("RazorScales");

            //check that there is 1 in play
            AssertNumberOfBodyInPlay(anathema, 1);

            //Put Metabolic Armor in play. This should destroy a body currently in play
            PutIntoPlay("MetabolicArmor");
            AssertNumberOfBodyInPlay(anathema, 1);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestMetabolicArmorIncreaseDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(haka.CharacterCard, 25);


            //Put Metabolic Armor in play
            PutIntoPlay("MetabolicArmor");

            //Increase damage dealt by Villain targets by 1
            QuickHPStorage(haka);
            DealDamage(anathema, haka, 5, DamageType.Melee, true);
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestMetabolicArmorEndOfTurnGainHp()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            SetHitPoints(anathema, 30);
            GoToPlayCardPhase(anathema);


            //Put Razor Scales in play
            PutIntoPlay("MetabolicArmor");
            int armorStartingHp = 3;
            SetHitPoints(GetCard("MetabolicArmor"), armorStartingHp);

            //At the end of the Villain Turn, all Villain targets regain 1HP.
            QuickHPStorage(anathema);
            GoToEndOfTurn(anathema);

            //Check that anathema and metabolic armor both gained 1 HP
            QuickHPCheck(1);
            Assert.AreEqual(armorStartingHp + 1, GetCard("MetabolicArmor").HitPoints);
        }

        [Test()]
        public void TestHeavyCarapaceDestroyOtherBody()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 1 body in play
            PutIntoPlay("RazorScales");

            //check that there is 1 in play
            AssertNumberOfBodyInPlay(anathema, 1);

            //Put Heavy Carapace in play. This should destroy a body currently in play
            PutIntoPlay("HeavyCarapace");
            AssertNumberOfBodyInPlay(anathema, 1);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestHeavyCarapaceReduceDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(anathema.CharacterCard, 25);


            //Put Heavy Carapace in play
            PutIntoPlay("HeavyCarapace");

            //Reduce damage dealt to Villain targets by 2
            QuickHPStorage(anathema);
            DealDamage(haka, anathema, 5, DamageType.Melee, false);
            QuickHPCheck(-3);

            QuickHPStorage(GetCard("HeavyCarapace"));
            DealDamage(haka.CharacterCard, GetCard("HeavyCarapace"), 5, DamageType.Melee, false);
            QuickHPCheck(-3);
        }
        [Test()]
        public void TestReflexBoosterDestroyOtherHead()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 1 head in play
            PutIntoPlay("CarapaceHelmet");

            //check that there is 1 in play
            AssertNumberOfHeadInPlay(anathema, 1);

            //Put Reflex Booster in play. This should destroy a head currently in play
            PutIntoPlay("ReflexBooster");
            AssertNumberOfHeadInPlay(anathema, 1);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        public void TestReflexBoosterEndOfTurnPlay()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Megalopolis");

            StartGame();
            GoToPlayCardPhase(anathema);
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            PutIntoPlay("ReflexBooster");
            //Get the number of cards in the deck before end of turn
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(anathema);

            //At end of the turn, anathema should play an extra card
            GoToEndOfTurn(anathema);

            //Get the number of cards in the deck after start of turn
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(anathema);

            //number of cards in deck should be less than before start of turn
            Assert.IsTrue(numCardsInDeckBefore > numCardsInDeckAfter);

        }

        [Test()]
        public void TestCarapaceHelmetDestroyOtherHead()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 1 head in play
            PutIntoPlay("ReflexBooster");

            //check that there is 1 in play
            AssertNumberOfHeadInPlay(anathema, 1);

            //Put Carapace Helmet in play. This should destroy a head currently in play
            PutIntoPlay("CarapaceHelmet");
            AssertNumberOfHeadInPlay(anathema, 1);
            AssertNumberOfCardsInTrash(anathema, 1);
        }

        [Test()]
        public void TestCarapaceHelmetReduceDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put Carapace Helmet in play. 
            PutIntoPlay("CarapaceHelmet");

            //Reduce damage dealt to this card by 1
            QuickHPStorage(GetCard("CarapaceHelmet"));
            DealDamage(ra.CharacterCard, (GetCard("CarapaceHelmet")), 5, DamageType.Fire);
            QuickHPCheck(-4);
        }
        [Test()]
        public void TestCarapaceHelmetImmuneToEnvironment()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put Carapace Helmet in play. 
            PutIntoPlay("CarapaceHelmet");

            //villain targets are immune to environment damage

            //check anathema
            QuickHPStorage(anathema);
            DealDamage(GetCard("PoliceBackup"), anathema.CharacterCard, 5, DamageType.Projectile);
            QuickHPCheck(0);

            //check self
            QuickHPStorage(GetCard("CarapaceHelmet"));
            DealDamage(GetCard("PoliceBackup"), GetCard("CarapaceHelmet"), 5, DamageType.Projectile);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestEnhancedSensesDestroyOtherHead()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put 1 head in play
            PutIntoPlay("ReflexBooster");

            //check that there is 1 in play
            AssertNumberOfHeadInPlay(anathema, 1);

            //Put Enhanced Senses in play. This should destroy a head currently in play
            PutIntoPlay("EnhancedSenses");
            AssertNumberOfHeadInPlay(anathema, 1);
            AssertNumberOfCardsInTrash(anathema, 1);
        }
        [Test()]
        public void TestEnhancedSensesChangeDamageType()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Put Enhanced Senses in play. 
            PutIntoPlay("EnhancedSenses");

            //All damage dealt should be sonic
            AssertDamageTypeChanged(ra, ra.CharacterCard, anathema.CharacterCard, 5, DamageType.Fire, DamageType.Sonic);
            AssertDamageTypeChanged(ra, anathema.CharacterCard, ra.CharacterCard, 5, DamageType.Melee, DamageType.Sonic);


        }

        [Test()]
        public void TestEnhancedSensesHeroPlayResponse()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(haka, 25);

            //Put Enhanced Senses in play. 
            PutIntoPlay("EnhancedSenses");

            //Whenever a Hero card enters play, this card deals that Hero Character 1 sonic damage.
            QuickHPStorage(haka);
            PutIntoPlay("Taiaha");
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestTheStuffOfNightmares()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);


            //Put The Stuff of Nightmares in play
            PutIntoPlay("TheStuffOfNightmares");

            //Whenever a Villain target enters play, that target deals the hero target with the second lowest HP 2 psychic damage.           
            //hero target with second lowest hp is ra
            QuickHPStorage(ra);
            PutIntoPlay("ReflexBooster");
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestExplosiveTransformationDealDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);


            int?[] beforeHps = { ra.CharacterCard.HitPoints, legacy.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };

            //Put Explosive Transformation in play. 
            //anathema deals each Hero target 1 projectile damage.
            PutIntoPlay("ExplosiveTransformation");
            int?[] afterHps = { ra.CharacterCard.HitPoints, legacy.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };

            Assert.True(beforeHps[0] - 1 == afterHps[0] && beforeHps[1] - 1 == afterHps[1] && beforeHps[2] - 1 == afterHps[2], $"Expected hitpoints of {beforeHps[0] - 1}, {beforeHps[1] - 1}, {beforeHps[2] - 1}. Actual: {afterHps[0]}, {afterHps[1]}, {afterHps[2]}");
        }

        [Test()]
        public void TestExplosiveTransformationRevealCards()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Tachyon", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);


            PutOnDeck("WhipTendril");
            PutOnDeck("ThresherClaw");
            PutOnDeck("RazorScales");
            PutOnDeck("EnhancedSenses");

            int numCardsInDeckBefore = GetNumberOfCardsInDeck(anathema);
            //Put Explosive Transformation in play. 
            //Reveal the top {H} cards of the Villain Deck. Put the first revealed arm, body, and head into play. Shuffle the remaining cards back into the Villain Deck.
            PutIntoPlay("ExplosiveTransformation");
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(anathema);

            AssertNumberOfArmsInPlay(anathema, 1);
            AssertNumberOfBodyInPlay(anathema, 1);
            AssertNumberOfHeadInPlay(anathema, 1);
            //3 cards were put into play by explosive transformation, plus explosive transformation itself
            Assert.AreEqual(numCardsInDeckBefore - 4, numCardsInDeckAfter);
        }
        [Test()]
        public void TestRampageDealDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Tachyon", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //draw a card, so Ra now has the most cards in hand
            DrawCard(ra, 1, false);

            //Put Rampage in play. 
            //anathema deals the Hero target with the most cards in hand {H-1} melee damage.
            //ra has the most cards in hand, H-1 = 3
            QuickHPStorage(ra);
            PutIntoPlay("AnathemaRampage");
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestRampageDamageTriggers()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Tachyon", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            GoToPlayCardPhase(anathema);

            //Play RazorScales to make sure anathema doesn't flip
            PlayCard("RazorScales");

            //Put Rampage in play. 
            //Until the start of the next Villain Turn, increase damage dealt by Villain targets by 1 and reduce damage dealt to Villain targets by 1.
            PlayCard("AnathemaRampage");

            //damage dealt by villain should be increase by 1
            QuickHPStorage(ra);
            DealDamage(anathema, ra, 3, DamageType.Sonic);
            QuickHPCheck(-4);

            //damage dealt to villain targets should be reduced by 1
            QuickHPStorage(anathema);
            DealDamage(ra, anathema, 3, DamageType.Fire);
            QuickHPCheck(-2);

            //damage trigger lasts until start of next turn, resetting
            GoToStartOfTurn(anathema);

            //triggers have been removed, should be normal
            QuickHPStorage(ra);
            DealDamage(anathema, ra, 3, DamageType.Sonic);
            QuickHPCheck(-3);

            //triggers have been removed, should be normal
            QuickHPStorage(anathema);
            DealDamage(ra, anathema, 3, DamageType.Fire);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoppelgangerStrikeDealDamageSuccess()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);
            GoToPlayCardPhase(anathema);

            //Play 2 villain targets to provide an X
            PlayCard("BoneCleaver");
            PlayCard("KnuckleDragger");

            PutOnDeck("WhipTendril");

            //Put Doppelganger Strike in play. 
            //The Hero target with the highest HP deals the Hero Character with the lowest HP X toxic damage, where X is the number of villain targets in play.
            //A Hero dealt damage this way must discard {H-2} cards.
            //highest HP is haka, lowest hp is legacy,3 targets (including anathema, so X=3, H - 2 = 1
            QuickHPStorage(legacy);
            QuickHandStorage(legacy);
            PutIntoPlay("DoppelgangerStrike");
            QuickHPCheck(-3);
            QuickHandCheck(-1);
        }
        [Test()]
        public void TestDoppelgangerStrikeDealDamageFailure()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 15);
            SetHitPoints(haka.CharacterCard, 25);
            GoToPlayCardPhase(anathema);
            AddImmuneToDamageTrigger(haka, true, false);

            //Play 2 villain targets to provide an X
            PlayCard("BoneCleaver");
            PlayCard("KnuckleDragger");

            //Put Doppelganger Strike in play. 
            //The Hero target with the highest HP deals the Hero Character with the lowest HP X toxic damage, where X is the number of villain targets in play.
            //A Hero dealt damage this way must discard {H-2} cards.
            //since all heroes immune, no damage should be dealt, and no cards discarded
            QuickHPStorage(legacy);
            QuickHandStorage(legacy);
            PutIntoPlay("DoppelgangerStrike");
            QuickHPCheck(0);
            QuickHandCheck(0);
        }
        [Test()]
        public void TestDoppelgangerStrikePlayCard()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();

            //stack deck to reduce variance
            PutOnDeck("Biofeedback");

            GoToPlayCardPhase(anathema);

            //Put Doppelganger Strike in play. 
            //play the top card of the villain deck
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(anathema);
            PutIntoPlay("DoppelgangerStrike");
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(anathema);

            //2 cards should have been played, Doppelganger strike, and the new card
            Assert.AreEqual(numCardsInDeckBefore - 2, numCardsInDeckAfter);
        }
        [Test()]
        public void TestBiofeedbackGainHP()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            SetHitPoints(anathema.CharacterCard, 30);
            GoToPlayCardPhase(anathema);

            //put biofeedback in play
            PutIntoPlay("Biofeedback");

            //Whenever anathema deals damage to a Hero target, he regains 1 HP.
            QuickHPStorage(anathema);
            DealDamage(anathema, ra, 5, DamageType.Toxic);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestBiofeedbackNoGainHPOnNoDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();

            PutIntoPlay("FleshOfTheSunGod");

            ResetAnathemaDeck();
            SetHitPoints(anathema.CharacterCard, 30);
            GoToPlayCardPhase(anathema);

            //put biofeedback in play
            PutIntoPlay("Biofeedback");

            //Whenever anathema deals damage to a Hero target, he regains 1 HP.
            QuickHPStorage(anathema);
            DealDamage(anathema, ra, 5, DamageType.Fire);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestBiofeedbackSelfDamage()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();
            ResetAnathemaDeck();
            SetHitPoints(anathema.CharacterCard, 30);
            GoToPlayCardPhase(anathema);

            //put an arm in play to test effect
            var knuckle = PlayCard("KnuckleDragger");

            //put biofeedback in play
            PutIntoPlay("Biofeedback");

            //Whenever an arm, body, or head is destroyed by a Hero target, Anathema deals himself 2 psychic damage.
            QuickHPStorage(anathema);
            DestroyCard(knuckle, ra.CharacterCard);
            QuickHPCheck(-2);

            //reset and test with damage.
            PlayCard(knuckle);
            QuickHPStorage(anathema);
            DealDamage(ra.CharacterCard, knuckle, 99, DamageType.Fire, true);
            QuickHPCheck(-2);

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Arm_IsArm([Values("BoneCleaver", "WhipTendril", "KnuckleDragger", "ThresherClaw")] string arm)
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();

            GoToPlayCardPhase(anathema);

            Card card = PlayCard(arm);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "arm", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Body_IsBody([Values("RazorScales", "MetabolicArmor", "HeavyCarapace")] string body)
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();

            GoToPlayCardPhase(anathema);

            Card card = PlayCard(body);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "body", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Head_IsHead([Values("ReflexBooster", "CarapaceHelmet", "EnhancedSenses")] string head)
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();

            GoToPlayCardPhase(anathema);

            Card card = PlayCard(head);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "head", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Ongoing_IsOngoing([Values("TheStuffOfNightmares", "Biofeedback")] string ongoing)
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();

            GoToPlayCardPhase(anathema);

            Card card = PlayCard(ongoing);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "ongoing", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Oneshot_IsOneshot([Values("ExplosiveTransformation", "AnathemaRampage", "DoppelgangerStrike")] string oneshot)
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Legacy", "Haka", "Megalopolis");

            StartGame();

            GoToPlayCardPhase(anathema);

            Card card = PlayCard(oneshot);
            AssertInTrash(card);
            AssertCardHasKeyword(card, "one-shot", false);
        }




    }
}

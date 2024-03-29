using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using Cauldron.Anathema;
using Handelabra;

namespace CauldronTests
{
    [TestFixture()]
    public class AnathemaVariantTests : CauldronBaseTest
    {
        #region AnathemaHelperFunctions


        protected void AssertNumberOfArmsInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsArm(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, $"{ttc.Name} should have had {number} cards in play, but actually had {actual}: {cardsInPlay.Select(c => c.Title).ToCommaList()}");
        }

        protected List<Card> GetListOfArmsInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlayAndNotUnderCard && this.IsArm(c));
            return cardsInPlay.ToList();
        }

        protected void AssertNumberOfHeadInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsHead(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, $"{ttc.Name} should have had {number} cards in play, but actually had {actual}: {cardsInPlay.Select(c => c.Title).ToCommaList()}");
        }

        protected List<Card> GetListOfHeadsInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsHead(c));
            return cardsInPlay.ToList();
        }

        protected void AssertNumberOfBodyInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsBody(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, $"{ttc.Name} should have had {number} cards in play, but actually had {actual}: {cardsInPlay.Select(c => c.Title).ToCommaList()}");
        }

        protected List<Card> GetListOfBodyInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsBody(c));
            return cardsInPlay.ToList();
        }

        private bool IsArm(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "arm");
        }

        private bool IsHead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "head");
        }

        private bool IsBody(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "body");
        }

        #endregion

        [Test()]
        public void TestAcceleratedEvolutionAnathemaLoadedProperly()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(anathema);
            Assert.IsInstanceOf(typeof(AcceleratedEvolutionAnathemaCharacterCardController), anathema.CharacterCardController);

            Assert.AreEqual(70, anathema.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaStartGame()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");

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
        public void TestAcceleratedEvolutionAnathemaOnFlip_ShuffleExplosiveTransformations()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            IEnumerable<Card> transformations = FindCardsWhere((Card c) => anathema.TurnTaker.Deck.HasCard(c) && c.Identifier == "ExplosiveTransformation");
            PutInTrash(transformations);
            AssertInTrash(transformations);

            //Shuffle all copies of explosive transformation from the villain trash into the villain deck.
            QuickShuffleStorage(anathema);
            AssertNotFlipped(anathema);
            FlipCard(anathema);
            AssertFlipped(anathema);
            AssertInDeck(transformations);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaOnFlip_PlayAllUnderCards()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            IEnumerable<Card> cardsUnder = GetCards("Biofeedback", "TheStuffOfNightmares", "HeavyCarapace");
            MoveCards(anathema, cardsUnder, anathema.CharacterCard.UnderLocation);
            AssertNumberOfCardsUnderCard(anathema.CharacterCard, 3);
            //When {Anathema} flips to this side, put all cards from underneath him into play. 
            AssertNotFlipped(anathema);
            FlipCard(anathema);
            AssertFlipped(anathema);
            AssertNumberOfCardsUnderCard(anathema.CharacterCard, 0);
            AssertInPlayArea(anathema, cardsUnder);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaMoveCardsUnder_AnathemaDestroysArm()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card armToDestroy = GetListOfArmsInPlay(anathema).First();
            AssertIsInPlayAndNotUnderCard(armToDestroy);
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            DestroyCard(armToDestroy, anathema.CharacterCard);
            AssertUnderCard(anathema.CharacterCard, armToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaDontMoveCardsUnder_OtherDestroysArm()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card armToDestroy = GetListOfArmsInPlay(anathema).First();
            AssertIsInPlayAndNotUnderCard(armToDestroy);
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            DestroyCard(armToDestroy, legacy.CharacterCard);
            AssertInTrash(armToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaMoveCardsUnder_AnathemaDestroysHead()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            AssertIsInPlayAndNotUnderCard(headToDestroy);
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            DestroyCard(headToDestroy, anathema.CharacterCard);
            AssertUnderCard(anathema.CharacterCard, headToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaDontMoveCardsUnder_OtherDestroysHead()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            AssertIsInPlayAndNotUnderCard(headToDestroy);
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            DestroyCard(headToDestroy, legacy.CharacterCard);
            AssertInTrash(headToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaDontMoveCardsUnder_AnathemaDestroysBody()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card bodyToDestroy = GetListOfBodyInPlay(anathema).First();
            AssertIsInPlayAndNotUnderCard(bodyToDestroy);
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            DestroyCard(bodyToDestroy, anathema.CharacterCard);
            AssertInTrash(bodyToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaMoveCardsUnder_HeadDestroysHead()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            Card headToPlay = FindCardsWhere((Card c) => anathema.TurnTaker.Deck.HasCard(c) && IsHead(c)).First();
            AssertIsInPlayAndNotUnderCard(headToDestroy);
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            PlayCard(headToPlay);
            AssertUnderCard(anathema.CharacterCard, headToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaEndOfTurnFront_ArmOnTop_NoFlip()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            //prevent exlosive transform from failing the test
            MoveCards(anathema, (Card c) => c.Identifier == "ExplosiveTransformation", anathema.TurnTaker.Trash);
            Card armOnDeck = PutOnDeck("WhipTendril");
            AssertInDeck(armOnDeck);
            //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            //Then if there are {H} or more cards under {Anathema}, flip his villain character card.           
            GoToEndOfTurn(anathema);
            AssertUnderCard(anathema.CharacterCard, armOnDeck);
            AssertNumberOfCardsInRevealed(anathema, 0);
            AssertNotFlipped(anathema.CharacterCard);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaEndOfTurnFront_ArmOnTop_Flip()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            //prevent exlosive transform from failing the test
            MoveCards(anathema, (Card c) => c.Identifier == "ExplosiveTransformation", anathema.TurnTaker.Trash);
            IEnumerable<Card> cardsUnder = GetCards("Biofeedback", "TheStuffOfNightmares", "HeavyCarapace", "RazorScales");
            MoveCards(anathema, cardsUnder, anathema.CharacterCard.UnderLocation);
            Card armOnDeck = PutOnDeck("WhipTendril");
            AssertInDeck(armOnDeck);
            //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            //Then if there are {H} or more cards under {Anathema}, flip his villain character card.  
            AssertNotFlipped(anathema.CharacterCard);
            GoToEndOfTurn(anathema);
            AssertFlipped(anathema.CharacterCard);
            AssertNumberOfCardsInRevealed(anathema, 0);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaEndOfTurnFront_HeadOnTop_NoFlip()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            //prevent exlosive transform from failing the test
            MoveCards(anathema, (Card c) => c.Identifier == "ExplosiveTransformation", anathema.TurnTaker.Trash);
            Card headOnDeck = PutOnDeck("CarapaceHelmet");
            AssertInDeck(headOnDeck);
            //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            //Then if there are {H} or more cards under {Anathema}, flip his villain character card.           
            GoToEndOfTurn(anathema);
            AssertUnderCard(anathema.CharacterCard, headOnDeck);
            AssertNumberOfCardsInRevealed(anathema, 0);
            AssertNotFlipped(anathema.CharacterCard);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaEndOfTurnFront_HeadOnTop_Flip()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            //prevent exlosive transform from failing the test
            MoveCards(anathema, (Card c) => c.Identifier == "ExplosiveTransformation", anathema.TurnTaker.Trash);
            IEnumerable<Card> cardsUnder = GetCards("Biofeedback", "TheStuffOfNightmares", "HeavyCarapace", "RazorScales");
            MoveCards(anathema, cardsUnder, anathema.CharacterCard.UnderLocation);
            Card headOnDeck = PutOnDeck("CarapaceHelmet");
            AssertInDeck(headOnDeck);
            //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            //Then if there are {H} or more cards under {Anathema}, flip his villain character card.  
            AssertNotFlipped(anathema.CharacterCard);
            GoToEndOfTurn(anathema);
            AssertFlipped(anathema.CharacterCard);
            AssertNumberOfCardsInRevealed(anathema, 0);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaEndOfTurnFront_NonArmHead()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            //prevent exlosive transform from failing the test
            MoveCards(anathema, (Card c) => c.Identifier == "ExplosiveTransformation", anathema.TurnTaker.Trash);
            Card headOnDeck = PutOnDeck("Biofeedback");
            AssertInDeck(headOnDeck);
            //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            //Then if there are {H} or more cards under {Anathema}, flip his villain character card.           
            GoToEndOfTurn(anathema);
            AssertInTrash(headOnDeck);
            AssertNumberOfCardsInRevealed(anathema, 0);
            AssertNotFlipped(anathema.CharacterCard);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaAdvancedEndOfTurnBack()
        {
            SetupGameController(new string[] { "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: true);
            StartGame();
            MoveCards(anathema, FindCardsWhere((Card c) => !c.IsCharacter && c.IsInPlayAndHasGameText && anathema.TurnTaker.PlayArea.HasCard(c)), anathema.TurnTaker.Deck);
            PlayCard("WhipTendril");
            FlipCard(anathema);

            SetHitPoints(anathema, 30);
            //At the end of the villain turn {Anathema} regains 1 HP for each villain target in play.
            //should be 2 targets in play
            QuickHPStorage(anathema);
            GoToEndOfTurn(anathema);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaAdvancedEndOfTurnFront()
        {
            SetupGameController(new string[] { "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: true);
            StartGame();
            SetHitPoints(anathema, 30);
            DestroyCards(FindCardsWhere((Card c) => !c.IsCharacter && c.IsInPlayAndHasGameText && anathema.TurnTaker.PlayArea.HasCard(c)));
            //At the end of the villain turn, {Anathema} regains {H - 2} HP.
            //H = 3 -> regain 1 HP
            QuickHPStorage(anathema);
            GoToEndOfTurn(anathema);
            QuickHPCheck(1);

        }

        [Test()]
        public void TestAcceleratedEvolutionAnathemaBackFlipEffect()
        {
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);

            //When explosive transformation enters play, flip {Anathema}'s character cards.
            PlayCard("ExplosiveTransformation");
            AssertNotFlipped(anathema);
        }

        [Test()]
        public void TestAcceleratedEvolutionHeadIndestructibilityTest_NonFlippedVillainTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            DestroyCard(headToDestroy, ra.CharacterCard);
            AssertInTrash(headToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionHeadIndestructibilityTest_NonFlippedHeroTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            GoToStartOfTurn(legacy);
            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            DestroyCard(headToDestroy, ra.CharacterCard);
            AssertInTrash(headToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionHeadIndestructibilityTest_FlippedVillainTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);
            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            DestroyCard(headToDestroy, ra.CharacterCard);
            AssertInPlayArea(anathema, headToDestroy);

        }

        [Test()]
        public void TestAcceleratedEvolutionHeadIndestructibilityTest_FlippedVillainTurn_NegativeHP()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);

            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            DealDamage(ra.CharacterCard, headToDestroy, 50, DamageType.Fire, isIrreducible: true);
            DestroyCard(headToDestroy, ra.CharacterCard);
            AssertInPlayArea(anathema, headToDestroy);
            GoToNextTurn();
            AssertInTrash(anathema, headToDestroy);
        }

        [Test()]
        public void TestAcceleratedEvolutionHeadIndestructibilityTest_FlippedHeroTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);
            GoToStartOfTurn(legacy);
            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            DestroyCard(headToDestroy, ra.CharacterCard);
            AssertInTrash(headToDestroy);

        }

        [Test()]
        public void TestAcceleratedEvolutionArmIndestructibilityTest_NonFlippedVillainTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card armToDestroy = GetListOfArmsInPlay(anathema).First();
            DestroyCard(armToDestroy, ra.CharacterCard);
            AssertInTrash(armToDestroy);

        }

        [Test()]
        public void TestAcceleratedEvolutionArmIndestructibilityTest_NonFlippedHeroTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            GoToStartOfTurn(legacy);
            Card armToDestroy = GetListOfArmsInPlay(anathema).First();
            DestroyCard(armToDestroy, ra.CharacterCard);
            AssertInTrash(armToDestroy);

        }

        [Test()]
        public void TestAcceleratedEvolutionArmIndestructibilityTest_FlippedVillainTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);
            Card armToDestroy = GetListOfArmsInPlay(anathema).First();
            DestroyCard(armToDestroy, ra.CharacterCard);
            AssertInPlayArea(anathema, armToDestroy);

        }

        [Test()]
        public void TestAcceleratedEvolutionArmsIndestructibilityTest_FlippedHeroTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);
            GoToStartOfTurn(legacy);
            Card armToDestroy = GetListOfArmsInPlay(anathema).First();
            DestroyCard(armToDestroy, ra.CharacterCard);
            AssertInTrash(armToDestroy);

        }

        [Test()]
        public void TestAcceleratedEvolutionBodyIndestructibilityTest_FlippedVillainTurn()
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.

            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            FlipCard(anathema.CharacterCard);
            AssertFlipped(anathema);
            Card bodyToDestroy = GetListOfBodyInPlay(anathema).First();
            DestroyCard(bodyToDestroy, ra.CharacterCard);
            AssertInTrash(bodyToDestroy);

        }

        [Test]
        public void TestAcceleratedEvolutionChallenge_Front_AnathemaDestroys()
        {
            SetupGameController(new string[] { "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            //Front Challenge: Replace both instances of “arm or head card” with “villain target”.
            //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
            Card bodyToDestroy = GetListOfBodyInPlay(anathema).First();
            DestroyCard(bodyToDestroy, anathema.CharacterCard);
            AssertUnderCard(anathema.CharacterCard, bodyToDestroy);

            Card headToDestroy = GetListOfHeadsInPlay(anathema).First();
            DestroyCard(headToDestroy, anathema.CharacterCard);
            AssertUnderCard(anathema.CharacterCard, headToDestroy);
        }

        [Test]
        public void TestAcceleratedEvolutionChallenge_Front_EndOfTurnReveal_Body()
        {
            SetupGameController(new string[] { "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            //Front Challenge: Replace both instances of “arm or head card” with “villain target”.
            //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            Card bodyToReveal = StackDeck(anathema, "RazorScales");
            GoToEndOfTurn(anathema);
            AssertUnderCard(anathema.CharacterCard, bodyToReveal);
        }

        [Test]
        public void TestAcceleratedEvolutionChallenge_Back()
        {
            SetupGameController(new string[] { "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Ra", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            //Back Challenge: Body cards are indestructible during the villain turn
            FlipCard(anathema.CharacterCard);

            Card bodyToDestroy = GetListOfBodyInPlay(anathema).First();
            DestroyCard(bodyToDestroy, anathema.CharacterCard);
            AssertIsInPlayAndNotUnderCard(bodyToDestroy);
        }
    }
}

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
    public class AnathemaVariantTests : BaseTest
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
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Megalopolis");

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
            SetupGameController("Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter", "Legacy", "Megalopolis");
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




    }
}

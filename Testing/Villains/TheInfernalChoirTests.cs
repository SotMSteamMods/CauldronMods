using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.TheInfernalChoir;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class TheInfernalChoirTests : BaseTest
    {
        #region HelperFunctions

        protected TheInfernalChoirTurnTakerController choir { get { return FindVillain("TheInfernalChoir") as TheInfernalChoirTurnTakerController; } }

        protected Card heart1 => GetCard("VagrantHeartPhase1");
        protected Card heart2 => GetCard("VagrantHeartPhase2");

        private void AssertCard(string identifier, string[] keywords = null, int hitpoints = 0)
        {
            Card card = GetCard(identifier);
            if (keywords != null)
            {
                foreach (string keyword in keywords)
                {
                    AssertCardHasKeyword(card, keyword, false);
                }
            }
            if (hitpoints > 0)
            {
                AssertMaximumHitPoints(card, hitpoints);
            }
        }

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotDealDamageTrigger(TurnTakerController ttc, Card specificCard)
        {
            CannotDealDamageStatusEffect cannotDealDamageEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageEffect.SourceCriteria.IsSpecificCard = specificCard;
            cannotDealDamageEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AssertDamageTypeChanged(HeroTurnTakerController httc, Card source, Card target, int amount, DamageType initialDamageType, DamageType expectedDamageType)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            this.RunCoroutine(this.GameController.DealDamage(httc, source, (Card c) => c == target, amount, initialDamageType, false, false, storedResults, null, null, false, null, null, false, false, new CardSource(GetCardController(source))));

            if (storedResults != null)
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
        [Order(0)]
        public void TestTheInfernalChoir_LoadedProperly()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(choir);
            Assert.IsInstanceOf(typeof(TheInfernalChoirTurnTakerController), choir);
            Assert.IsInstanceOf(typeof(TheInfernalChoirCharacterCardController), choir.CharacterCardController);
        }

        [Test()]
        public void TestTheInfernalChoir_StartGame()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(choir, choir.CharacterCard);
            AssertIsTarget(choir.CharacterCard, 55);
            AssertNotFlipped(choir);

            AssertNotFlipped(heart1);
            AssertIsInPlay(heart1);
            Assert.IsTrue(heart1.Location.IsHeroPlayAreaRecursive, "Vagrant Heart should be in a Hero Play Area, instead " + heart1.Location.GetFriendlyName());


        }

        [Test()]
        public void TestTheInfernalChoir_DeckList()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "Ra", "Megalopolis");

            AssertCardHasKeyword(choir.CharacterCard, "villain", false);

            AssertCard("VagrantHeartPhase1", new string[] { "hidden heart" });
            AssertCard("VagrantHeartPhase2", new string[] { "soul" });

            AssertCard("RedSunXu", new string[] { "ghost" }, 3);
            AssertCard("KataMichi", new string[] { "ghost" }, 9);
            AssertCard("DanchengTheGiant", new string[] { "ghost" }, 7);
            AssertCard("BaneOfEmbers", new string[] { "ghost" }, 4);
            AssertCard("BaneOfIron", new string[] { "ghost" }, 3);

            AssertCard("HauntingNocturne", new string[] { "charm" }, 6);
            AssertCard("WretchedSymphony", new string[] { "charm" }, 7);
            AssertCard("InfernalElegy", new string[] { "charm" }, 11);
            
            AssertCard("Eclipse", new string[] { "ongoing" });
            AssertCard("BeneathTheFlesh", new string[] { "ongoing" });

            AssertCard("TrueColors", new string[] { "one-shot" });
            AssertCard("TheVoicesGather", new string[] { "one-shot" });
            AssertCard("MoonEater", new string[] { "one-shot" });
            AssertCard("NoWitnesses", new string[] { "one-shot" });
        }

        [Test()]
        public void TestVagrantHeartPhase1_DamageResponse()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);

            var deckCount = GetNumberOfCardsInDeck(legacy);
            var underCount = GetNumberOfCardsUnderCard(heart1);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 5, DamageType.Melee);
            QuickHPCheck(0, 0);
            AssertNumberOfCardsInDeck(legacy, deckCount - 5);
            AssertNumberOfCardsAtLocation(heart1.UnderLocation, underCount + 5);

            QuickHPUpdate();
            DealDamage(choir, choir, 5, DamageType.Cold);
            QuickHPCheck(0, 0);
            AssertNumberOfCardsInDeck(legacy, deckCount - 10);
            AssertNumberOfCardsAtLocation(heart1.UnderLocation, underCount + 10);
        }

        [Test()]
        public void TestVagrantHeartPhase1_FlipFromMove()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);

            var deckCount = GetNumberOfCardsInDeck(legacy);
            var underCount = GetNumberOfCardsUnderCard(heart1);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 36, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNumberOfCardsInDeck(legacy, deckCount);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertNotFlipped(heart2);
        }

        [Test()]
        public void TestVagrantHeartPhase1_FlipFromDraw()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);

            var deckCount = GetNumberOfCardsInDeck(legacy);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 35, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNotFlipped(choir.CharacterCard);

            DrawCard(legacy);

            AssertNumberOfCardsInDeck(legacy, deckCount - 1);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertNotFlipped(heart2);
        }

        [Test()]
        public void TestVagrantHeartPhase1_FlipFromPlay()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);
            
            var deckCount = GetNumberOfCardsInDeck(legacy);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 35, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNotFlipped(choir.CharacterCard);

            PlayTopCard(legacy);

            AssertNumberOfCardsInDeck(legacy, deckCount - 1);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertNotFlipped(heart2);
        }

        [Test()]
        public void TestVagrantHeartPhase1_Defeated()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            AssertInPlayArea(legacy, heart1);

            DestroyCard(legacy);

            AssertGameOver(EndingResult.AlternateDefeat);
        }
    }
}

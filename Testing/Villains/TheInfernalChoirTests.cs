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

            AssertCard("VagrantHeartPhase1", new string[] { "soul" });
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
        public void TestTheInfernalChoir_Front_EndOfTurnPlay()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            var c1 = GetCard("BaneOfIron", 0);
            var c2 = GetCard("BaneOfIron", 1);

            PutOnDeck(choir, c1);
            PutOnDeck(choir, c2);

            GoToPlayCardPhase(choir);
            base.RunActiveTurnPhase();

            GoToEndOfTurn(choir);
            AssertIsInPlay(c1);
            AssertIsInPlay(c2);
        }

        [Test()]
        public void TestTheInfernalChoir_Front_EndOfTurnDamage()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "Megalopolis");
            StartGame();

            var c1 = PlayCard("BaneOfIron");

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, c1);
            GoToEndOfTurn(choir);
            QuickHPCheck(0, -1, -1, 0);
        }

        [Test()]
        public void TestTheInfernalChoir_Front_Advanced()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "Megalopolis" }, advanced: true);
            StartGame();

            var c1 = PlayCard("BaneOfIron");

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, c1);
            DealDamage(legacy, haka, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);

            QuickHPUpdate();
            DealDamage(c1, haka, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);

            QuickHPUpdate();
            DealDamage(choir, haka, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);
        }

        [Test()]
        public void TestTheInfernalChoir_Flipped_StartAndEnd()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "Ra", "Megalopolis" }, advanced: false);
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            GoToStartOfTurn(legacy);

            FlipCard(choir);
            GoToEndOfTurn(env);

            var c1 = GetCard("BaneOfIron", 0);
            var c2 = GetCard("BaneOfIron", 1);

            PutOnDeck(choir, c1);
            PutOnDeck(choir, c2);

            var p1 = PutOnDeck("InspiringPresence", true);
            MoveCards(legacy, GetTopCardsOfDeck(legacy, 4), legacy.TurnTaker.Deck, true);
            
            var p2 = PutOnDeck("Dominion", true);
            MoveCards(haka, GetTopCardsOfDeck(haka, 4), haka.TurnTaker.Deck, true);

            var p3 = PutOnDeck("ImbuedFire", true);
            MoveCards(ra, GetTopCardsOfDeck(ra, 4), ra.TurnTaker.Deck, true);

            var inPlay = GameController.HeroTurnTakerControllers.ToDictionary(htt => htt, htt => GetNumberOfCardsInPlay(htt));

            //Start of Turn
            //remove all by 5 cards from each hero's deck
            //play the top card of each deck, then remove hero plays from game

            GoToStartOfTurn(choir);

            AssertInPlayArea(choir, c2);
            AssertInPlayArea(legacy, p1);
            AssertInPlayArea(haka, p2);
            AssertInPlayArea(ra, p3);

            GoToPlayCardPhase(choir);

            RunActiveTurnPhase();
            AssertInPlayArea(choir, c1);

            GoToEndOfTurn(choir);

            AssertInPlayArea(choir, c2);
            AssertOutOfGame(p1);
            AssertOutOfGame(p2);
            AssertOutOfGame(p3);

            GameController.HeroTurnTakerControllers.ForEach(htt => AssertNumberOfCardsInPlay(htt, inPlay[htt]));
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


        [Test()]
        public void TestVagrantHeartPhase2_PreventEmptyDraw()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            FlipCard(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);

            QuickHandStorage(legacy);
            DrawCard(legacy);
            QuickHandCheck(1);

            MoveAllCards(legacy, legacy.TurnTaker.Deck, legacy.TurnTaker.Trash);

            QuickHandStorage(legacy);
            DrawCard(legacy);
            QuickHandCheck(0);
            AssertNumberOfCardsInDeck(legacy, 0);

        }

        [Test()]
        public void TestVagrantHeartPhase2_PreventEmptyPlay()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            FlipCard(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);

            var p1 = PutOnDeck("InspiringPresence");

            PlayTopCard(legacy);
            AssertInPlayArea(legacy, p1);

            MoveAllCards(legacy, legacy.TurnTaker.Deck, legacy.TurnTaker.Trash);

            AssertNumberOfCardsInDeck(legacy, 0);
            int count = GetNumberOfCardsInPlay(legacy);

            PlayTopCard(legacy);

            AssertNumberOfCardsInDeck(legacy, 0);
            AssertNumberOfCardsInPlay(legacy, count);

        }

        [Test()]
        public void TestVagrantHeartPhase2_PreventEmptyDiscard()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            FlipCard(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);

            var p1 = DiscardTopCards(legacy, 1);
            AssertInTrash(p1);

            MoveAllCards(legacy, legacy.TurnTaker.Deck, legacy.TurnTaker.Trash);

            AssertNumberOfCardsInDeck(legacy, 0);
            int count = GetNumberOfCardsInTrash(legacy);

            p1 = DiscardTopCards(legacy, 1);

            AssertNumberOfCardsInTrash(legacy, count);
            AssertNumberOfCardsInDeck(legacy, 0);

        }

        [Test()]
        public void TestVagrantHeartPhase2_TestOmnitronXReset()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            FlipCard(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);

            DiscardAllCards(omnix);
            var p1 = PutInHand("Reset");
            var p2 = PutInHand("DefensiveBlast");
            RunCoroutine(GameController.BulkMoveCards(omnix, omnix.TurnTaker.Deck.Cards, omnix.TurnTaker.Trash, cardSource: omnix.CharacterCardController.GetCardSource()));

            DecisionSelectCard = p2;
            PlayCard(p1);
            AssertInTrash(p1);
            AssertNumberOfCardsInTrash(omnix, 1);
        }

    }
}

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
    public class TheInfernalChoirTests : CauldronBaseTest
    {
        #region HelperFunctions

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
        public void TestTheInfernalChoir_Front_Indestructible()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            PrintSpecialStringsForCard(choir.CharacterCard);
            DestroyCard(choir.CharacterCard, legacy.CharacterCard);
            AssertNotGameOver();
            AssertInPlayArea(choir, choir.CharacterCard);
        }

        [Test()]
        public void TestTheInfernalChoir_Flipped_NotIndestructible()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis" }, advanced: true);
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            GetCard("BaneOfIron", 0);
            GoToEndOfTurn(choir);

            FlipCard(choir);
            Card police = PlayCard("PoliceBackup");
            PrintSpecialStringsForCard(choir.CharacterCard);
            DealDamage(police, choir, 56, DamageType.Projectile, isIrreducible: true);
            //DestroyCard(choir.CharacterCard, legacy.CharacterCard);
            AssertGameOver();
        }

        [Test()]
        public void TestTheInfernalChoir_Flipped_Redirect_Ambiguous()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis" }, advanced: true);
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            FlipCard(choir);

            SetHitPoints(legacy.CharacterCard, 30);
            SetHitPoints(haka.CharacterCard, 30);

            DecisionSelectTarget = haka.CharacterCard;

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, writhe, mainstay, idealist, medico);
            DealDamage(mainstay, choir, 5, DamageType.Melee);
            QuickHPCheck(0, 0, -5, 0, 0, 0, 0);
        }

        [Test()]
        public void TestTheInfernalChoir_Front_EndOfTurnDamage()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "Megalopolis");
            StartGame();

            PlayCard("TakeDown");

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


            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            DealDamage(legacy, haka, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2);

            QuickHPUpdate();
            DealDamage(choir, haka, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2);

            //test ghost after so it's damage mod doesn' change things above
            var c1 = PlayCard("BaneOfIron");
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, c1);
            QuickHPUpdate();
            DealDamage(c1, haka, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);
        }

        [Test()]
        public void TestTheInfernalChoir_Front_SpecialString()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "Megalopolis" }, advanced: true);
            StartGame();


            PrintSpecialStringsForCard(choir.CharacterCard);
            SaveAndLoad();
            
            PrintSpecialStringsForCard(choir.CharacterCard);
        }

        [Test()]
        public void TestTheInfernalChoir_Back_SpecialString()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "Megalopolis" }, advanced: true);
            StartGame();

            FlipCard(choir);

            PrintSpecialStringsForCard(choir.CharacterCard);
            SaveAndLoad();

            PrintSpecialStringsForCard(choir.CharacterCard);
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
        public void TestTheInfernalChoir_Flipped_DestroyCharacters()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis" }, advanced: false);
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

            GameController.HeroTurnTakerControllers.ForEach(htt => RunCoroutine(GameController.BulkMoveCards(htt, htt.TurnTaker.Deck.Cards, htt.TurnTaker.Trash)));

            //Start of Turn
            //all heros with no cards in there deck are destroyed

            GoToStartOfTurn(choir);

            AssertIncapacitated(legacy);
            AssertIncapacitated(haka);
            AssertIncapacitated(sentinels);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test()]
        public void TestTheInfernalChoir_Flipped_Advanced()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis" }, advanced: true);
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            GetCard("BaneOfIron", 0);
            GoToEndOfTurn(choir);

            FlipCard(choir);

            var top = GetTopCardOfDeck(legacy);
            GoToStartOfTurn(legacy);
            AssertOutOfGame(top);
            GoToEndOfTurn(legacy);

            top = GetTopCardOfDeck(haka);
            GoToStartOfTurn(haka);
            AssertOutOfGame(top);
            GoToEndOfTurn(haka);

            top = GetTopCardOfDeck(sentinels);
            GoToStartOfTurn(sentinels);
            AssertOutOfGame(top);
            GoToEndOfTurn(sentinels);
        }

        [Test()]
        public void TestTheInfernalChoir_Flipped_Challenge()
        {
            SetupGameController(new[] { "Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis" }, challenge: true);
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            GoToEndOfTurn(choir);

            FlipCard(choir);

            AddDamageCannotBeRedirectedTrigger(dd => dd.DamageSource.IsSameCard(legacy.CharacterCard), legacy.CharacterCardController.GetCardSource());

            //When {TheInfernalChoir} flips, reduce damage dealt to {TheInfernalChoir} by 2 until the start of the villain turn.
            QuickHPStorage(choir);
            DealDamage(legacy, choir, 3, DamageType.Melee);
            QuickHPCheck(-1);

            GoToNextTurn();
            QuickHPStorage(choir);
            DealDamage(legacy, choir, 3, DamageType.Melee);
            QuickHPCheck(-1);

            //should expire at the start of the next turn
            GoToStartOfTurn(choir);
            QuickHPStorage(choir);
            DealDamage(legacy, choir, 3, DamageType.Melee);
            QuickHPCheck(-3);


        }

        [Test()]
        public void TestVagrantHeartPhase1_DamagePrevention()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

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
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

            var deckCount = GetNumberOfCardsInDeck(legacy);
            var underCount = GetNumberOfCardsUnderCard(heart1);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 36, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNumberOfCardsInDeck(legacy, deckCount);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertIsInPlay(heart2);
            AssertNotFlipped(heart2);
            AssertOutOfGame(heart1);
            AssertNotInPlay(heart1);
        }

        [Test()]
        public void TestVagrantHeartPhase1_FlipFromDraw()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

            var deckCount = GetNumberOfCardsInDeck(legacy);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 35, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNotFlipped(choir.CharacterCard);

            DrawCard(legacy);

            AssertNumberOfCardsInDeck(legacy, deckCount - 1);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertIsInPlay(heart2);
            AssertNotFlipped(heart2);
            AssertOutOfGame(heart1);
            AssertNotInPlay(heart1);
        }

        [Test()]
        public void TestVagrantHeartPhase1_FlipFromPlay()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

            PutOnDeck("InspiringPresence", false);

            var deckCount = GetNumberOfCardsInDeck(legacy);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 35, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNotFlipped(choir.CharacterCard);

            PlayTopCard(legacy);

            AssertNumberOfCardsInDeck(legacy, deckCount - 1);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertIsInPlay(heart2);
            AssertNotFlipped(heart2);
            AssertOutOfGame(heart1);
            AssertNotInPlay(heart1);
        }

        [Test()]
        public void TestVagrantHeartPhase1_DontFlipFromReveal()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Cauldron.Cricket", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

            PutOnDeck("InspiringPresence", false);

            var deckCount = GetNumberOfCardsInDeck(legacy);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 35, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNotFlipped(choir.CharacterCard);

            DecisionSelectLocation = new LocationChoice(legacy.TurnTaker.Deck);

            PlayCard("BeforeTheThunder");

            AssertNumberOfCardsInDeck(legacy, 1);

            AssertNotFlipped(choir.CharacterCard);

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);
        }


        [Test()]
        public void TestVagrantHeartPhase1_TempleOfSzuLong()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Cauldron.Cricket", "TheTempleOfZhuLong");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

            PutOnDeck("InspiringPresence", false);
            var c = GetCard("ShinobiAssassin");

            var deckCount = GetNumberOfCardsInDeck(legacy);

            QuickHPStorage(choir, legacy);
            DealDamage(legacy, choir, 35, DamageType.Melee);
            QuickHPCheck(0, 0);

            AssertNotFlipped(choir.CharacterCard);
            AssertNumberOfCardsInDeck(legacy, 1);
            AssertNotFlipped(choir.CharacterCard);
            PutOnDeck(legacy, c);

            DealDamage(legacy, choir, 2, DamageType.Melee);

            AssertNumberOfCardsInDeck(legacy, deckCount + 1);

            AssertFlipped(choir.CharacterCard);
            AssertInPlayArea(legacy, heart2);
            AssertIsInPlay(heart2);
            AssertNotFlipped(heart2);
            AssertOutOfGame(heart1);
            AssertNotInPlay(heart1);
        }

        [Test()]
        public void TestVagrantHeartPhase1_Defeated()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            AssertInPlayArea(legacy, heart1);
            AssertIsInPlay(heart1);
            AssertOffToTheSide(heart2);
            AssertNotInPlay(heart2);

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

            var p1 = DiscardTopCards(legacy, 1, legacy.CharacterCardController.GetCardSource());
            AssertInTrash(p1);

            MoveAllCards(legacy, legacy.TurnTaker.Deck, legacy.TurnTaker.Trash);

            AssertNumberOfCardsInDeck(legacy, 0);
            int count = GetNumberOfCardsInTrash(legacy);

            var top = legacy.TurnTaker.Trash.GetTopCards(2).ToList();
            var bottom = legacy.TurnTaker.Trash.GetBottomCards(2).ToList();

            AssertOnTopOfLocation(top[0], legacy.TurnTaker.Trash, 0);
            AssertOnTopOfLocation(top[1], legacy.TurnTaker.Trash, 1);
            AssertOnBottomOfLocation(bottom[0], legacy.TurnTaker.Trash, 0);
            AssertOnBottomOfLocation(bottom[1], legacy.TurnTaker.Trash, 1);

            p1 = DiscardTopCards(legacy, 1, legacy.CharacterCardController.GetCardSource());

            AssertNumberOfCardsInTrash(legacy, count);
            AssertNumberOfCardsInDeck(legacy, 0);

            AssertOnTopOfLocation(top[0], legacy.TurnTaker.Trash, 0);
            AssertOnTopOfLocation(top[1], legacy.TurnTaker.Trash, 1);
            AssertOnBottomOfLocation(bottom[0], legacy.TurnTaker.Trash, 0);
            AssertOnBottomOfLocation(bottom[1], legacy.TurnTaker.Trash, 1);
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

            var p3 = DiscardTopCards(omnix, 1);
            AssertNumberOfCardsInTrash(omnix, 2);
        }

        [Test()]
        public void TestRedSunXu_MakeEquipment()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var e1 = PlayCard("TheLegacyRing");
            var e2 = GetCard("StunBolt");

            var card = PlayCard("RedSunXu", 0, true);
            AssertInPlayArea(choir, card);

            AssertAreTargets(c => IsEquipment(c));
            AssertCardHasKeyword(e1, "ghost", true);
            AssertCardHasKeyword(e2, "ghost", true);
        }

        [Test()]
        public void TestRedSunXu_EndOfTurn()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var card = PlayCard("RedSunXu", 0, true);
            AssertInPlayArea(choir, card);

            //2 selected targets deal themselves 3 psychic
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DecisionSelectCards = new[] { legacy.CharacterCard, wraith.CharacterCard };
            GoToEndOfTurn(choir);
            QuickHPCheck(0, -3, 0, -3, 0);
        }

        [Test()]
        public void TestKataMichi_IncreaseDamage_HiddenHeart()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");

            var card = PlayCard("KataMichi", 0, true);
            AssertInPlayArea(choir, card);

            //legacy damage increases, rest not
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(legacy, omnix, 1, DamageType.Cold);
            QuickHPCheck(0, 0, -2, 0, 0);

            //legacy damage increases, rest not
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(omnix, wraith, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, -1, 0);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(choir, legacy, 1, DamageType.Cold);
            QuickHPCheck(0, -1, 0, 0, 0);
        }

        [Test()]
        public void TestKataMichi_IncreaseDamage_SoulRevealed()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            PlayCard("TakeDown");

            var card = PlayCard("KataMichi", 0, true);
            AssertInPlayArea(choir, card);

            //no ones damage increased
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(legacy, omnix, 1, DamageType.Cold);
            QuickHPCheck(0, 0, -1, 0, 0);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(omnix, wraith, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, -1, 0);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(choir, legacy, 1, DamageType.Cold);
            QuickHPCheck(0, -1, 0, 0, 0);
        }

        [Test()]
        public void TestKataMichi_PowerReplaced_Innate()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();


            PlayCard("TakeDown");

            var card = PlayCard("KataMichi", 0, true);
            AssertInPlayArea(choir, card);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            UsePower(legacy);
            QuickHPCheck(0, -3, -2, -2, -2);

            AssertNotUsablePower(legacy, legacy.CharacterCard);
        }

        [Test()]
        public void TestKataMichi_PowerReplaced_Other()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");

            var card = PlayCard("KataMichi", 0, true);
            AssertInPlayArea(choir, card);

            var e1 = PlayCard("StunBolt");

            DecisionSelectTarget = legacy.CharacterCard;
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            UsePower(e1);
            //should have no effect
            QuickHPCheck(0, -1, 0, 0, 0);

        }

        [Test()]
        public void TestKataMichi_PowerReplaced_AppliedNumerology()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheHarpy", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");

            var card = PlayCard("KataMichi", 0, true);
            AssertInPlayArea(choir, card);

            var o1 = PlayCard("AppliedNumerology");
            DecisionYesNo = false;
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, harpy.CharacterCard, card);
            UsePower(harpy);
            QuickHPCheck(0, -4, -3, -3, -3);

            AssertNotUsablePower(harpy, harpy.CharacterCard);
        }

        [Test()]
        public void TestKataMichi_PowerReplaced_KataMichiDestroyed()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");

            var card = PlayCard("KataMichi", 0, true);
            AssertInPlayArea(choir, card);
            SetHitPoints(card, 2);

            DecisionSelectCards = new[] { card, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard };

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard);
            UsePower(wraith);
            QuickHPCheck(0, -2, -2, -2);

            AssertNotUsablePower(wraith, wraith.CharacterCard);
        }

        [Test()]
        public void TestDanchengTheGiant_DrawDamage()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");

            var card = PlayCard("DanchengTheGiant", 0, true);
            AssertInPlayArea(choir, card);

            DecisionYesNo = false;
            QuickHandStorage(legacy, omnix, wraith);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DrawCard(legacy);
            QuickHPCheck(0, -4, 0, 0, 0);
            QuickHandCheck(1, 0, 0);
        }

        [Test()]
        public void TestDanchengTheGiant_DrawDiscardDamage()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");

            var card = PlayCard("DanchengTheGiant", 0, true);
            AssertInPlayArea(choir, card);

            DecisionYesNo = true;
            QuickHandStorage(legacy, omnix, wraith);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DrawCard(legacy);
            QuickHPCheck(0, -1, 0, 0, 0);
            QuickHandCheck(0, 0, 0);

            //make sure the trigger implementation isn't leaking
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(choir.CharacterCard, legacy, 4, DamageType.Infernal);
            QuickHPCheck(0, -4, 0, 0, 0);
        }

        [Test()]
        public void TestDanchengTheGiant_ChoirCannotDealDamage()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            PlayCard("TakeDown");
            AddCannotDealNextDamageTrigger(wraith, choir.CharacterCard);

            var card = PlayCard("DanchengTheGiant", 0, true);
            AssertInPlayArea(choir, card);

            DecisionYesNo = true;
            QuickHandStorage(legacy, omnix, wraith);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DrawCard(legacy);
            QuickHPCheck(0, 0, 0, 0, 0);
            QuickHandCheck(0, 0, 0);

            //make sure the trigger implementation isn't leaking
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(choir.CharacterCard, legacy, 4, DamageType.Infernal);
            QuickHPCheck(0, -4, 0, 0, 0);
        }

        [Test()]
        public void TestBaneOfEmbers_Immunity()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheWraith", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var card = PlayCard("BaneOfEmbers", 0, true);
            AssertInPlayArea(choir, card);

            PrintSpecialStringsForCard(card);


            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(legacy, card, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, -1);

            PrintSpecialStringsForCard(card);

            //now immune
            QuickHPUpdate();
            DealDamage(legacy, card, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0);

            GoToStartOfTurn(legacy);

            PrintSpecialStringsForCard(card);

            //now not
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, wraith.CharacterCard, card);
            DealDamage(legacy, card, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, -1);

            //now immune
            QuickHPUpdate();
            DealDamage(legacy, card, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestBaneOfEmbers_EndOfTurnDamage([Values(4, 3, 2, 1)] int hp)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var card = PlayCard("BaneOfEmbers", 0, true);
            AssertInPlayArea(choir, card);
            SetHitPoints(card, hp);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            GoToEndOfTurn(choir);
            QuickHPCheck(0, -hp, -hp, -hp, 0, 0, 0, 0);
        }



        [Test()]
        public void TestBaneOfIron_DamageReduction()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();


            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var card = PlayCard("BaneOfIron", 0, true);
            AssertInPlayArea(choir, card);

            PrintSpecialStringsForCard(card);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            DealDamage(card, legacy, 1, DamageType.Melee);
            QuickHPCheck(0, -1, 0, 0, 0, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(legacy, omnix, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -1, 0, 0, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(omnix, mainstay, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -1, 0, 0, 0, 0);
        }

        [Test()]
        public void TestBaneOfIron_DestroyOnDestroy()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var o1 = PlayCard("InspiringPresence");
            var o2 = PlayCard("DangerSense");
            var o3 = PlayCard("TheLegacyRing");

            var card = PlayCard("BaneOfIron", 0, true);
            AssertInPlayArea(choir, card);

            DecisionAutoDecideIfAble = false;
            DecisionSelectCards = new[] { o1, o2, o3 };
            DealDamage(legacy, card, 4, DamageType.Melee);

            AssertInTrash(o1, o2, o3);
        }

        [Test()]
        public void TestHauntingNocturne_PlayEclipse([Values(false, true)] bool stageInTrash)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);
            var eclipse = GetCard("Eclipse");

            if (stageInTrash)
                PutInTrash(eclipse);
            else
                PutOnDeck(choir, eclipse);

            QuickShuffleStorage(choir);
            var card = PlayCard("HauntingNocturne", 0, true);
            AssertInPlayArea(choir, card);
            AssertInPlayArea(choir, eclipse);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestHauntingNocturne_IncreaseDamage()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");

            var card = PlayCard("HauntingNocturne", 0, true);
            AssertInPlayArea(choir, card);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            DealDamage(choir.CharacterCard, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0, 0, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(omnix, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestHauntingNocturne_EclipseRFG()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");

            Card eclipse = GetCard("Eclipse");
            MoveCard(choir, eclipse, choir.TurnTaker.OutOfGame);

            var haunting = PlayCard("HauntingNocturne", 0, true);
            AssertInPlayArea(choir, haunting);
            AssertNotInPlayArea(choir, eclipse);

        }

        [Test()]
        public void TestWretchedSymphony_DamageReduction()
        {

            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            Card wretched = GetCard("WretchedSymphony", 0);
            PrintSpecialStringsForCard(wretched);
            choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            GoToStartOfTurn(legacy);

            var card = PlayCard(wretched, true);
            AssertInPlayArea(choir, card);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            DealDamage(legacy.CharacterCard, choir, 2, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(omnix, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestWretchedSymphony_EndOfTurn_SoulRevealed()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var g1 = PlayCard("BaneOfIron", 0, true);
            SetHitPoints(g1, 1);
            var g2 = PlayCard("DanchengTheGiant", 0, true);
            SetHitPoints(g2, 1);

            var card = PlayCard("WretchedSymphony", 0, true);
            PrintSpecialStringsForCard(card);

            AssertInPlayArea(choir, card);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            GoToEndOfTurn(choir);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            AssertHitPoints(g1, g1.MaximumHitPoints.Value);
            AssertHitPoints(g2, g2.MaximumHitPoints.Value);
        }

        [Test()]
        public void TestWretchedSymphony_EndOfTurn_HiddenHeart()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var g2 = PlayCard("DanchengTheGiant", 0, true);
            SetHitPoints(g2, 1);

            var card = PlayCard("WretchedSymphony", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInPlayArea(choir, card);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            GoToEndOfTurn(choir);
            QuickHPCheck(0, 0, -2, -2, -2, -2, -2, 0);
            AssertHitPoints(g2, 1);
        }

        [Test()]
        public void TestWretchedSymphony_EndOfTurn_HiddenHeart_TheSentinels()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = sentinels;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var g2 = PlayCard("DanchengTheGiant", 0, true);
            SetHitPoints(g2, 1);

            var card = PlayCard("WretchedSymphony", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInPlayArea(choir, card);

            DecisionSelectCard = writhe;
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, omnix.CharacterCard, mainstay, writhe, medico, idealist, card);
            GoToEndOfTurn(choir);
            QuickHPCheck(0, -2, -2, -3, 0, -3, -3, 0); //nemesis damage to other sentinels
            AssertHitPoints(g2, 1);
        }

        [Test()]
        public void TestInfernalElegy()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "OmnitronX", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            var e1 = PutOnDeck("ImpendingCasualty");
            var g1 = PutInTrash("BaneOfIron");

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var card = PlayCard("InfernalElegy", 0, true);
            AssertInPlayArea(choir, card);
            AssertInPlayArea(env, e1);

            DestroyCard(e1);

            AssertInPlayArea(choir, g1);
        }

        [Test()]
        public void TestTrueColors_HiddenHeart()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            var card = PlayCard("TrueColors", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInTrash(choir, card);
            QuickHPCheck(0, 0, -6, 0, 0, 0, 0);
        }

        [Test()]
        public void TestTrueColors_SoulRevealed()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            var card = PlayCard("TrueColors", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInTrash(choir, card);
            QuickHPCheck(0, -6, -6, 0, 0, 0, 0);
        }


        [Test()]
        public void TestTheVoicesGather_HiddenHeart()
        {
            Handelabra.Log.ToggleAllLogs(true);

            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var c1 = PutOnDeck("BaneOfIron");
            var c2 = PutOnDeck("TrueColors");
            var c3 = PutOnDeck("Eclipse");

            AssertIsInPlay(heart1);
            AssertNotInPlay(heart2);

            QuickHandStorage(legacy, haka, sentinels);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            var card = PlayCard("TheVoicesGather", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInTrash(choir, card);
            AssertInPlayArea(choir, c1);
            AssertInTrash(c2);
            AssertInTrash(c3);
            QuickHPCheck(0, 0, -2, 0, 0, 0, 0);
            QuickHandCheck(0, 0, 0);
        }

        [Test()]
        public void TestTheVoicesGather_SoulRevealed()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var c1 = PutOnDeck("BaneOfIron");
            var c2 = PutOnDeck("TrueColors");
            var c3 = PutOnDeck("Eclipse");

            AssertNotInPlay(heart1);
            AssertIsInPlay(heart2);

            QuickHandStorage(legacy, haka, sentinels);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            var card = PlayCard("TheVoicesGather", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInTrash(choir, card);
            AssertInPlayArea(choir, c1);
            AssertInTrash(c2);
            AssertInTrash(c3);
            QuickHPCheck(0, 0, -2, 0, 0, 0, 0);
            QuickHandCheck(1, 1, 1);
        }

        [Test()]
        public void TestMoonEater_HiddenHeart([Values(1, 2, 3)] int number)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var inDeck = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInDeck(httc));
            var inTrash = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInTrash(httc));

            DecisionSelectNumber = number;
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, writhe, mainstay, medico, idealist);
            var card = PlayCard("MoonEater", 0, true);
            PrintSpecialStringsForCard(card);

            AssertInTrash(card);
            int expectedDamage = Math.Max(5 - number, 0);
            QuickHPCheck(0, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage);

            foreach (var httc in GameController.HeroTurnTakerControllers)
            {
                AssertNumberOfCardsInDeck(httc, inDeck[httc] - number);
                AssertNumberOfCardsInTrash(httc, inTrash[httc] + number);
            }
        }

        [Test()]
        public void TestMoonEater_SoulRevealed([Values(1, 2, 3)] int number)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var inDeck = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInDeck(httc));
            var inTrash = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInTrash(httc));

            DecisionSelectNumber = number;
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            var card = PlayCard("MoonEater", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInTrash(card);
            int expectedDamage = Math.Max(5 - (2 * number), 0);
            QuickHPCheck(0, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage);

            foreach (var httc in GameController.HeroTurnTakerControllers)
            {
                AssertNumberOfCardsInDeck(httc, inDeck[httc] - number);
                AssertNumberOfCardsInTrash(httc, inTrash[httc] + number);
            }
        }


        [Test()]
        public void TestMoonEater_SoulRevealed_EmptyDeck([Values(1, 2, 3)] int number)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();
            FlipCard(choir.CharacterCard);

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            MoveCards(legacy, FindCardsWhere(c => c.Location.IsDeck && c.IsHero), (Card c) => c.NativeTrash);

            var inDeck = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInDeck(httc));
            var inTrash = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInTrash(httc));

            DecisionSelectNumber = number;
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            var card = PlayCard("MoonEater", 0, true);
            PrintSpecialStringsForCard(card);
            AssertInTrash(card);
            int expectedDamage = Math.Max(5, 0);
            QuickHPCheck(0, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage, -expectedDamage);

            foreach (var httc in GameController.HeroTurnTakerControllers)
            {
                AssertNumberOfCardsInDeck(httc, inDeck[httc]);
                AssertNumberOfCardsInTrash(httc, inTrash[httc]);
            }
        }

        [Test()]
        public void TestBeneathTheFlesh_PlayCard([Values(1, 2, 3)] int number)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var p1 = StackDeck("InspiringPresence");

            var inDeck = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInDeck(httc));
            var inTrash = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInTrash(httc));

            var card = PlayCard("BeneathTheFlesh", 0, true);
            AssertInPlayArea(choir, card);

            DecisionSelectNumber = number;
            DecisionYesNo = true;
            DecisionSelectCard = p1;

            DealDamage(haka, legacy, 3, DamageType.Melee);
            AssertInPlayArea(legacy, p1);

            AssertNumberOfCardsInDeck(legacy, inDeck[legacy] - number);
            AssertNumberOfCardsInTrash(legacy, inTrash[legacy] + number - 1);
        }

        [Test()]
        public void TestBeneathTheFlesh_PlayCard_InitialDamageIncapsHero()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            int number = 2;
            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var p1 = StackDeck("InspiringPresence");

            var inDeck = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInDeck(httc));
            var inTrash = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInTrash(httc));

            var card = PlayCard("BeneathTheFlesh", 0, true);
            AssertInPlayArea(choir, card);

            DecisionSelectNumber = number;
            DecisionYesNo = true;
            DecisionSelectCard = p1;

            SetHitPoints(legacy.CharacterCard, 2);
            AssertNoDecision(SelectionType.DiscardFromDeck);
            DealDamage(haka, legacy, 3, DamageType.Melee);
        }

        [Test()]
        public void TestBeneathTheFlesh_DontCard([Values(1, 2, 3)] int number)
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            AddCannotDealDamageTrigger(choir, choir.CharacterCard);

            var p1 = StackDeck("InspiringPresence");

            var inDeck = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInDeck(httc));
            var inTrash = GameController.HeroTurnTakerControllers.ToDictionary(httc => httc, httc => GetNumberOfCardsInTrash(httc));

            var card = PlayCard("BeneathTheFlesh", 0, true);
            AssertInPlayArea(choir, card);

            DecisionSelectNumber = number;
            DecisionYesNo = false;

            DealDamage(haka, legacy, 3, DamageType.Melee);

            AssertNumberOfCardsInDeck(legacy, inDeck[legacy] - number);
            AssertNumberOfCardsInTrash(legacy, inTrash[legacy] + number);
        }

        [Test()]
        public void TestEclipse()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "TheSentinels", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            PutInTrash(GetRandomCardFromHand(sentinels)); //preload haka's trash

            var card = PlayCard("Eclipse", 0, true);
            AssertInPlayArea(choir, card);

            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, mainstay, writhe, medico, idealist);
            GoToEndOfTurn(choir);
            //have to include The Choir's end of turn damage
            QuickHPCheck(0, -1, -1, -1, -1, -4, -1);
        }

        [Test()]
        public void TestNoWitnesses()
        {
            SetupGameController("Cauldron.TheInfernalChoir", "Legacy", "Haka", "Ra", "Megalopolis");
            //choir.DebugForceHeartPlayer = legacy;
            StartGame();

            DecisionAutoDecideIfAble = true;
            PlayCard("TakeDown");
            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy, haka, ra);
            QuickHPStorage(choir.CharacterCard, legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard);
            var card = PlayCard("NoWitnesses", 0, true);
            AssertInTrash(choir, card);
            QuickHPCheck(0, -3, 0, -3);
            QuickHandCheck(1, 0, -1);
        }
    }
}

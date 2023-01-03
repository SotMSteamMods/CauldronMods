using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.Celadroch;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class CeladrochTests : CauldronBaseTest
    {
        #region HelperFunctions

        protected TokenPool stormPool => celadroch.CharacterCard.FindTokenPool(CeladrochCharacterCardController.StormPoolIdentifier);

        protected void SuppressPillarRewardTriggers(Card pillar)
        {
            var pcc = FindCardController(pillar);
            Assert.IsInstanceOf(typeof(CeladrochPillarCardController), pcc);

            GameController.AddTemporaryTriggerInhibitor<DealDamageAction>(t => t is Trigger<DealDamageAction> dt && dt.CardSource.Card == pillar && !dt.Types.Contains(TriggerType.ReduceDamage), dda => false, pcc.GetCardSource());
            GameController.AddTemporaryTriggerInhibitor<DestroyCardAction>(t => t is Trigger<DestroyCardAction> dt && dt.CardSource.Card == pillar && !dt.Types.Contains(TriggerType.ReduceDamage), dda => false, pcc.GetCardSource());
        }

        protected void SuppressCeladrochMinionPlay()
        {
            GameController.AddTemporaryTriggerInhibitor<CompletedCardPlayAction>(t => t is Trigger<CompletedCardPlayAction> pt && pt.CardSource.Card == celadroch.CharacterCard, pa => false, celadroch.CharacterCardController.GetCardSource());
        }


        protected void SafetyRemovePillars()
        {
            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            MoveCard(celadroch, p1, celadroch.TurnTaker.OutOfGame);
            MoveCard(celadroch, p2, celadroch.TurnTaker.OutOfGame);
            MoveCard(celadroch, p3, celadroch.TurnTaker.OutOfGame);
        }

        #endregion

        [Test()]
        [Order(0)]
        public void TestCeladroch_LoadedProperly()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(celadroch);
            Assert.IsInstanceOf(typeof(CeladrochTurnTakerController), celadroch);
            Assert.IsInstanceOf(typeof(CeladrochCharacterCardController), celadroch.CharacterCardController);
        }

        [Test()]
        public void TestCeladroch_StartGame()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(celadroch, celadroch.CharacterCard);
            AssertNotTarget(celadroch.CharacterCard);
            AssertNotFlipped(celadroch);

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            AssertInPlayArea(celadroch, p1);
            AssertInPlayArea(celadroch, p2);
            AssertInPlayArea(celadroch, p3);

            var topCard = celadroch.TurnTaker.Deck.TopCard;
            AssertCardSpecialString(celadroch.CharacterCard, 1, $"Celadroch's top card is {topCard.Title}");

            AssertNumberOfCardsInRevealed(celadroch, 0);
        }

        [Test()]
        public void TestCeladroch_DeckList()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Haka", "Ra", "Megalopolis");

            AssertCardHasKeyword(celadroch.CharacterCard, "villain", false);

            AssertCardConfiguration("PillarOfNight", new string[] { "relic" }, 25);
            AssertCardConfiguration("PillarOfSky", new string[] { "relic" }, 25);
            AssertCardConfiguration("PillarOfStorms", new string[] { "relic" }, 25);

            AssertCardConfiguration("AvatarOfDeath", new string[] { "avatar" }, 20);

            AssertCardConfiguration("SummersWrath", new string[] { "elemental" }, 5);
            AssertCardConfiguration("WintersBane", new string[] { "elemental" }, 5);
            AssertCardConfiguration("SpringsAtrophy", new string[] { "elemental" }, 5);
            AssertCardConfiguration("AutumnsTorment", new string[] { "elemental" }, 5);

            AssertCardConfiguration("TatteredDevil", new string[] { "demon" }, 10);
            AssertCardConfiguration("HollowAngel", new string[] { "demon" }, 10);

            AssertCardConfiguration("WhisperingBreath", new string[] { "zombie" }, 6);
            AssertCardConfiguration("GraspingBreath", new string[] { "zombie" }, 6);
            AssertCardConfiguration("LeechingBreath", new string[] { "zombie" }, 6);

            AssertCardConfiguration("ForsakenCrusader", new string[] { "chosen" }, 3);
            AssertCardConfiguration("LordOfTheMidnightRevel", new string[] { "chosen" }, 12);
            AssertCardConfiguration("LaughingHag", new string[] { "chosen" }, 5);

            AssertCardConfiguration("ScreamingGale", new string[] { "ongoing" });
            AssertCardConfiguration("HoursTilDawn", new string[] { "ongoing" });
            AssertCardConfiguration("RattlingWind", new string[] { "ongoing" });
            AssertCardConfiguration("NightUnderTheMountain", new string[] { "ongoing" });
            AssertCardConfiguration("LingeringExhalation", new string[] { "ongoing" });

            AssertCardConfiguration("GallowsBlast", new string[] { "one-shot" });
            AssertCardConfiguration("TheMountainsMadness", new string[] { "one-shot" });
        }

        [Test()]
        public void TestCeladroch_FrontCannotPlayCards()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");
            StartGame();

            var card = PlayCard("AvatarOfDeath");

            AssertInDeck(card);
        }

        [Test()]
        public void TestCeladroch_FrontTestTokenGain()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");
            StartGame();

            AssertTokenPoolCount(stormPool, 1);
            GoToEndOfTurn(celadroch);

            GoToStartOfTurn(celadroch);
            AssertTokenPoolCount(stormPool, 2);
            GoToEndOfTurn(celadroch);
        }

        [Test()]
        public void TestCeladroch_FrontFlipOnTokens()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");

            stormPool.AddTokens(2);

            StartGame();

            AssertTokenPoolCount(stormPool, 3);
            GoToEndOfTurn(celadroch);

            GoToStartOfTurn(celadroch);
            AssertFlipped(celadroch);
            AssertTokenPoolCount(stormPool, 4); //flipped side token gain
        }


        [Test()]
        public void TestCeladroch_AdvancedFlipExtraTokens()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Legacy", "Megalopolis" }, advanced: true);

            stormPool.AddTokens(2);

            StartGame();

            AssertTokenPoolCount(stormPool, 3);
            GoToEndOfTurn(celadroch);

            GoToStartOfTurn(celadroch);
            AssertFlipped(celadroch);
            AssertTokenPoolCount(stormPool, 6); //flipped side token gain
        }

        [Test()]
        public void TestCeladroch_NormalFrontNoRelicDR()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Legacy", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            QuickHPStorage(p1, p2, p3);
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning);
            QuickHPCheck(-1, -1, -1);

            QuickHPUpdate();
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning, true);
            QuickHPCheck(-1, -1, -1);
        }

        [Test()]
        public void TestCeladroch_AdvancedFrontRelicDr()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Legacy", "Megalopolis" }, advanced: true);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            QuickHPStorage(p1, p2, p3);
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning);
            QuickHPCheckZero();

            QuickHPUpdate();
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning, true);

            QuickHPCheck(-1, -1, -1);
        }


        [Test()]
        public void TestCeladroch_PillarRewards_NumberOfTriggers([Values(3, 4, 5)] int H)
        {
            var math = new CeladrochPillarRewards(H);

            int[] beforeHp = new int[] { 25, 22, 21, 21, 12, 7, 2, 25, 10, -4 };
            int[] afterHp = new int[] { 22, 21, 21, 12, 7, 2, -20, -100, 20, -10 };

            int[] answers3 = new int[] { 0, 1, 0, 2, 1, 1, 1, 6, 0, 0 };
            int[] answers4 = new int[] { 0, 0, 0, 2, 1, 1, 1, 5, 0, 0 };
            int[] answers5 = new int[] { 0, 0, 0, 2, 1, 0, 1, 4, 0, 0 };

            var answers = H <= 3 ? answers3 : H == 4 ? answers4 : answers5;

            for (int index = 0; index < beforeHp.Length; index++)
            {
                int before = beforeHp[index];
                int after = afterHp[index];
                int expected = answers[index];

                int actual = math.NumberOfTriggers(before, after);

                Assert.AreEqual(expected, actual, $"Test {index}: NumberOfTriggers ({H},{before},{after}) Expect={expected}, Result = {actual}");
            }
        }

        [Test()]
        public void TestCeladroch_PillarRewards_HpTilNextTrigger([Values(3, 4, 5)] int H)
        {
            var math = new CeladrochPillarRewards(H);

            int[] currentHp = new int[] { 25, 22, 21, 21, 12, 7, 2, 25, 10, -4 };

            int[] answers3 = new int[] { 4, 1, 4, 4, 3, 2, 1, 4, 1, 0 };
            int[] answers4 = new int[] { 5, 2, 1, 1, 2, 2, 2, 5, 5, 0 };
            int[] answers5 = new int[] { 6, 3, 2, 2, 5, 6, 1, 6, 3, 0 };

            var answers = H <= 3 ? answers3 : H == 4 ? answers4 : answers5;

            for (int index = 0; index < currentHp.Length; index++)
            {
                int hp = currentHp[index];
                int expected = answers[index];

                int actual = math.HpTillNextTrigger(hp);

                Assert.AreEqual(expected, actual, $"Test {index}: HpTillNextTrigger ({H},{hp}) Expect={expected}, Result = {actual}");
            }
        }

        [Test()]
        public void TestCeladroch_PillarRewards_RemainingTriggers([Values(3, 4, 5)] int H)
        {
            var math = new CeladrochPillarRewards(H);

            int[] currentHp = new int[] { 25, 22, 21, 21, 12, 7, 2, 25, 10, -4 };

            int[] answers3 = new int[] { 6, 6, 5, 5, 3, 2, 1, 6, 3, 0 };
            int[] answers4 = new int[] { 5, 5, 5, 5, 3, 2, 1, 5, 2, 0 };
            int[] answers5 = new int[] { 4, 4, 4, 4, 2, 1, 1, 4, 2, 0 };

            var answers = H <= 3 ? answers3 : H == 4 ? answers4 : answers5;

            for (int index = 0; index < currentHp.Length; index++)
            {
                int hp = currentHp[index];
                int expected = answers[index];

                int actual = math.RemainingRewards(hp);

                Assert.AreEqual(expected, actual, $"Test {index}: RemainingTriggers ({H},{hp}) Expect={expected}, Result = {actual}");
            }
        }


        [Test()]
        public void TestCeladroch_PillarCard_CannotGainHp([Values("PillarOfNight", "PillarOfSky", "PillarOfStorms")] string pillar)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var p = GetCard(pillar);
            SetHitPoints(p, 24);

            QuickHPStorage(p);
            GainHP(p, 1);
            QuickHPCheck(0);

            SetHitPoints(haka.CharacterCard, 20);
            QuickHPStorage(haka);
            GainHP(haka.CharacterCard, 3);
            QuickHPCheck(3);
        }

        [Test()]
        public void TestCeladroch_PillarCard_RemoveFromGameDamage([Values("PillarOfNight", "PillarOfSky", "PillarOfStorms")] string pillar)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var p = GetCard(pillar);
            SuppressPillarRewardTriggers(p);

            DealDamage(ra, p, 30, DamageType.Fire);

            AssertOutOfGame(p);
        }

        [Test()]
        public void TestCeladroch_PillarCard_RemoveFromGameDestroy([Values("PillarOfNight", "PillarOfSky", "PillarOfStorms")] string pillar)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var p = GetCard(pillar);
            SuppressPillarRewardTriggers(p);
            DestroyCard(p, ra.CharacterCard);

            AssertOutOfGame(p);
        }

        [Test()]
        public void TestCeladroch_PillarCard_RemoveFromGameMove()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            SuppressPillarRewardTriggers(p1);
            SuppressPillarRewardTriggers(p2);
            SuppressPillarRewardTriggers(p3);

            MoveCard(ra, p1, p1.NativeDeck);
            AssertOutOfGame(p1);

            MoveCard(ra, p2, celadroch.TurnTaker.Trash);
            AssertOutOfGame(p2);

            MoveCard(ra, p3, ra.TurnTaker.PlayArea);
            AssertIsInPlay(p3);
        }

        [Test()]
        public void TestCeladroch_PillarCard_RemoveFromGameWeirdStuff()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            MoveCard(ra, p1, ra.CharacterCard.UnderLocation);
            AssertIsInPlay(p1);

            MoveCard(ra, p2, ra.CharacterCard.NextToLocation);
            AssertOutOfGame(p2);

            FlipCard(p3);
            AssertOutOfGame(p3);
        }

        [Test()]
        public void TestPillarOfNight_RewardTrigger()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            //H = 3, 4 damage per trigger, 6 total triggers possible
            DecisionSelectTurnTaker = ra.HeroTurnTaker;
            QuickHandStorage(ra);
            DealDamage(ra, p1, 4, DamageType.Cold);
            QuickHandCheck(1);

            DealDamage(ra, p1, 8, DamageType.Cold);
            QuickHandCheck(2);

            DealDamage(ra, p1, 8, DamageType.Cold);
            QuickHandCheck(2);

            //over kill ensure trigger fires on destruction
            DealDamage(ra, p1, 20, DamageType.Cold);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestPillarOfSky_RewardTrigger()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            SuppressPillarRewardTriggers(p1);

            //setup enough powers
            DecisionSelectTarget = p1;
            var h1 = PlayCard("MotivationalCharge");
            var h2 = PlayCard("Mere");
            var h3 = PlayCard("LivingConflagration");

            //H = 3, 4 damage per trigger, 6 total triggers possible
            //sequence turntakers and powers, damaging p1 as the target soak.
            DecisionSelectTurnTakers = new[] { legacy.HeroTurnTaker, ra.HeroTurnTaker, haka.HeroTurnTaker, legacy.HeroTurnTaker, ra.HeroTurnTaker, haka.HeroTurnTaker };
            DecisionSelectCards = new[]
            {
                legacy.CharacterCard,
                ra.CharacterCard, p1,
                haka.CharacterCard, p1,
                h1, p1,
                h2, p1,
                h3, p1,
            };

            DealDamage(ra, p2, 4, DamageType.Cold);

            AssertNotUsablePower(legacy, legacy.CharacterCard);

            DealDamage(ra, p2, 8, DamageType.Cold);
            AssertNotUsablePower(ra, ra.CharacterCard);
            AssertNotUsablePower(haka, haka.CharacterCard);

            DealDamage(ra, p2, 8, DamageType.Cold);
            AssertNotUsablePower(legacy, h1);
            AssertNotUsablePower(ra, h2);

            //over kill ensure trigger fires on destruction
            DealDamage(ra, p2, 20, DamageType.Cold);
            AssertNotUsablePower(haka, h3);
        }


        [Test()]
        public void TestPillarOfStorms_RewardTrigger()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            SuppressPillarRewardTriggers(p1);

            //preload cards into hands
            var h1 = MakeCustomHeroHand(legacy, new[] { "InspiringPresence", "BackFistStrike", "BackFistStrike" });

            var h2 = MakeCustomHeroHand(haka, new[] { "Dominion", "ElbowSmash", "ElbowSmash" });

            var h3 = MakeCustomHeroHand(ra, new[] { "ImbuedFire", "FireBlast", "FireBlast" });

            //H = 3, 4 damage per trigger, 6 total triggers possible
            //sequence turntakers and cards, damaging p1 as the target soak.
            DecisionSelectTurnTakers = new[] { legacy.HeroTurnTaker, legacy.HeroTurnTaker, haka.HeroTurnTaker, haka.HeroTurnTaker, ra.HeroTurnTaker, ra.HeroTurnTaker };
            DecisionSelectCards = new[]
            {
                h1[1], p1,
                h1[2], p1,
                h2[1], p1,
                h2[2], p1,
                h3[1], p1,
                h3[2], p1,
            };

            DealDamage(ra, p3, 4, DamageType.Cold);
            AssertInTrash(h1[1]);

            DealDamage(ra, p3, 8, DamageType.Cold);
            AssertInTrash(h1[2]);
            AssertInTrash(h2[1]);

            DealDamage(ra, p3, 8, DamageType.Cold);
            AssertInTrash(h2[2]);
            AssertInTrash(h3[1]);

            //over kill ensure trigger fires on destruction
            DealDamage(ra, p3, 20, DamageType.Cold);
            AssertInTrash(h3[2]);
        }

        [Test()]
        public void TestCeladroch_PillarCard_ProvideCeladrochDR()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            SuppressPillarRewardTriggers(p1);
            SuppressPillarRewardTriggers(p2);
            SuppressPillarRewardTriggers(p3);

            AssertIsTarget(celadroch.CharacterCard, 100);

            QuickHPStorage(celadroch.CharacterCard);
            DealDamage(ra, celadroch.CharacterCard, 5, DamageType.Cold);
            QuickHPCheck(-2);

            DestroyCard(p1);

            QuickHPStorage(celadroch.CharacterCard);
            DealDamage(ra, celadroch.CharacterCard, 5, DamageType.Cold);
            QuickHPCheck(-3);

            DestroyCard(p2);

            QuickHPStorage(celadroch.CharacterCard);
            DealDamage(ra, celadroch.CharacterCard, 5, DamageType.Cold);
            QuickHPCheck(-4);

            DestroyCard(p3);

            QuickHPStorage(celadroch.CharacterCard);
            DealDamage(ra, celadroch.CharacterCard, 5, DamageType.Cold);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestCeladroch_EndOfTurnDamage([Values(1, 2, 4)] int tokensToTest)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StartGame(false);

            RemoveTokensFromPool(stormPool, 4); //zero out pool
            AssertTokenPoolCount(stormPool, 0);

            AddTokensToPool(stormPool, tokensToTest);

            QuickHPStorage(ra, haka, legacy);

            GoToEndOfTurn(celadroch);
            int expectedDamage = tokensToTest - 2;
            if (expectedDamage < 0) expectedDamage = 0;

            Console.WriteLine("DEBUG: Expected Damage = " + expectedDamage.ToString());

            QuickHPCheck(0, -expectedDamage, -expectedDamage);
        }

        [Test()]
        public void TestCeladroch_PlayCardToRemoveTokens()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);

            StackAfterShuffle(celadroch.TurnTaker.Deck, new[] { "AvatarOfDeath" });

            AddTokensToPool(stormPool, 3);
            DecisionYesNo = true;
            StartGame(false);

            var card = GetCard("AvatarOfDeath");
            AssertInPlayArea(celadroch, card);
            AssertTokenPoolCount(stormPool, 2);
        }

        [Test()]
        public void TestCeladroch_NoCardsPlayedDestoryCards()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);

            StackAfterShuffle(celadroch.TurnTaker.Deck, new[] { "AvatarOfDeath" });

            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);

            var o1 = PlayCard("Fortitude");
            var o2 = PlayCard("TheLegacyRing");
            var o3 = PlayCard("ImbuedFire");
            var o4 = PlayCard("Dominion");

            DecisionSelectCards = new[] { haka.CharacterCard, o1, o2, o3 };

            GoToEndOfTurn(celadroch);

            AssertInTrash(o1);
            AssertInTrash(o2);
            AssertInTrash(o3);
            AssertIsInPlay(o4);
        }

        [Test()]
        public void TestCeladroch_CardsPlayedNoDestroyCards()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            StackAfterShuffle(celadroch.TurnTaker.Deck, new[] { "TatteredDevil" });

            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);

            var o1 = PlayCard("Fortitude");
            var o2 = PlayCard("TheLegacyRing");
            var o3 = PlayCard("ImbuedFire");
            var o4 = PlayCard("Dominion");

            GoToPlayCardPhase(celadroch);
            PlayTopCard(celadroch);

            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new[] { o1, o2, o3 };

            GoToEndOfTurn(celadroch);

            AssertIsInPlay(o1);
            AssertIsInPlay(o2);
            AssertIsInPlay(o3);
            AssertIsInPlay(o4);
        }


        [Test()]
        public void TestCeladroch_PlayForsakenCrusader()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);

            var card = PutInTrash("ForsakenCrusader");

            GoToEndOfTurn(celadroch);
            AssertInPlayArea(celadroch, card);

        }

        [Test()]
        [Sequential]
        public void TestCeladroch_BulkPlayMinions([Values("elemental", "zombie", "demon", "chosen")] string keyword, [Values(4, 3, 2, 3)] int expectedCards)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);

            var cards = FindCardsWhere(c => c.DoKeywordsContain(keyword)).ToList();
            Assert.IsTrue(expectedCards == cards.Count, $"Test Setup Issue, should have {expectedCards} {keyword} cards.");

            var topCard = cards[base.GameController.Game.RNG.Next(0, cards.Count - 1)];
            StackAfterShuffle(celadroch.TurnTaker.Deck, new[] { topCard.Identifier });
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SafetyRemovePillars();

            QuickShuffleStorage(celadroch.TurnTaker.Deck);
            GoToPlayCardPhase(celadroch);
            PlayTopCard(celadroch);
            AssertIsInPlay(topCard);

            foreach (var c in cards)
            {
                AssertIsInPlay(c);
            }
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestCeladroch_BulkPlayMinions_BugFix()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "SilverGulch1883" }, advanced: false);

            string keyword = "elemental";
            int expectedCards = 4;
            var cards = FindCardsWhere(c => c.DoKeywordsContain(keyword)).ToList();
            Assert.IsTrue(expectedCards == cards.Count, $"Test Setup Issue, should have {expectedCards} {keyword} cards.");

            Card summersWrath = GetCard("SummersWrath");
            var topCard = summersWrath;
            StackAfterShuffle(celadroch.TurnTaker.Deck, new[] { topCard.Identifier });
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SafetyRemovePillars();

            GoToPlayCardPhase(celadroch);

            IEnumerable<Card> wagons = FindCardsWhere(c => c.Identifier == "ExplosivesWagon");
            PlayCards(wagons);

            PlayTopCard(celadroch);
            AssertInTrash(summersWrath);
            
        }

        [Test()]
        public void TestZombies_MoveNextToAndDealDamage([Values("GraspingBreath", "LeechingBreath", "WhisperingBreath")] string zombie)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            //setup haka to be the highest of all
            PlayCard("Dominion");
            DrawCard(haka);
            SetHitPoints(legacy, 20);

            GoToPlayCardPhase(celadroch);

            var card = GetCard(zombie);
            PlayCard(card);
            AssertNextToCard(card, haka.CharacterCard);

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(ra, haka, legacy);

            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);
            GoToEndOfTurn(celadroch);

            QuickHPCheck(0, -3, 0);
        }

        [Test()]
        public void TestZombies_IncapBehavior([Values("GraspingBreath", "LeechingBreath", "WhisperingBreath")] string zombie)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            //setup haka to be the highest of all
            PlayCard("Dominion");
            DrawCard(haka);
            SetHitPoints(legacy, 20);

            GoToPlayCardPhase(celadroch);

            var card = GetCard(zombie);
            PlayCard(card);
            AssertNextToCard(card, haka.CharacterCard);

            DecisionAutoDecideIfAble = true;
            GoToEndOfTurn(celadroch);

            DealDamage(celadroch.CharacterCard, haka.CharacterCard, 99, DamageType.Cold);

            AssertNextToCard(card, haka.CharacterCard);
        }

        [Test()]
        public void TestGraspingBreath_PreventAction()
        {
            //[Values("GraspingBreath", "LeechingBreath", "WhisperingBreath")] string zombie

            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            //setup haka to be the highest of all
            PlayCard("Dominion");
            DrawCard(haka);
            SetHitPoints(legacy, 20);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("GraspingBreath");
            PlayCard(card);
            AssertNextToCard(card, haka.CharacterCard);

            QuickHandStorage(haka);
            DrawCard(haka);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestLeechingBreath_PreventAction()
        {
            //[Values("GraspingBreath", "LeechingBreath", "WhisperingBreath")] string zombie

            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            //setup haka to be the highest of all
            PlayCard("Dominion");
            DrawCard(haka);
            SetHitPoints(legacy, 20);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("LeechingBreath");
            PlayCard(card);
            AssertNextToCard(card, haka.CharacterCard);

            AssertNotUsablePower(haka, haka.CharacterCard);
            UsePower(haka.CharacterCard);
            AssertNotUsablePower(haka, haka.CharacterCard);
        }

        [Test()]
        public void TestWhisperingBreath_PreventAction()
        {
            //[Values("GraspingBreath", "LeechingBreath", "WhisperingBreath")] string zombie

            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            //setup haka to be the highest of all
            PlayCard("Dominion");
            DrawCard(haka);
            SetHitPoints(legacy, 20);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("WhisperingBreath");
            PlayCard(card);
            AssertNextToCard(card, haka.CharacterCard);

            AssertCannotPlayCards(haka);
        }


        [Test()]
        public void TestCelestials_EndOfTurnDamage([Values("HollowAngel", "TatteredDevil")] string celestials)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            GoToPlayCardPhase(celadroch);

            var card = GetCard(celestials);
            PlayCard(card);

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(ra, haka, legacy);

            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);
            GoToEndOfTurn(celadroch);

            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestCelestials_SoloNotImmune([Values("HollowAngel", "TatteredDevil")] string celestials)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            GoToPlayCardPhase(celadroch);

            var card = GetCard(celestials);
            PlayCard(card);

            DecisionSelectTarget = card;
            QuickHPStorage(card);
            DealDamage(haka, card, 2, DamageType.Cold);
            DealDamage(legacy, card, 2, DamageType.Cold);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestCelestials_TestImmunity()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            GoToPlayCardPhase(celadroch);

            var c1 = GetCard("HollowAngel");
            var c2 = GetCard("TatteredDevil");
            PlayCard(c1);
            PlayCard(c2);

            QuickHPStorage(c1, c2);
            DealDamage(haka, c1, 2, DamageType.Cold);
            DealDamage(legacy, c1, 2, DamageType.Cold);
            QuickHPCheck(-2, 0);

            QuickHPStorage(c1, c2);
            DealDamage(haka, c2, 2, DamageType.Cold);
            DealDamage(legacy, c2, 2, DamageType.Cold);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestCelestials_TestImmunity_DamageInterrupt()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Cauldron.BlackwoodForest" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            GoToPlayCardPhase(celadroch);

            DestroyNonCharacterVillainCards();

            var c1 = GetCard("HollowAngel");
            var c2 = GetCard("TatteredDevil");
            PlayCard(c1);
            PlayCard(c2);

            Card overgrownCathedral = PlayCard("OvergrownCathedral");
            DecisionSelectCards = new Card[] { celadroch.CharacterCard, ra.CharacterCard, haka.CharacterCard, legacy.CharacterCard, c2 };
            QuickHPStorage(c1, c2);
            DealDamage(haka, c1, 2, DamageType.Cold);
            DealDamage(legacy, c2, 2, DamageType.Cold);
            DealDamage(legacy, c1, 2, DamageType.Cold);
            DealDamage(ra, c1, 2, DamageType.Cold);
            DealDamage(ra, c2, 2, DamageType.Cold);

            // c1 -> haka (-2) + legacy (-2) + ra (0)
            // c2 -> overgrown cathedral (-1) + legacy (0) + ra (-2)
            QuickHPCheck(-4, -3);

            QuickHPStorage(c1, c2);
            DealDamage(ra, c1, 2, DamageType.Cold);
            DealDamage(haka, c2, 2, DamageType.Cold);
            DealDamage(legacy, c2, 2, DamageType.Cold);
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestOngoings_PlayCard([Values("HoursTilDawn", "LingeringExhalation", "NightUnderTheMountain", "RattlingWind", "ScreamingGale")] string identifier)
        {

            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StackAfterShuffle(celadroch.TurnTaker.Deck, new[] { identifier, "AvatarOfDeath" });
            StartGame(false);

            var card = GetTopCardOfDeck(celadroch);
            var second = GetTopCardOfDeck(celadroch, 1);

            GoToPlayCardPhase(celadroch);
            PlayTopCard(celadroch);
            AssertInPlayArea(celadroch, card);
            AssertInPlayArea(celadroch, second);
        }


        [Test()]
        public void TestAvatarOfDeath_StartOfTurn()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            GoToEndOfTurn(env);

            var o1 = PlayCard(haka, "Dominion", 0, true);
            AssertIsInPlay(o1);

            var card = GetCard("AvatarOfDeath");
            PlayCard(card);
            AssertIsInPlay(card);

            GoToStartOfTurn(celadroch);

            AssertInTrash(o1);
        }

        [Test()]
        public void TestAvatarOfDeath_EndOfTurn()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            var card = GetCard("AvatarOfDeath");
            PlayCard(card);
            AssertIsInPlay(card);


            DecisionAutoDecideIfAble = true;
            QuickHPStorage(ra, haka, legacy);

            //suppress celadroch's end of turn damage
            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);

            GoToEndOfTurn(celadroch);

            QuickHPCheck(-3, -3, -3);
        }

        public void TestAvatarOfDeath_ReduceDamage()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            StartGame(false);
            SuppressCeladrochMinionPlay();

            var card = GetCard("AvatarOfDeath");
            PlayCard(card);
            AssertIsInPlay(card);

            QuickHPStorage(card);
            DealDamage(haka, card, 2, DamageType.Projectile);
            QuickHPCheck(-1);

            QuickHPStorage(card);
            DealDamage(haka, card, 2, DamageType.Projectile, true);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestScreamingGale()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", "TatteredDevil" });
            StartGame(false);

            var card = PlayCard("ScreamingGale");
            AssertInPlayArea(celadroch, card);

            var top = GetCard("AvatarOfDeath");
            AssertInPlayArea(celadroch, top);

            QuickHPStorage(ra, haka, legacy);
            DealDamage(celadroch.CharacterCard, haka, 2, DamageType.Psychic);
            DealDamage(top, ra, 2, DamageType.Radiant);
            QuickHPCheck(-3, -3, 0);
        }


        [Test()]
        public void TestAutumnsTorment()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", "TatteredDevil" });
            StartGame(false);

            var e1 = PlayCard("PlummetingMonorail");
            AssertInPlayArea(env, e1);

            var card = PlayCard("AutumnsTorment");
            AssertInPlayArea(celadroch, card);

            AssertInTrash(e1);

            QuickHPStorage(ra, haka, legacy);
            PlayCard("Dominion");

            QuickHPCheck(0, -2, 0);
        }

        [Test()]
        public void TestSummersWrath()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "SummersWrath" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("SummersWrath");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            QuickHPStorage(ra, haka, legacy);

            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);

            GoToEndOfTurn(celadroch);

            QuickHPCheck(-2, -2, -2);

            //test dr
            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0); //immune

            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, celadroch.CharacterCard, 3, DamageType.Cold);
            QuickHPCheck(0, 0); //dr reduces to zero

            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0); //still immune

            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, celadroch.CharacterCard, 3, DamageType.Cold, true);
            DealDamage(haka.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(-1, -3); //no longer immune

            GoToStartOfTurn(ra);

            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0); //immune again

        }

        [Test()]
        public void TestWintersBane()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "WintersBane" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("WintersBane");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            QuickHPStorage(ra, haka, legacy);

            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);

            GoToEndOfTurn(celadroch);

            QuickHPCheck(0, -3, 0);

            //test dr
            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, card, 1, DamageType.Cold);
            QuickHPCheck(0, 0);

            QuickHPStorage(card, celadroch.CharacterCard);
            DealDamage(haka.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(-1, 0);
        }


        [Test()]
        public void TestSpringsAtrophy()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "SpringsAtrophy" });
            StartGame(false);

            SetHitPoints(ra.CharacterCard, 15);
            GoToPlayCardPhase(celadroch);

            var card = GetCard("SpringsAtrophy");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            QuickHPStorage(ra, haka, legacy);

            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);

            GoToEndOfTurn(celadroch);
            QuickHPCheck(-2, 0, 0);
        }


        [Test()]
        public void TestForsakenCrusader()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "ForsakenCrusader", "TatteredDevil" });
            StartGame(false);

            SetHitPoints(ra.CharacterCard, 15);
            GoToPlayCardPhase(celadroch);

            var card = GetCard("ForsakenCrusader");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            QuickHPStorage(ra, haka, legacy);

            AddCannotDealDamageTrigger(celadroch, celadroch.CharacterCard);
            GoToEndOfTurn(celadroch);

            QuickHPCheck(0, -1, -1);

            GoToEndOfTurn(env);
            AssertInPlayArea(celadroch, card);

            GoToStartOfTurn(celadroch);
            var devil = GetCard("TatteredDevil");
            AssertInPlayArea(celadroch, devil);
        }

        [Test()]
        public void TestLaughingHag([Values("Dominion", "TheLegacyRing")] string cardToDestroy)
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "LaughingHag", "TatteredDevil" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("LaughingHag");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            var destroy = PlayCard(cardToDestroy);

            GoToEndOfTurn(celadroch);

            AssertInTrash(destroy);

            //test immune to fire, lightning, cold, toxic
            QuickHPStorage(card);
            DealDamage(celadroch, card, 1, DamageType.Fire, true);
            DealDamage(haka, card, 1, DamageType.Lightning, true);
            DealDamage(ra, card, 1, DamageType.Cold, true);
            DealDamage(card, card, 1, DamageType.Toxic, true);
            DealDamage(haka, card, 1, DamageType.Melee);
            QuickHPCheck(-1);

            //test hero's take additional damage
            QuickHPStorage(ra, haka, legacy);
            DealDamage(celadroch, ra, 1, DamageType.Fire);
            DealDamage(ra, haka, 1, DamageType.Melee);
            DealDamage(card, legacy, 1, DamageType.Projectile);
            QuickHPCheck(-2, -2, -2);
        }


        [Test()]
        public void TestGallowsBlast_Play()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;

            StackDeckAfterShuffle(celadroch, new[] { "GallowsBlast" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("GallowsBlast");

            base.AssertNumberOfCardsInTrash(ra, 0);
            base.AssertNumberOfCardsInTrash(haka, 0);
            base.AssertNumberOfCardsInTrash(legacy, 0);
            base.AssertNumberOfCardsInHand(ra, 4);
            base.AssertNumberOfCardsInHand(haka, 4);
            base.AssertNumberOfCardsInHand(legacy, 4);

            QuickHPStorage(ra, haka, legacy);
            PlayCard(card);
            AssertInTrash(celadroch, card);
            QuickHPCheck(-5, -5, -5);

            base.AssertNumberOfCardsInTrash(ra, 1);
            base.AssertNumberOfCardsInTrash(haka, 1);
            base.AssertNumberOfCardsInTrash(legacy, 1);

            base.AssertNumberOfCardsInHand(ra, 3);
            base.AssertNumberOfCardsInHand(haka, 3);
            base.AssertNumberOfCardsInHand(legacy, 3);
        }

        [Test()]
        public void TestGallowsBlast_Play_DamageReduction()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;

            StackDeckAfterShuffle(celadroch, new[] { "GallowsBlast" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("GallowsBlast");


            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(2);
            effect.SourceCriteria.IsSpecificCard = celadroch.CharacterCard;
            effect.UntilStartOfNextTurn(celadroch.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(celadroch.CharacterCardController)));

            QuickHPStorage(ra, haka, legacy);
            PlayCard(card);
            AssertInTrash(celadroch, card);
            QuickHPCheck(-3, -3, -3);
        }


        [Test()]
        public void TestGallowsBlast_StatusEffect()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;

            StackDeckAfterShuffle(celadroch, new[] { "GallowsBlast" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);

            var card = GetCard("GallowsBlast");

            AssertNumberOfStatusEffectsInPlay(0);

            PlayCard(card);
            AssertInTrash(celadroch, card);

            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectsContains("");

        }


        [Test()]
        public void TestHoursTilDawn()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", "TatteredDevil" });
            StartGame(false);

            SetHitPoints(celadroch.CharacterCard, 70);

            var card = PlayCard("HoursTilDawn");
            AssertInPlayArea(celadroch, card);

            var top = GetCard("AvatarOfDeath");
            SetHitPoints(top, 15);
            AssertInPlayArea(celadroch, top);

            QuickHPStorage(celadroch.CharacterCard, top);
            GoToEndOfTurn(celadroch);
            QuickHPCheck(2, 2);

            QuickHPStorage(celadroch.CharacterCard, top);
            DestroyCard(card);
            QuickHPCheck(10, 0);
        }

        [Test()]
        public void TestHoursTilDawn_PlayedOnFront()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);

            StartGame();

            var card = PlayCard("HoursTilDawn", isPutIntoPlay: true);
            AssertInPlayArea(celadroch, card);

            DestroyCard(card);
            AssertNotGameOver();
        }

        [Test()]
        public void TestNightUnderTheMountain_DestroyAtStart()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", "TatteredDevil" });
            StartGame(false);

            var card = PlayCard("NightUnderTheMountain");
            AssertInPlayArea(celadroch, card);

            var top = GetCard("AvatarOfDeath");
            AssertInPlayArea(celadroch, top);

            GoToEndOfTurn(celadroch);

            GoToEndOfTurn(env);

            AssertInPlayArea(celadroch, card);
            AssertNumberOfStatusEffectsInPlay(0);
            GoToStartOfTurn(celadroch);
            AssertInTrash(card);

            AssertNumberOfStatusEffectsInPlay(1);
        }

        [Test()]
        public void TestNightUnderTheMountain_StatusEffect()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", "TatteredDevil" });
            StartGame(false);

            GoToPlayCardPhase(celadroch);
            PrintSeparator("IN PLAY PHASE");

            var card = PlayCard("NightUnderTheMountain");
            AssertInPlayArea(celadroch, card);

            var top = GetCard("AvatarOfDeath");
            AssertInPlayArea(celadroch, top);

            AssertNumberOfStatusEffectsInPlay(0);
            DestroyCard(card);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(ra, haka, legacy);

            // verify the +2 buff
            DealDamage(celadroch, ra.CharacterCard, 2, DamageType.Infernal);
            QuickHPCheck(-4, 0, 0);
            QuickHPUpdate();
            DealDamage(top, ra.CharacterCard, 2, DamageType.Infernal);
            QuickHPCheck(-4, 0, 0);
            QuickHPUpdate();


            GoToEndOfTurn(celadroch);

            // check that buff effect has expired
            AssertNumberOfStatusEffectsInPlay(0);

            //includes cela's end of turn
            // buff should have expired by now
            // cela = 2 to two highest
            // death = H to all
            QuickHPCheck(-3, -5, -5);


        }


        [Test()]
        public void TestRattlingWind()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", "TatteredDevil" });
            StartGame(false);

            var card = PlayCard("RattlingWind");
            AssertInPlayArea(celadroch, card);

            var top = GetCard("AvatarOfDeath");
            AssertInPlayArea(celadroch, top);

            QuickHPStorage(ra, haka, legacy);
            DrawCard(ra, 2);
            QuickHPCheck(-2, 0, 0);

            QuickHPStorage(ra, haka, legacy);
            DrawCard(legacy, 1);
            QuickHPCheck(0, 0, -1);

            QuickHPStorage(celadroch, ra, haka, legacy);
            DestroyCard(card);
            QuickHPCheck(0, -1, -1, -1);
        }


        [Test()]
        public void TestTheMountainsMadess()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;

            StartGame(false);

            var card = GetCard("TheMountainsMadness");

            MoveCards(celadroch, celadroch.TurnTaker.Deck.Cards.Where(c => c.DoKeywordsContain("chosen")), celadroch.TurnTaker.Trash);
            MoveCards(celadroch, celadroch.TurnTaker.Deck.Cards.TakeRandom(10, GameController.Game.RNG), celadroch.TurnTaker.Trash);
            StackDeck(new[] { "AvatarOfDeath" });
            var tCount = celadroch.TurnTaker.Trash.Cards.Count(celadroch => celadroch.IsTarget);

            GoToPlayCardPhase(celadroch);

            QuickShuffleStorage(celadroch);
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath", });

            PlayCard(card);
            AssertInTrash(celadroch, card);

            var top = GetCard("AvatarOfDeath");
            AssertInPlayArea(celadroch, top);

            tCount = celadroch.TurnTaker.Trash.Cards.Count(celadroch => celadroch.IsTarget);

            Assert.AreEqual(0, tCount);
        }


        [Test()]
        public void TestLingeringExhalation_TestDoesntTriggerMinionPlay()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath" });
            StartGame(false);
            SafetyRemovePillars();

            var card = GetCard("LingeringExhalation");
            var avatar = GetCard("AvatarOfDeath");
            var c1 = PutInTrash("WintersBane");
            var c2 = GetCard("SummersWrath");
            var c3 = GetCard("SpringsAtrophy");
            var c4 = GetCard("AutumnsTorment");

            GoToPlayCardPhase(celadroch);
            PlayCard(card);
            AssertInPlayArea(celadroch, card);
            AssertInPlayArea(celadroch, avatar); //played by ongong

            DestroyCard(avatar); //send to trash
            AssertInTrash(avatar);

            //card destroys self
            DealDamage(ra, celadroch, 16, DamageType.Cold);
            AssertInTrash(card);

            AssertInPlayArea(celadroch, avatar);
            AssertInPlayArea(celadroch, c1);
            AssertInDeck(celadroch, c2);
            AssertInDeck(celadroch, c3);
            AssertInDeck(celadroch, c4);

        }

        [Test()]
        public void TestLingeringExhalation_Basic()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath" });
            StartGame(false);
            SafetyRemovePillars();

            var card = GetCard("LingeringExhalation");
            var avatar = GetCard("AvatarOfDeath");

            GoToPlayCardPhase(celadroch);
            PlayCard(card);
            AssertInPlayArea(celadroch, card);
            AssertInPlayArea(celadroch, avatar); //played by ongong

            DestroyCard(avatar); //send to trash
            AssertInTrash(avatar);

            //nothing happens
            DealDamage(ra, celadroch, 5, DamageType.Cold);
            AssertInPlayArea(celadroch, card);

            //nothing happens
            DealDamage(ra, celadroch, 10, DamageType.Cold);
            AssertInPlayArea(celadroch, card);

            //card destroys self
            DealDamage(ra, celadroch, 1, DamageType.Cold);
            AssertInTrash(card);

            AssertInPlayArea(celadroch, avatar);
        }


        [Test()]
        public void TestLingeringExhalation_TestZombiesGetPlayedCorrectly()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "AvatarOfDeath" });
            StartGame(false);
            SafetyRemovePillars();

            DrawCard(ra);
            var card = GetCard("LingeringExhalation");
            var avatar = GetCard("AvatarOfDeath");
            var c1 = PutInTrash("GraspingBreath");
            var c2 = GetCard("LeechingBreath");
            var c3 = GetCard("WhisperingBreath");

            GoToPlayCardPhase(celadroch);
            PlayCard(card);
            AssertInPlayArea(celadroch, card);
            AssertInPlayArea(celadroch, avatar); //played by ongong

            DestroyCard(avatar); //send to trash
            AssertInTrash(avatar);

            //card destroys self
            DealDamage(ra, celadroch, 16, DamageType.Cold);
            AssertInTrash(card);

            AssertInPlayArea(celadroch, avatar);
            AssertNextToCard(c1, ra.CharacterCard);
            AssertInDeck(celadroch, c2);
            AssertInDeck(celadroch, c3);
        }


        [Test()]
        public void TestLordOfTheMidnightRevel_CounterDamage()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            SuppressCeladrochMinionPlay();
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "LordOfTheMidnightRevel" });
            StartGame(false);
            SafetyRemovePillars();
            GoToPlayCardPhase(celadroch);

            var card = GetCard("LordOfTheMidnightRevel");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            QuickHPStorage(celadroch.CharacterCard, card, ra.CharacterCard, haka.CharacterCard, legacy.CharacterCard);
            //deal damage to lord, no response
            DealDamage(ra, card, 2, DamageType.Fire);
            QuickHPCheck(0, -2, 0, 0, 0);

            QuickHPUpdate();
            //damage to celadroch, counter damage
            DealDamage(ra, celadroch, 2, DamageType.Fire);
            QuickHPCheck(-2, 0, -3, 0, 0);

            QuickHPUpdate();
            //counter damage, once per turn
            DealDamage(ra, celadroch, 2, DamageType.Fire);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }

        [Test()]
        public void TestLordOfTheMidnightRevel_DestructionEffect()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Ra", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            AddTokensToPool(stormPool, 3);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            StackDeckAfterShuffle(celadroch, new[] { "LordOfTheMidnightRevel" });
            StartGame(false);
            SafetyRemovePillars();
            GoToPlayCardPhase(celadroch);

            var card = GetCard("LordOfTheMidnightRevel");
            PlayCard(card);
            AssertInPlayArea(celadroch, card);

            //load out a bunch of hero ongoings and equipments
            var c1 = PlayCard("Dominion");
            var c2 = PlayCard("TheLegacyRing");
            var c3 = PlayCard("NextEvolution");
            var c4 = PlayCard("ImbuedFire");
            var c5 = PlayCard("WrathfulGaze");

            DecisionSelectCards = new[] { c1, c2, c3, c4, c5 };

            DestroyCard(card, ra.CharacterCard);
            //2 other non-character targets in play, so 2 cards destoryed;
            AssertInTrash(c1);
            AssertInTrash(c2);
            AssertIsInPlay(c3);
            AssertIsInPlay(c4);
            AssertIsInPlay(c5);
        }

        [Test()]
        public void TestPillarOfNight_CrushingRiftInteraction()
        {
            //from issue #764

            SetupGameController(new[] { "Cauldron.Celadroch", "Cauldron.Impact", "Haka", "Legacy", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            //H = 3, 4 damage per trigger, 6 total triggers possible
            DecisionSelectTurnTaker = impact.HeroTurnTaker;
            QuickHandStorage(impact);
            DealDamage(impact, p1, 4, DamageType.Cold);
            QuickHandCheck(1);

            //Pillar at 21 hp
            DecisionSelectTarget = p1;
            QuickHPStorage(p1);
            PlayCard(impact, "CrushingRift");
            QuickHPCheck(-10);
            QuickHandCheck(2);
        }

        [Test]
        public void TestCeladrochChallenge()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Cauldron.Impact", "Haka", "Legacy", "Megalopolis" }, challenge: true);
            StartGame();
            //At the start of the game, put a token in the storm pool.
            //the start of the villain turn will auto add a token, so there should be two
            AssertTokenPoolCount(stormPool, 2);
        }
    }
}

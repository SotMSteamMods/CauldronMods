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
    public class CeladrochTests : BaseTest
    {
        #region HelperFunctions

        protected TokenPool stormPool => celadroch.CharacterCard.FindTokenPool(CeladrochCharacterCardController.StormPoolIdentifier);
        protected TurnTakerController celadroch { get { return FindVillain("Celadroch"); } }

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
            AssertCardSpecialString(celadroch.CharacterCard, 0, $"Celadroch's top card is {topCard.Title}");

            AssertNumberOfCardsInRevealed(celadroch, 0);
        }

        [Test()]
        public void TestCeladroch_DeckList()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Haka", "Ra", "Megalopolis");

            AssertCardHasKeyword(celadroch.CharacterCard, "villain", false);

            AssertCard("PillarOfNight", new string[] { "relic" }, 25);
            AssertCard("PillarOfSky", new string[] { "relic" }, 25);
            AssertCard("PillarOfStorms", new string[] { "relic" }, 25);

            AssertCard("AvatarOfDeath", new string[] { "avatar" }, 20);

            AssertCard("SummersWrath", new string[] { "elemental" }, 5);
            AssertCard("WintersBane", new string[] { "elemental" }, 5);
            AssertCard("SpringsAtrophy", new string[] { "elemental" }, 5);
            AssertCard("AutumnsTorment", new string[] { "elemental" }, 5);

            AssertCard("TatteredDevil", new string[] { "demon" }, 10);
            AssertCard("HollowAngel", new string[] { "demon" }, 10);

            AssertCard("WhisperingBreath", new string[] { "zombie" }, 6);
            AssertCard("GraspingBreath", new string[] { "zombie" }, 6);
            AssertCard("LeechingBreath", new string[] { "zombie" }, 6);

            AssertCard("ForsakenCrusader", new string[] { "chosen" }, 3);
            AssertCard("LordOfTheMidnightRevel", new string[] { "chosen" }, 12);
            AssertCard("LaughingHag", new string[] { "chosen" }, 5);

            AssertCard("ScreamingGale", new string[] { "ongoing" });
            AssertCard("HoursTilDawn", new string[] { "ongoing" });
            AssertCard("RattlingWind", new string[] { "ongoing" });
            AssertCard("NightUnderTheMountain", new string[] { "ongoing" });
            AssertCard("LingeringExhalation", new string[] { "ongoing" });

            AssertCard("GallowsBlast", new string[] { "one-shot" });
            AssertCard("TheMountainsMadness", new string[] { "one-shot" });
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
        public void TestCeladroch_PillarCard_RemoveFromGameDamge([Values("PillarOfNight", "PillarOfSky", "PillarOfStorms")] string pillar)
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
        public void TestCeladroch_CardsPlayedNoDestoryCards()
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
            
            foreach(var c in cards)
            {
                AssertIsInPlay(c);
            }
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestZombies_MoveNextToAndDealDamage([Values("GraspingBreath","LeechingBreath","WhisperingBreath")] string zombie)
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
            GoToEndOfTurn(celadroch);

            //celadroch's end of turn damage is included.
            QuickHPCheck(-2, -5, 0);
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
            GoToEndOfTurn(celadroch);

            //celadroch's end of turn damage is included.
            QuickHPCheck(-2, -4, -4);
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
            GoToEndOfTurn(celadroch);

            //celadroch's end of turn damage is included.
            QuickHPCheck(-3, -5, -5);
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
    }
}

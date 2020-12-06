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
        public void TestCeladroch_NormalFrontNoRelicDr()
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
        public void TestCeladroch_PillarOfNight_RewardTrigger()
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
        public void TestCeladroch_PillarOfSky_RewardTrigger()
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
        public void TestCeladroch_PillarOfStorms_RewardTrigger()
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
        public void TestCeladroch_Pillars_ProvideDr()
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
    }
}
